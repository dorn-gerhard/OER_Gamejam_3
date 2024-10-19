using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TexDrawLib.Core;
using System.Text.RegularExpressions;
using Block = TexDrawLib.TEXInput.Block;
using System.Text;

namespace TexDrawLib
{
    [AddComponentMenu("TEXDraw/TEXInput Logger")]
    public class TEXInputLogger : MonoBehaviour
    {


        internal List<Block> blocks = new List<Block>();

        public string emptyPlaceholder = "\\w";

        public string templateInput = "...";

        public TEXInput input;

        private StringBuilder _tmpStrBuilder = new StringBuilder();

        public string ReplaceString(string input)
        {
            if (this.input.tex.orchestrator == null)
            {
                return input;
            }

            blocks.Clear();
            _tmpStrBuilder.Clear();
            int i = 0;

            ReplaceString(input, _tmpStrBuilder, -1, ref i, input.Length);

            return templateInput.Replace("...", _tmpStrBuilder.ToString());
        }

        private void ReplaceString(string input, StringBuilder output, int parent, ref int pos, int length)
        {
            if (pos == length)
            {
                var dyn = new Block()
                {
                    index = blocks.Count,
                    start = pos,
                    length = 0,
                    group = parent
                };
                var cmd = "\\link[" + dyn.index + "]";
                output.Append(cmd + "{" + emptyPlaceholder + "}");
                blocks.Add(dyn);
            }
            else
            {
                while (pos < length)
                {
                    ReplaceNextToken(input, output, parent, ref pos, length);
                }
            }
        }

        private void ReplaceNextToken(string input, StringBuilder output, int parent, ref int pos, int length)
        {
            if (pos >= length)
                return;
            var ch = input[pos];
            var dyn = new Block()
            {
                index = blocks.Count,
                start = pos++,
                length = 1,
                group = parent
            };
            var cmd = "\\link[" + dyn.index + "]";
            if (ch == '\r' || ch == '\t')
            {
            }
            else if (ch == '\n')
            {
                blocks.Add(dyn);
                output.Append("\\\\\n" + cmd + "{}");
            }
            else if (ch == ' ')
            {
                blocks.Add(dyn);
                output.Append(cmd + "{\\ }");
            }
            else if (!TexParserUtility.reservedChars.Contains(ch))
            {
                blocks.Add(dyn);
                output.Append(cmd + "{" + ch + "}");
            }
            else if (ch == '$')
            {
                var end = input.IndexOf('$', pos);
                if (end == pos)
                {
                    // math block
                    var end2 = input.IndexOf("$$", pos + 1);
                    if (end2 >= 0)
                    {
                        dyn.length = end2 + 1 - dyn.start;
                        blocks.Add(dyn);
                        output.Append(cmd + "{$$");
                        pos += 1;
                        ReplaceString(input, output, dyn.index, ref pos, end2);
                        pos += 2;
                        output.Append("$$}");
                        //dyn.group = blocks.Count;
                        blocks[dyn.index] = dyn;
                    }
                    else
                    {
                        pos--;
                        ReplaceStringInRaw(input, output, dyn.index, ref pos, length);
                    }
                }
                else if (end >= 0)
                {
                    dyn.length = end - dyn.start;
                    blocks.Add(dyn);
                    output.Append(cmd + "{$");
                    ReplaceString(input, output, dyn.index, ref pos, end);
                    pos++;
                    output.Append("$}");
                    //dyn.group = blocks.Count;
                    blocks[dyn.index] = dyn;
                }
                else
                {
                    pos--;
                    ReplaceStringInRaw(input, output, dyn.index, ref pos, length);
                }
            }
            else if (ch == '{')
            {
                var end = pos;
                TexParserUtility.ReadGroup(input, ref end);
                if (end == input.Length)
                {
                    pos--;
                    ReplaceStringInRaw(input, output, dyn.index, ref pos, length);
                }
                else
                {
                    output.Append(cmd + "{");
                    if (end == length)
                        output.Append("{\\tt\\braceleft}");
                    else
                        dyn.length = end - dyn.start + 1;
                    blocks.Add(dyn);
                    ReplaceString(input, output, dyn.index, ref pos, end);
                    pos++;
                    output.Append("}");
                    //dyn.group = blocks.Count;
                    blocks[dyn.index] = dyn;
                }

            }
            else if (ch == '}')
            {
                blocks.Add(dyn);
                output.Append(cmd + "{\\tt\\braceright}");
            }
            else if (ch == '^' || ch == '_')
            {
                if (blocks.Count == 0)
                {
                    // dummy base
                    ReplaceString(input, output, parent, ref pos, pos);
                }
                var baseBlock = blocks[blocks.Count - 1];
                while (baseBlock.group != -1 && baseBlock.group != parent)
                {
                    baseBlock = blocks[baseBlock.group];
                }
                // move end brace to the end
                output.Remove(output.Length - 1, 1);
                while (ch == '^' || ch == '_')
                {
                    if (pos < length)
                    {
                        output.Append(ch);
                        ReplaceStringUnpackBrace(input, output, baseBlock.index, ref pos, length);
                        TexParserUtility.SkipWhiteSpace(input, ref pos);
                        if (pos >= length) break;
                        ch = input[pos];
                        if (ch == '^' || ch == '_')
                        {
                            pos++;
                        }
                    }
                    else
                    {
                        pos--;
                        ReplaceStringInRaw(input, output, baseBlock.index, ref pos, length);
                        break;
                    }
                }
                output.Append("}");
                baseBlock.length = pos - baseBlock.start;
                blocks[baseBlock.index] = baseBlock;
            }
            else if (ch == '\\')
            {
                string word = TexParserUtility.LookForAWord(input, ref pos);
                var parser = this.input.tex.orchestrator.parser;
                if (word.Length == 0)
                {
                    if (pos == length)
                    {
                        blocks.Add(dyn);
                        output.Append(cmd + "{\\tt\\backslash}");
                    }
                    else if (input[pos] == '[' || input[pos] == '(')
                    {
                        // To do
                    }
                }
                else if (parser.allCommands.Contains(word))
                {
                    dyn.length += word.Length;
                    blocks.Add(dyn);
                    output.Append(cmd + "{\\" + word);
                    if (word == "frac" || word == "nfrac")
                    {
                        ReplaceStringUnpackBrace(input, output, dyn.index, ref pos, length);
                        ReplaceStringUnpackBrace(input, output, dyn.index, ref pos, length);
                        dyn.length = pos - dyn.start;
                        blocks[dyn.index] = dyn;
                    }
                    else if (word == "sqrt" || word == "root")
                    {
                        if (pos < input.Length && input[pos] == '[')
                        {
                            ReplaceStringUnpackBrace(input, output, dyn.index, ref pos, length, '[', ']');
                        }
                        ReplaceStringUnpackBrace(input, output, dyn.index, ref pos, length);
                        dyn.length = pos - dyn.start;
                        blocks[dyn.index] = dyn;
                    }
                    output.Append("}");
                }
                else
                {
                    pos -= word.Length + 1;
                    ReplaceStringInRaw(input, output, dyn.index, ref pos, pos + word.Length + 1);
                }
            }
        }

