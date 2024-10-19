using UnityEngine;
using UnityEngine.UIElements;

namespace TexDrawLib.Core
{
    public class HorizontalRuleAtom : AbstractAtom
    {
        public float top;
        public float bottom;
        public float thickness;
        public float scale;
        public Color32 color;

        public static HorizontalRuleAtom Get()
        {
            return ObjPool<HorizontalRuleAtom>.Get();
        }

        public override Box CreateBox(TexBoxingState state)
        {
            var vbox = VerticalBox.Get();
            if (top > 0)
                vbox.Add(StrutBox.Get(0, top, 0));
            if (thickness > 0)
                vbox.Add(RuleBox.Get(color, state.width, thickness, 0, scale));
            if (bottom > 0)
                vbox.Add(StrutBox.Get(0, top, 0));
            return vbox;
        }

        public override void Flush()
        {
            top = bottom = thickness = scale = 0;
            ObjPool<HorizontalRuleAtom>.Release(this);
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var ratio = state.Ratio;
            scale = state.Size.current;
            thickness = state.Math.lineThickness * ratio;
            color = state.Color.current;
            switch (command)
            {
                case "toprule":
                    thickness *= 2;
                    bottom = state.Paragraph.lineSpacing;
                    break;
                case "bottomrule":
                    thickness *= 2;
                    top = state.Paragraph.lineSpacing;
                    break;
                case "midrule":
                    bottom = top = state.Paragraph.lineSpacing;
                    break;
                case "hline":
                default:
                    break;
            }
        }
    }
}
