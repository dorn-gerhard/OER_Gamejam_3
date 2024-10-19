using UnityEngine;
using static TexDrawLib.Core.TexParserUtility;

namespace TexDrawLib.Core
{
    public class TransformAtom : InlineAtom
    {
        public float rotation = 0;
        public float rotationUnit = 360;
        public TexAlignment rotationAlignment = TexAlignment.Left | TexAlignment.Baseline;
        public Vector2 rotationOffset = Vector2.zero;
        public Vector2 scale = Vector2.one;

        public static TransformAtom Get()
        {
            return ObjPool<TransformAtom>.Get();
        }

        public static TransformAtom Get(Atom a)
        {
            var b = ObjPool<TransformAtom>.Get();
            b.atom = a;
            return b;
        }

        public override Box CreateBox(TexBoxingState state)
        {
            if (atom == null)
                return StrutBox.Empty;

            var box = TransformBox.Get(atom.CreateBox(state));
            box.scale = scale;
            box.rotation = rotation * Mathf.Deg2Rad * (rotationUnit / 360f);
            box.rotationOffset = rotationOffset;
            box.CalcAlignment(rotationAlignment);
            box.ApplyTransform();
            return box;
        }

        public override void Flush()
        {
           
            ObjPool<TransformAtom>.Release(this);
            scale = Vector2.one;
            rotation = 0;
            rotationUnit = 360;
            rotationAlignment = TexAlignment.Left | TexAlignment.Baseline;
            rotationOffset = Vector2.zero;
            atom?.Flush();
            atom = null;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
        
            if (value == null || position >= value.Length) return;
            SkipWhiteSpace(value, ref position);
            switch (command)
            {
                case "scalebox":
                    if (value[position] == beginGroupChar)
                    {
                        var scaleX = ReadGroup(value, ref position);
                        var scaleY = scaleX;
                        SkipWhiteSpace(value, ref position);
                        if (position < value.Length && value[position] == '[')
                        {
                            scaleY = ReadGroup(value, ref position, '[', ']');
                        }
                        TexUtility.TryParse(scaleX, out scale.x);
                        TexUtility.TryParse(scaleY, out scale.y);
                    }
                    break;
                case "rotatebox":
                    if (value[position] == '[')
                    {
                        var syntax = ReadGroup(value, ref position, '[', ']');
                        SkipWhiteSpace(value, ref position);
                        int s = 0, i = 0;
                        while (i < syntax.Length)
                        {
                            i = syntax.IndexOf(',', s + 1);
                            if (i == -1)
                                i = syntax.Length;
                            var conf = syntax.Substring(s, i - s);
                            var eq = conf.IndexOf('=');
                            if (eq != -1)
                            {
                                var val = conf.Substring(eq + 1);
                                switch (conf.Substring(0, eq))
                                {
                                    case "x":
                                        rotationOffset.x = TexUtility.ParseUnit(val, state);
                                        break;
                                    case "y":
                                        rotationOffset.y = TexUtility.ParseUnit(val, state);
                                        break;
                                    case "units":
                                        TexUtility.TryParse(val, out rotationUnit);
                                        break;
                                    case "origin":
                                        rotationAlignment = 0;
                                        for (int x = 0; x < val.Length; x++)
                                        {
                                            switch (val[x])
                                            {
                                                case 'l':
                                                    rotationAlignment |= TexAlignment.Left;
                                                    break;
                                                case 'r':
                                                    rotationAlignment |= TexAlignment.Right;
                                                    break;
                                                case 't':
                                                    rotationAlignment |= TexAlignment.Top;
                                                    break;
                                                case 'b':
                                                    rotationAlignment |= TexAlignment.Bottom;
                                                    break;
                                                case 'B':
                                                    rotationAlignment |= TexAlignment.Baseline;
                                                    break;
                                            }
                                        }
                                        break;
                                }
                            }
                            s = i;
                        }
                    }
                    if (position < value.Length && value[position] == beginGroupChar)
                    {
                        var rot = ReadGroup(value, ref position);
                        SkipWhiteSpace(value, ref position);
                        TexUtility.TryParse(rot, out rotation);
                    }
                    break;
                case "reflectbox":
                    scale = new Vector2(-1, 1);
                    break;
            }
            atom = state.parser.Parse(ReadGroup(value, ref position), state, true);
        }
    }
}