        private void ReplaceStringUnpackBrace(string input, StringBuilder output, int parent, ref int pos, int length, char braceleft = '{', char braceright = '}')
        {
            TexParserUtility.SkipWhiteSpace(input, ref pos);
            if (pos < input.Length && input[pos] == braceleft)
            {
                var oldpos = pos + 1;
                TexParserUtility.ReadGroup(input, ref pos, braceleft, braceright);
                output.Append(braceleft);
                ReplaceString(input, output, parent, ref oldpos, pos - 1);
                output.Append(braceright);
                pos = oldpos + 1;
            }
            else
            {
                ReplaceNextToken(input, output, parent, ref pos, length);
            }
        }

        private void ReplaceStringInRaw(string input, StringBuilder output, int parent, ref int pos, int length)
        {
            while (pos < length)
            {
                var ch = input[pos];
                var dyn = new Block()
                {
                    index = blocks.Count,
                    start = pos++,
                    length = 1,
                    group = parent
                };
                var cmd = "\\link[" + dyn.index + "]";
                if (ch == '\r' || ch == '\t')
                {
                    continue;
                }
                else if (ch == '\n')
                {
                    blocks.Add(dyn);
                    output.Append("\\\\\n" + cmd + "{}");
                }
                else if (ch == ' ')
                {
                    blocks.Add(dyn);
                    output.Append(cmd + "{\\ }");
                }
                else if (!TexParserUtility.reservedChars.Contains(ch))
                {
                    blocks.Add(dyn);
                    output.Append(cmd + "{\\tt{" + ch + "}}");
                }
                else if (ch == '\\')
                {
                    blocks.Add(dyn);
                    output.Append(cmd + "{\\tt{\\backslash}}");
                }
                else
                {
                    blocks.Add(dyn);
                    output.Append(cmd + "\\verb|" + ch + "|");
                }
            }
        }

        internal IEnumerable<Block> GetBlockMatches(int start, int length)
        {
            return blocks.Where((x) => x.start >= start && (x.end) <= (start + length));
        }

        internal Block GetBlockClosest(int pos)
        {
            if (blocks.Count == 0)
            {
                return new Block
                {
                    index = -1
                };
            }
            Block b = blocks[0];
            // find just before the pos
            foreach (var i in blocks)
            {
                if (i.start >= pos)
                    break;
                if (i.start > b.start)
                {
                    b = i;
                }
            }
            // make sure we don't do partial select
            while (b.group >= 0 && !blocks[b.group].isInside(pos))
            {
                if (b.index == b.group)
                {
                    break;
                }
                b = blocks[b.group];
            }
            // find closer if we can
            if (b.isInside(pos) || !b.isAround(pos))
            {
                var n = GetNext(b);
                if (n.isAround(pos))
                {
                    b = n;
                }
            }
            return b;
        }

        internal Block GetNext(Block block)
        {
            if (block.index < blocks.Count - 1)
                return blocks[block.index + 1];
            else
                return block;
        }

        internal Block GetPrevious(Block block)
        {
            if (block.index > 0)
                return blocks[block.index - 1];
            else
                return block;
        }

        public int GetNearestPosition(Vector2 pos)
        {
            var param = input.tex.orchestrator;
            var boxes = param.rendererState.vertexLinks;

            // scan nearest
            (float dist, Block block, bool right) nearest = (99999, default, false);
            foreach ((string key, Rect area) in boxes)
            {
                var d = SqDistance(area, pos);
                if (d < nearest.dist && TexUtility.TryParse(key, out int index))
                    nearest = (d, blocks[index], pos.x > area.center.x);
            }
            return nearest.block.start + (nearest.right ? nearest.block.length : 0);
        }

        private static float SqDistance(Rect r, Vector2 p)
        {
            var c = r.center;
            var dx = Math.Abs(Math.Abs(p.x - c.x) - r.width / 2);
            var dy = Math.Abs(Math.Abs(p.y - c.y) - r.height / 2);
            return dx * dx + dy * dy * 9; // yeah assume vertical is "heavier" here
        }
    }
}
