using System;
using System.Collections.Generic;
using UnityEngine;

namespace TexDrawLib.Core
{
    public class RuleBox : StrutBox
    {

        private const string RuleSkin = "squaresolid";


        public static RuleBox Get(Color32 color, float width, float height, float depth, float scale)
        {
            var box = ObjPool<RuleBox>.Get();
            box.Set(width, height, depth);
            var config = TEXPreference.main.configuration;
            var resolution = config.Document.retinaRatio;
            // if border = config * size * ratio, then size = border / (config * ratio)
            box.ruleChar = CharBox.Get(TEXPreference.main.GetChar(RuleSkin), scale, resolution, color);
            return box;
        }

        public static void PutBorderVertical(HorizontalBox box, Color32 color, float thickness, int times, bool forleft, float scale)
        {
            Action<Box> add = forleft ? (Action<Box>)((item) => box.Add(0, item)) : ((item) => box.Add(item));
            for (int i = 0; i < times; i++)
            {
                add(Get(thickness, box.height, box.depth));
                add(Get(color, thickness, box.height, box.depth, scale));
            }
        }

        public static void PutBorderHorizontal(VerticalBox box, Color32 color, float thickness, int times, bool fortop, float scale)
        {
            Action<Box> add = fortop ? (Action<Box>)((item) => box.Add(0, item)) : ((item) => box.Add(item));
            for (int i = 0; i < times; i++)
            {
                if (i > 0)
                    add(Get(box.width, thickness, 0));
                add(Get(color, box.width, thickness, 0, scale));
            }
        }

        public CharBox ruleChar;

        public override void Flush()
        {
            ObjPool<RuleBox>.Release(this);
            this.ruleChar?.Flush();
            this.ruleChar = null;
            base.Flush();
        }

        public override void Draw(TexRendererState state)
        {
            var r = new Rect(state.x, state.y - depth, width, height + depth);
#if !TEXDRAW_TMP
            // Not applicable yet
            state.Draw(new TexRendererState.QuadState(TexUtility.frontBlockIndex, r, new Rect(), ruleChar.color));
#else
            state.Draw(new TexRendererState.QuadState(TexUtility.frontBlockIndex, r, new Rect(), new Color(1, 1, 0, 0.1f)));
            List<float> xr = ListPool<float>.Get(), yr = ListPool<float>.Get();
            List<float> xuv = ListPool<float>.Get(), yuv = ListPool<float>.Get();

            float coeff = ruleChar.coeff2;
            float vpad = 0.2f * coeff; // vertexPadding
            const float atlasPadding = 0.14f;
            var uvStart = 0;
            var uvEnd = ((coeff) * atlasPadding) / coeff * ruleChar.uv.width;
            var inflate = ruleChar.coeff2;
            xr.Add(r.x - coeff);
            xr.Add(r.x + vpad);
            xr.Add(r.x + vpad);
            xr.Add(r.x + r.width - vpad);
            xr.Add(r.x + r.width - vpad);
            xr.Add(r.x + r.width + coeff);
            yr.Add(r.y - coeff);
            yr.Add(r.y + vpad);
            yr.Add(r.y + vpad);
            yr.Add(r.y + r.height - vpad);
            yr.Add(r.y + r.height - vpad);
            yr.Add(r.y + r.height + coeff);
            xuv.Add(ruleChar.uv.x + uvStart);
            xuv.Add(ruleChar.uv.x + uvEnd);
            xuv.Add(ruleChar.uv.x + uvEnd);
            xuv.Add(ruleChar.uv.x + ruleChar.uv.width - uvEnd);
            xuv.Add(ruleChar.uv.x + ruleChar.uv.width - uvEnd);
            xuv.Add(ruleChar.uv.x + ruleChar.uv.width - uvStart);
            yuv.Add(ruleChar.uv.y + uvStart);
            yuv.Add(ruleChar.uv.y + uvEnd);
            yuv.Add(ruleChar.uv.y + uvEnd);
            yuv.Add(ruleChar.uv.y + ruleChar.uv.height - uvEnd);
            yuv.Add(ruleChar.uv.y + ruleChar.uv.height - uvEnd);
            yuv.Add(ruleChar.uv.y + ruleChar.uv.height - uvStart);
            for (int x = 0; x < 6; x+=2)
            {
                for (int y = 0; y < 6; y+=2)
                {
                    ruleChar.Draw(state, Rect.MinMaxRect(xr[x] + inflate, yr[y] + inflate, xr[x + 1] - inflate, yr[y + 1] - inflate), Rect.MinMaxRect(xuv[x], yuv[y], xuv[x + 1], yuv[y + 1]));
                }
            }

            ListPool<float>.Release(xr);
            ListPool<float>.Release(yr);
            ListPool<float>.Release(xuv);
            ListPool<float>.Release(yuv);
#endif
        }
    }
}
