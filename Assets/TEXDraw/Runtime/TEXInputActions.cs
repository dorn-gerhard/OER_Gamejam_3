using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace TexDrawLib
{
    public partial class TEXInput : Selectable, IUpdateSelectedHandler, IDragHandler
    {

        public bool escapeInput = true;

        private int isBeforeACommand(int pos)
        {
            while (pos >= 0)
            {
                var ch = text[pos];
                if (char.IsLetterOrDigit(ch))
                {
                    pos--;
                    continue;
                }
                else if (ch == '\\')
                {
                    if (pos > 0 && text[pos - 1] == '\\')
                    {
                        return -1;
                    }
                    return pos;
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        private void Append(char ch)
        {
            string s = new string(ch, 1);
            if (escapeInput)
            {
                switch (ch)
                {
                    case '\\':
                        s = "\\backslash";
                        break;
                    case '$':
                        s = "\\dollar";
                        break;
                    case '{':
                        s = "\\braceleft";
                        break;
                    case '}':
                        s = "\\braceright";
                        break;
                    case '%':
                        s = "\\percent";
                        break;
                    case '#':
                        s = "\\numbersign";
                        break;
                }
            }
            var posCmd = isBeforeACommand(selectionStart - 1);
            if (posCmd >= 0)
            {
                text = text.Substring(0, posCmd) + "{" + text.Substring(posCmd, selectionStart - posCmd) + "}" + text.Substring(selectionStart);
                selectionBegin += 2;
            }
            SetSelection(s);
            selectionBegin = selectionEnd;
        }


        public void SetSelection(string text)
        {
            if (text != this.selectedText)
            {
                if (escapeInput && selectionLength > 0)
                {
                    Backspace();
                }
                this.selectedText = text;
                m_OnChange.Invoke(text);
            }
        }

        readonly static char[] brackets = new char[] { '{', '}' };

        private void Backspace()
        {
            if (hasSelection)
            {
                if (escapeInput)
                {
                    var lgs = logger.GetBlockMatches(selectionStart, selectionLength);
                    var t = text;
                    int offset = 0, start = 0;

                    foreach (var lg in lgs)
                    {
                        if (lg.length > 0 && lg.start >= start)
                        {
                            t = t.Remove(lg.start - offset, lg.length);
                            start = lg.end;
                            offset += lg.length;
                        }
                    }
                    text = t;
                    selectionBegin = selectionStart;
                }
                else
                {

                    selectedText = "";
                    selectionBegin = selectionStart;
                }
            }
            else if (selectionStart > 0)
            {
                if (escapeInput)
                {
                    var g = logger.GetBlockClosest(selectionBegin);
                    text = text.Remove(g.start, g.length);
                    selectionBegin -= g.length;
                }
                else
                {
                    text = text.Remove(selectionBegin - 1, 1);
                    selectionBegin--;
                }
            }
        }

        private void Delete()
        {
            if (hasSelection)
            {
                Backspace();
            }
            else if (selectionStart < text.Length)
            {
                if (escapeInput)
                {
                    var s = selectionBegin;
                    var g = logger.GetBlockClosest(s);
                    while (g.isInside(s) && g.index < logger.blocks.Count - 1)
                    {
                        g = logger.GetNext(g);
                    }
                    text = text.Remove(g.start, g.length);
                }
                else
                {
                    text = text.Remove(selectionBegin, 1);
                }

            }
        }

        private void Move(bool shift, Func<int, int> mover)
        {
            var x = shift | hasSelection ? selectionEnd : selectionBegin;

            if (shift)
                selectionEnd = mover(x);
            else
                selectionBegin = mover(x);
        }

        private int MoveLeft(int x)
        {
            if (x <= 0)
            {
                return x;
            }
            else if (escapeInput)
            {
                var candidates = TexDrawLib.Core.ListPool<int>.Get();
                var b = logger.GetBlockClosest(x);
                var orib = b;
                candidates.Add(b.start);
                candidates.Add(b.end);
                // childrens
                b = logger.GetNext(orib);
                while (b.group != orib.group && b.index != logger.blocks.Count - 1)
                {
                    b = logger.GetNext(b);
                }
                for (int i = orib.index; i < b.index; i++)
                {
                    candidates.Add(logger.blocks[i].start);
                    candidates.Add(logger.blocks[i].end);
                }
                // parents
                b = orib;
                while (b.group != -1 && b.group != b.index)
                {
                    b = logger.blocks[b.group];
                    candidates.Add(b.start);
                    candidates.Add(b.end);
                }
                // its previous
                b = logger.GetPrevious(orib);
                while (b.group != orib.group && b.index != 0)
                {
                    b = logger.GetPrevious(b);
                }
                candidates.Sort();
                candidates.Reverse();
                foreach (int ex in candidates)
                {
                    if (ex < x)
                    {
                        x = ex;
                        break;
                    }
                }
                TexDrawLib.Core.ListPool<int>.Release(candidates);
                return x;
            }
            else
            {
                return x - 1;
            }
        }

        private int MoveRight(int x)
        {
            if (x >= text.Length)
            {
                return x;
            }
            else if (escapeInput)
            {
                var candidates = TexDrawLib.Core.ListPool<int>.Get();
                var b = logger.GetBlockClosest(x);
                var orib = b;
                candidates.Add(b.start);
                candidates.Add(b.end);
                // childrens
                while (logger.GetNext(b).group == b.index)
                {
                    b = logger.GetNext(b);
                    candidates.Add(b.start);
                    candidates.Add(b.end);
                }
                // parents
                b = orib;
                while (b.group != -1 && b.group != b.index)
                {
                    b = logger.blocks[b.group];
                    candidates.Add(b.start);
                    candidates.Add(b.end);
                }
                // its next
                b = logger.GetNext(orib);
                while (b.group != orib.group && b.index != logger.blocks.Count - 1)
                {
                    b = logger.GetNext(b);
                }
                candidates.Add(b.start);
                candidates.Add(b.end);
                candidates.Sort();
                foreach (int ex in candidates)
                {
                    if (ex > x)
                    {
                        x = ex;
                        break;
                    }
                }
                TexDrawLib.Core.ListPool<int>.Release(candidates);
                return x;
            }
            else
            {
                return x + 1;
            }
        }

        private int MoveUp(int x)
        {
            var up = text.Take(x);
            var line = up.Count(c => c == '\n');
            if (line == 0) return x;

            var lines = up.Select((c, i) => new { c, i }).Where(cc => cc.c == '\n').Select(cc => cc.i);
            var dist = x - lines.LastOrDefault();
            var pos = lines.ElementAtOrDefault(line - 2) + dist;
            return pos;
        }

        private int MoveDown(int x)
        {
            var down = text.Skip(x);
            var line = down.Count(c => c == '\n');
            if (line == 0) return x;

            var lines = text.Take(x).Select((c, i) => new { c, i }).Where(cc => cc.c == '\n').Select(cc => cc.i);
            var dist = x - (lines.LastOrDefault());
            var pos = x + down.Select((c, i) => new { c, i }).Where(cc => cc.c == '\n').First().i + dist;
            return pos;
        }

        private struct UndoState
        {
            public string text;
            int selectionStart;
            int selectionLength;

            public UndoState(TEXInput input)
            {
                text = input.text;
                selectionStart = input.selectionStart;
                selectionLength = input.selectionLength;
            }

            public void Apply(TEXInput input)
            {
                input.text = text;
                input.selectionStart = selectionStart;
                input.selectionLength = selectionLength;
            }

            public static bool IsEqual(UndoState l, UndoState r)
            {
                return l.text == r.text && l.selectionLength == r.selectionLength
                    && l.selectionStart == r.selectionStart;
            }
        }

        [NonSerialized]
        private List<UndoState> undoStack = new List<UndoState>();

        /// <summary>
        /// Record undo
        /// </summary>
        public void RecordUndo()
        {
            var st = new UndoState(this);
            if (undoStack.Count == 0 || !UndoState.IsEqual(st, undoStack[undoStack.Count - 1]))
            {
                undoStack.Add(st);
                // maybe the cap is too subjective
                if (undoStack.Count > 15)
                    undoStack.RemoveAt(0);
            }
        }

        private bool Undo()
        {
            if (undoStack.Count > 0)
            {
                var now = undoStack[undoStack.Count - 1];
                undoStack.RemoveAt(undoStack.Count - 1);

                if (now.text == text)
                    return Undo();
                else
                    now.Apply(this);
                return true;
            }
            return false;
        }
    }
}
