using System.Collections;
using System.Linq;
using TexDrawLib.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
    [AddComponentMenu("TEXDraw/TEXInput Cursor")]
    public class TEXInputCursor : MaskableGraphic
    {
        public Color activeColor = new Color32(0x00, 0x77, 0xCC, 0x99);
        public Color groupColor = new Color32(0xCC, 0x77, 0x00, 0x99);
        public Color idleColor = new Color32(0x80, 0x80, 0x80, 0x00);
        public float cursorWidth = 2;
        public float cursorBlink = 0;
        public float selectionDilate = 2;

        private Coroutine blinkCoroutine = null;
        private float blinkStartTime = 0;
        private bool isBlinkTime = false;
        private bool isHot = false;
        private bool isActive = false;

        public TEXInput input;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (!input || !input.IsInteractable())
                return;

            if (!(isActive = input.hasFocus) && idleColor.a < 1e-4f)
                return;

            var logger = input.logger;
            var param = input.tex.orchestrator;
            var start = input.selectionStart;
            var length = input.selectionLength;
            var links = param.rendererState.vertexLinks;
            var color = (Color32)(isActive ? activeColor : idleColor);
            var colorG = (Color32)(isActive ? groupColor : idleColor);
            var blocks = logger.GetBlockMatches(start, length);

            if (!isHot && isBlinkTime)
                color = new Color32(0, 0, 0, 0);

            if (links.Count == 0)
            {
                // no target to draw. guess it
                DrawQuad(vh, new Rect(param.rendererState.x - cursorWidth,
                    param.rendererState.y - lineDepth, cursorWidth * 2,
                    lineHeight), color);
            }
            else if (!blocks.Any() && logger.blocks.Count > 0)
            {
                var i = input.selectionStart;

                var b = logger.GetBlockClosest(i);

                if (b.index == -1)
                {
                }
                else if (b.length == 0 && b.start == i)
                {
                    // placeholder
                    DrawQuad(vh, links[b.index].area, color);
                }
                else if (i >= b.end)
                {
                    if (b.group >= 0)
                    {
                        DrawQuad(vh, links[b.group].area, colorG);
                    }
                    Draw(vh, links[b.index].area, true, color);
                }
                else if (i >= b.start)
                {
                    if (b.group >= 0)
                    {
                        DrawQuad(vh, links[b.group].area, colorG);
                    }
                    Draw(vh, links[b.index].area, false, color);
                }
            }
            else
            {
                // just simple block
                foreach (var b in blocks)
                {
                    DrawQuad(vh, Dilate(links[b.index].area), color);
                }
            }
        }

        // private Rect ExtractAreaOfLine(float scale, TexRenderer f)
        // {
        //     return new Rect(f.X * scale, (f.Y * scale - lineDepth), f.Width * scale, lineHeight);
        // }

        private Rect Dilate(Rect r)
        {
            r.height += selectionDilate * 2;
            r.width += selectionDilate * 2;
            r.y -= selectionDilate;
            r.x -= selectionDilate;
            return r;
        }

        private void Draw(VertexHelper verts, Rect r, bool onTheRight, Color32 color)
        {
            DrawQuad(verts, new Rect(r.x - cursorWidth + (onTheRight ? r.width : 0),
                r.y, cursorWidth * 2, Mathf.Max(lineDepth + lineHeight, r.height)), color);
        }

        private float lineHeight => TEXPreference.main.configuration.Typeface.lineAscent * ratio;

        private float lineDepth => TEXPreference.main.configuration.Typeface.lineDescent * ratio;

        private float ratio => 1 / 72.27f * TEXPreference.main.configuration.Document.pixelsPerInch * (input.tex.size / TEXPreference.main.configuration.Document.nativeSize);

        public static void DrawQuad(VertexHelper vertex, Rect v, Color32 c)
        {
            var z = new Vector2();
            var s = vertex.currentVertCount;
            vertex.AddVert(new Vector3(v.xMin, v.yMin), c, z);
            vertex.AddVert(new Vector3(v.xMax, v.yMin), c, z);
            vertex.AddVert(new Vector3(v.xMax, v.yMax), c, z);
            vertex.AddVert(new Vector3(v.xMin, v.yMax), c, z);

            vertex.AddTriangle(s + 0, s + 1, s + 2);
            vertex.AddTriangle(s + 0, s + 2, s + 3);
        }

        public static Rect Union(Rect r1, Rect r2)
        {
            return Rect.MinMaxRect(Mathf.Min(r1.x, r2.x), Mathf.Min(r1.y, r2.y),
                Mathf.Max(r1.xMax, r2.xMax), Mathf.Max(r1.yMax, r2.yMax));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            useLegacyMeshGeneration = false;
            if (cursorBlink > 0)
            {
                blinkCoroutine = StartCoroutine(CaretBlink());
            }
        }

        public bool hotState
        {
            get { return isHot; }
            set
            {
                isHot = value;
                blinkStartTime = Time.unscaledTime;
                isBlinkTime = false;
                if (blinkCoroutine == null)
                    blinkCoroutine = StartCoroutine(CaretBlink());
            }
        }

        IEnumerator CaretBlink()
        {
            // Always ensure caret is initially visible since it can otherwise be confusing for a moment.
            isBlinkTime = false;
            blinkStartTime = Time.unscaledTime;
            yield return null;

            while (input.hasFocus && cursorBlink > 0)
            {
                // the caret should be ON if we are in the first half of the blink period
                bool blinkState = (Time.unscaledTime - blinkStartTime) % cursorBlink > cursorBlink / 2;
                if (isBlinkTime != blinkState)
                {
                    isBlinkTime = blinkState;
                    if (!isHot)
                        SetVerticesDirty();
                }

                // Then wait again.
                yield return null;
            }
            blinkCoroutine = null;
        }
    }
}
