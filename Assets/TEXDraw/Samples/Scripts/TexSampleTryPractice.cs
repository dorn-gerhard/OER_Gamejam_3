using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TexDrawLib.Core;

using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib.Samples
{
    public class TexSampleTryPractice : MonoBehaviour
    {
        public TMPro.TMP_InputField input;
        public TEXDraw suggestTxt;
        public TEXDraw benchmarkTxt;
        public TEXDraw tex;
        public Dropdown dropdown;

        [Space]
        public int curSize;
        public int customFont;
        public int[] sizeOps;
        public TextAsset templates;
        private List<string> templateItems = new List<string>();

        List<string> symbols;
        List<string> commands;

        private const string emptySuggestion =
            @"Type a backslash '\backslash' and suggestions will show here";

        private const string startSuggestion =
            @"You just typed a backslash. Type an alphabet more to show a filtered list from {0} symbols and {1} commands in TEXDraw";

        // When input text gets changed ...
        public void InputUpdate()
        {
            //    dropdown.value = System.Array.IndexOf(templates, input.text);
            //Standard Update....
            tex.text = input.text;

            //Go find some suggestions...
            string typed = DetectTypedSymbol(input.text, input.caretPosition);
            string suggest;
            if (string.IsNullOrEmpty(typed))
                suggest = emptySuggestion;
            else if (typed == "\\")
                suggest = string.Format(startSuggestion, symbols.Count, commands.Count);
            else
                suggest = GetPossibleSymbols(typed.Substring(1));
            suggestTxt.text = suggest;
            DoBenchmark();
        }

        private IEnumerator Start()
        {
            var op = dropdown.options;
            op.Clear();
            op.Add(new Dropdown.OptionData("Custom (Use Here for Template)"));
            var items = templates.text.Replace("\r", "").Split("\n\n");

            for (int i = 0; i < items.Length; i++)
            {
                var itemSplit = items[i].Split(new char[] { '\n' }, 2);
                if (itemSplit.Length == 2)
                {

                    templateItems.Add(itemSplit[1]);
                    op.Add(new Dropdown.OptionData(itemSplit[0]));
                }
            }
            dropdown.value = 0;


            yield return null;

            var symbolsD = new HashSet<string>(tex.preference.symbols.Keys);
            commands = new List<string>();
            foreach (var item in tex.orchestrator.parser.aliasCommands)
            {
                if (symbolsD.Contains(item.Value))
                {
                    symbolsD.Add(item.Key);
                }
            }
            foreach (var item in tex.orchestrator.parser.allCommands)
            {
                if (!symbolsD.Contains(item))
                {
                    commands.Add(item);
                }
            }
            symbols = new List<string>(symbolsD);
            commands.Sort();
            symbols.Sort();
        }

        private static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        //static float lastBenchmark;
        private void DoBenchmark()
        {
            stopwatch.Reset();
            stopwatch.Start();

            // tex.SetTextDirty();
            tex.orchestrator.Parse(tex.text);
            tex.orchestrator.Box();
            tex.orchestrator.Render();

            stopwatch.Stop();

            //var end = () * 1000;

            benchmarkTxt.text = "\\it" + stopwatch.Elapsed.TotalMilliseconds.ToString("0.00") + " ms";
        }

        public string DetectTypedSymbol(string full, int caretPos)
        {
            string watchedStr = input.text.Substring(0, input.caretPosition);
            return Regex.Match(watchedStr, @"\\[\w]*$").Value;
        }

        private List<string> keys = ListPool<string>.Get();

        public string GetPossibleSymbols(string raw)
        {
            keys.Clear();

            var repRaw = "{\\bf\\color{white}{}" + raw + "}";
            foreach (var item in commands)
            {
                if (item.Contains(raw))
                    keys.Add("{\\color{yellow}" + item.Replace(raw, repRaw) + "}");
            }
            if (keys.Count > 0)
                keys.Add("----------");
            foreach (var item in symbols)
            {
                if (item.Contains(raw))
                    keys.Add("{\\large \\" + item + " } {\\color{grey} " + item.Replace(raw, repRaw) + "}");
            }

            return string.Join("\\\\", keys.ToArray());
        }

        public void UpdateAlignment(int alignment)
        {
            tex.alignment = new Vector2(alignment / 2f, 0.5f);
        }

        public void AlignmentLeft(bool yes)
        {
            if (yes)
                UpdateAlignment(0);
        }

        public void AlignmentCenter(bool yes)
        {
            if (yes)
                UpdateAlignment(1);
        }

        public void AlignmentRight(bool yes)
        {
            if (yes)
                UpdateAlignment(2);
        }

        public void UpdateWrap(int wrap)
        {
            // tex.autoWrap = (Wrapping)(wrap + (rtlMode ? 4 : 0));
        }

        public void UpdateTemplate(int idx)
        {
            if (idx > 0)
                input.text = templateItems[idx - 1];
        }

        public void UpdateCustomFont(bool custom)
        {
            // tex.fontIndex = custom ? customFont : -1;
        }

        public void UpdateSize(bool larger)
        {
            curSize = Mathf.Clamp(curSize + (larger ? 1 : -1), 0, sizeOps.Length - 1);
            tex.size = sizeOps[curSize];
        }

        public void UpdateRTL(bool RTL)
        {
            // rtlMode = RTL;
            // tex.autoWrap = (Wrapping)((int)tex.autoWrap % 4 + (rtlMode ? 4 : 0));
            // tex.GetComponent<TEXSupRTLSupport>().enabled = RTL;
        }
    }
}
