using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TexDrawLib.Core
{
    public class TexHyphenation
    {
        private const char Marker = '.';
        private readonly bool _hyphenateLastWord;
        private readonly bool _sortPatterns;
        private readonly string _hyphenateSymbol;
        private readonly int _minLetterCount;
        private readonly int _minWordLength;
        private Dictionary<string, int[]> _exceptions = new Dictionary<string, int[]>();
        private List<Pattern> _patterns;
        private static readonly Regex CreateMaskRegex = new Regex(@"\w", RegexOptions.Compiled);
        private static readonly Regex StripTexComments = new Regex(@"\s*%.*", RegexOptions.Compiled);

        /// <summary>
        /// Implementation of Frank Liang's hyphenation algorithm
        /// </summary>
        /// <param name="loader">ILoader for load hyphenation patterns</param>
        /// <param name="hyphenateSymbol">Symbol used for denote hyphenation</param>
        /// <param name="minWordLength">Minimum word length for hyphenation word</param>
        /// <param name="minLetterCount">Minimum number of characters left on line</param>
        /// <param name="hyphenateLastWord">Hyphenate last word, NOTE: this option works only if input text contains more than one word</param>
        /// <param name="sortPatterns">Sort patterns before using, can be needed for some languages like German, Portuguese, etc. </param>
        public TexHyphenation(TextAsset loader,
            string hyphenateSymbol = "-",
            int minWordLength = 5,
            int minLetterCount = 3,
            bool hyphenateLastWord = false,
            bool sortPatterns = false)
        {
            _hyphenateSymbol = hyphenateSymbol;
            _minWordLength = minWordLength;
            _minLetterCount = minLetterCount >= 0 ? minLetterCount : 0;
            _hyphenateLastWord = hyphenateLastWord;
            _sortPatterns = sortPatterns;
            LoadPatterns(loader);
        }

        // See https://hyphenation.org/ for patterns
        private void LoadPatterns(TextAsset loader)
        {
            var text = loader.text;
            text = StripTexComments.Replace(text, "");
            var sep = new[] { ' ', '\n', '\r' };
            var pat = text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            var patterns = new List<string>();
            var exceptions = new List<string>();
            int i = 0;
             
            while (i < pat.Length)
            {
                if (pat[i++] == "\\patterns{")
                {
                    break;
                }
            }

            while (i < pat.Length)
            {
                var p = pat[i++];
                if (p == "}")
                {
                    break;
                }
                patterns.Add(p);
            }

            while (i < pat.Length)
            {
                if (pat[i++] == "\\hyphenation{")
                {
                    break;
                }
            }

            while (i < pat.Length)
            {
                var p = pat[i++];
                if (p == "}")
                {
                    break;
                }
                exceptions.Add(p);
            }

            CreatePatterns(patterns, exceptions);
        }

        private void CreatePatterns(List<string> patterns, List<string> exceptions)
        {
            var sep = new[] { ' ', '\n', '\r' };
            _patterns = patterns
                .Select(CreatePattern)
                .ToList();

            if (_sortPatterns)
            {
                _patterns = _patterns.OrderBy(x => x).ToList();
            }

            _exceptions = exceptions.ToDictionary(x => x.Replace("-", ""), CreateHyphenateMaskFromExceptionString);
        }


        public string HyphenateText(string text)
        {
            if (_hyphenateLastWord == false)
            {
                string lastWord = FindLastWord(text);
                if (lastWord.Length > 0)
                    text = text.Remove(text.Length - lastWord.Length);

                var result = HyphenateWordsInText(text);

                return result.Append(lastWord).ToString();
            }

            return HyphenateWordsInText(text).ToString();
        }

        private StringBuilder HyphenateWordsInText(string text)
        {
            var currentWord = new StringBuilder();
            var result = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                    currentWord.Append(c);
                else
                {
                    if (currentWord.Length > 0)
                    {
                        result.Append(HyphenateWord(currentWord.ToString()));
                        currentWord.Clear();
                    }

                    result.Append(c);
                }
            }

            result.Append(HyphenateWord(currentWord.ToString()));
            return result;
        }

        private string FindLastWord(string phrase)
        {
            var currentWord = new StringBuilder();
            for (int i = phrase.Length - 1; i >= 0; i--)
            {
                if (char.IsLetter(phrase[i]))
                    currentWord.Append(phrase[i]);
                else if (currentWord.Length > 0 && currentWord.ToString().Any(char.IsLetter))
                    return new string(currentWord.ToString().Reverse().ToArray());
                else
                    currentWord.Append(phrase[i]);
            }

            return string.Empty;
        }

        public string HyphenateWord(string originalWord)
        {
            if (IsNotValidForHyphenate(originalWord))
                return originalWord;

            string word = originalWord.ToLowerInvariant();
            int[] hyphenationMask;
            if (_exceptions.ContainsKey(word))
                hyphenationMask = _exceptions[word];
            else
            {
                int[] levels = GenerateLevelsForWord(word);
                hyphenationMask = CreateHyphenateMaskFromLevels(levels);
                CorrectMask(hyphenationMask);
            }

            return HyphenateByMask(originalWord, hyphenationMask);
        }

        private void CorrectMask(int[] hyphenationMask)
        {
            if (hyphenationMask.Length > _minLetterCount)
            {
                Array.Clear(hyphenationMask, 0, _minLetterCount);
                var correctionLength = _minLetterCount > 0 ? _minLetterCount - 1 : 0;
                Array.Clear(hyphenationMask, hyphenationMask.Length - correctionLength, correctionLength);
            }
            else
                Array.Clear(hyphenationMask, 0, hyphenationMask.Length);
        }

        private bool IsNotValidForHyphenate(string originalWord)
        {
            return originalWord.Length <= _minWordLength;
        }

        private int[] GenerateLevelsForWord(string word)
        {
            string wordString = new StringBuilder().Append(Marker).Append(word).Append(Marker).ToString();
            var levels = new int[wordString.Length];
            for (int i = 0; i < wordString.Length - 2; ++i)
            {
                int patternIndex = 0;
                for (int count = 1; count <= wordString.Length - i; ++count)
                {
                    var patternFromWord = new Pattern(wordString.Substring(i, count));
                    if (Pattern.Compare(patternFromWord, _patterns[patternIndex]) < 0)
                        continue;
                    patternIndex = _patterns.FindIndex(patternIndex,
                        pattern => Pattern.Compare(pattern, patternFromWord) > 0);
                    if (patternIndex == -1)
                        break;
                    if (Pattern.Compare(patternFromWord, _patterns[patternIndex]) >= 0)
                        for (int levelIndex = 0;
                            levelIndex < _patterns[patternIndex].GetLevelsCount() - 1;
                            ++levelIndex)
                        {
                            int level = _patterns[patternIndex].GetLevelByIndex(levelIndex);
                            if (level > levels[i + levelIndex])
                                levels[i + levelIndex] = level;
                        }
                }
            }

            return levels;
        }

        private static int[] CreateHyphenateMaskFromLevels(int[] levels)
        {
            int length = levels.Length - 2;
            var hyphenationMask = new int[length];
            for (int i = 0; i < length; i++)
            {
                if (i != 0 && levels[i + 1] % 2 != 0)
                    hyphenationMask[i] = 1;
                else
                    hyphenationMask[i] = 0;
            }

            return hyphenationMask;
        }

        private string HyphenateByMask(string originalWord, int[] hyphenationMask)
        {
            var result = new StringBuilder();
            for (int i = 0; i < originalWord.Length; i++)
            {
                if (hyphenationMask[i] > 0)
                    result.Append(_hyphenateSymbol);
                result.Append(originalWord[i]);
            }

            return result.ToString();
        }

        private int[] CreateHyphenateMaskFromExceptionString(string s)
        {
            int[] array = CreateMaskRegex.Split(s)
                .Select(c => c == "-" ? 1 : 0)
                .ToArray();
            return array;
        }

        private Pattern CreatePattern(string pattern)
        {
            var levels = new List<int>(pattern.Length);
            var resultStr = new StringBuilder();
            bool waitDigit = true;
            foreach (char c in pattern)
            {
                if (Char.IsDigit(c))
                {
                    levels.Add(Int32.Parse(c.ToString(CultureInfo.InvariantCulture)));
                    waitDigit = false;
                }
                else
                {
                    if (waitDigit)
                        levels.Add(0);
                    resultStr.Append(c);
                    waitDigit = true;
                }
            }

            if (waitDigit)
                levels.Add(0);

            return new Pattern(resultStr.ToString(), levels);
        }
    }

    internal sealed class Pattern : IComparer<Pattern>, IComparable<Pattern>
    {
        private readonly string str;
        private readonly int[] levels;

        public int GetLevelByIndex(int index)
        {
            return levels[index];
        }

        public int GetLevelsCount()
        {
            return levels.Length;
        }

        public Pattern(string str, IEnumerable<int> levels)
        {
            this.str = str;
            this.levels = levels.ToArray();
        }


        public Pattern(string str)
        {
            this.str = str;
            levels = new int[0];
        }

        public static int Compare(Pattern x, Pattern y)
        {
            bool first = x.str.Length < y.str.Length;
            int minSize = first ? x.str.Length : y.str.Length;
            for (var i = 0; i < minSize; ++i)
            {
                if (x.str[i] < y.str[i])
                    return -1;
                if (x.str[i] > y.str[i])
                    return 1;
            }
            return first ? -1 : 1;
        }

        int IComparer<Pattern>.Compare(Pattern x, Pattern y)
        {
            return Compare(x, y);
        }

        public int CompareTo(Pattern other)
        {
            return Compare(this, other);
        }
    }
}
