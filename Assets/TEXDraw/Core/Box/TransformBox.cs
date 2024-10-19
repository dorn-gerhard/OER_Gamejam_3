using UnityEngine;
using System.Collections.Generic;

namespace TexDrawLib.Core
{
    public class TransformBox : Box
    {
        public float rotation = 0;
        public float italic = 0;
        public Vector2 rotationOffset = Vector2.zero;
        public Vector2 scale = Vector2.one;
        public Box box;
        public List<int> counts = new List<int>();

        public static TransformBox Get(Box baseBox)
        {
            var box = ObjPool<TransformBox>.Get();
            box.box = baseBox;

            box.Set(baseBox.width, baseBox.height, baseBox.depth, 0);
            return box;
        }

        public void CalcAlignment(TexAlignment alignment)
        {
            switch (alignment & (TexAlignment.Left | TexAlignment.Right))
            {
                case TexAlignment.Left:
                    rotationOffset.x = 0;
                    break;
                case TexAlignment.Center:
                    rotationOffset.x = box.width / 2;
                    break;
                case TexAlignment.Right:
                    rotationOffset.x = box.width;
                    break;
            }
            switch (alignment & (TexAlignment.Top | TexAlignment.Bottom | TexAlignment.Baseline))
            {
                case TexAlignment.Baseline:
                    rotationOffset.y = 0;
                    break;
                case TexAlignment.Top:
                    rotationOffset.y = box.height;
                    break;
                case TexAlignment.Bottom:
                    rotationOffset.y = -box.depth;
                    break;
                case TexAlignment.Center:
                    rotationOffset.y = box.TotalHeight / 2;
                    break;
            }
        }

        public void ApplyTransform()
        {
            float w = box.width, h = box.height, d = box.depth, i = 0;
            float rox = rotationOffset.x, roy = rotationOffset.y;
            if (rotation != 0)
            {
                // rotation matrices
                float acos = Mathf.Cos(rotation), asin = Mathf.Sin(rotation);
                // corner points
                float x1 = 0 - rox, x2 = w - rox, y1 = -d - roy, y2 = h - roy;
                /* 
                 * ----------- X -----------         ----------- Y -----------
                 * __     __                          __     __
                 * | x1 y1 | __      __               | x1 y1 | __      __
                 * | x1 y2 | |  cos θ |               | x1 y2 | |  sin θ |
                 * | x2 y1 | | -sin θ |               | x2 y1 | |  cos θ |
                 * | x2 y2 | ^^      ^^               | x2 y2 | ^^      ^^
                 * ^^     ^^                          ^^     ^^
                 */
                float X1 = acos * x1 - asin * y1, X2 = acos * x1 - asin * y2;
                float X3 = acos * x2 - asin * y1, X4 = acos * x2 - asin * y2;
                float Y1 = asin * x1 + acos * y1, Y2 = asin * x1 + acos * y2;
                float Y3 = asin * x2 + acos * y1, Y4 = asin * x2 + acos * y2;
                w = rox + Mathf.Max(Mathf.Max(X1, X2), Mathf.Max(X3, X4));
                i = -rox - Mathf.Min(Mathf.Min(X1, X2), Mathf.Min(X3, X4));
                h = roy + Mathf.Max(Mathf.Max(Y1, Y2), Mathf.Max(Y3, Y4));
                d = roy - Mathf.Min(Mathf.Min(Y1, Y2), Mathf.Min(Y3, Y4));
            }
            Set((w + i) * Mathf.Abs(scale.x), h * Mathf.Abs(scale.y), d * Mathf.Abs(scale.y), 0);
            italic = i;
        }

        public override void Draw(TexRendererState state)
        {
            counts.Clear();
            for (int i = 0; i < state.vertexes.Count; i++)
            {
                counts.Add(state.vertexes[i].m_Positions.Count);
            }
            box.Draw(state);
            // count the diff
            for (int i = 0; i < state.vertexes.Count; i++)
            {
                if (i >= counts.Count)
                    counts.Add(state.vertexes[i].m_Positions.Count);
                else
                    counts[i] = state.vertexes[i].m_Positions.Count - counts[i];
            }

            // apply transform
            Debug.Assert(state.vertexes.Count == counts.Count);
            float acos = Mathf.Cos(rotation), asin = Mathf.Sin(rotation);
            float rox = rotationOffset.x, roy = rotationOffset.y;
            for (int i = 0; i < state.vertexes.Count; i++)
            {
           
                var count = state.vertexes[i].m_Positions.Count;
                for (int k = count - counts[i]; k < count; k++)
                {
                    var p = state.vertexes[i].m_Positions[k];
                    var px = p.x - state.x - rox;
                    var py = p.y - state.y - roy;
                    p.x = acos * px - asin * py + rox;
                    p.y = asin * px + acos * py + roy;
                    p.x = p.x * scale.x + state.x;
                    p.y = p.y * scale.y + state.y;
                    if (scale.x < 0)
                        p.x += box.width * -scale.x;
                    p.x += italic;
                    state.vertexes[i].m_Positions[k] = p;
                }
            }
        }

        public override void Flush()
        {
            ObjPool<TransformBox>.Release(this);
            counts.Clear();
            rotation = 0;
            rotationOffset = Vector2.zero;
            scale = Vector2.one;
            if (box != null)
            {
                box.Flush();
                box = null;
            }
        }
    }
}
