using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace TexDrawLib
{
    [RequireComponent(typeof(TEXDraw))]
    public class TEXScroll : MonoBehaviour, IDragHandler, IScrollHandler, IPointerClickHandler
    {
        public bool constrainX = true;
        public bool clamp = true;

        public void OnDrag(PointerEventData eventData)
        {
            var tex = GetComponent<TEXDraw>();
            if (tex)
            {
                Drag(eventData.delta / tex.canvas.scaleFactor);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            var tex = GetComponent<TEXDraw>();
            if (tex)
            {
                var speed = -2 * (tex.canvas.pixelRect.size.magnitude / 50f);
                var scroll = eventData.scrollDelta * speed;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    scroll = new Vector2(-scroll.y, 0);
                }
#if UNITY_EDITOR || UNITY_STANDALONE
                scroll.x *= -1;
#endif
                scroll *= 1 / tex.canvas.scaleFactor;
                Drag(scroll);
            }
        }

        public void Drag(Vector2 delta)
        {
            var tex = GetComponent<TEXDraw>();
            if (tex)
            {
                var sc = tex.scrollArea;
                if (constrainX)
                    sc.y += delta.y;
                else
                    sc.position += delta;
                if (clamp)
                {
                    var canvasRect = tex.orchestrator.outputNativeCanvasSize;
                    sc.x = Mathf.Clamp(sc.x, -canvasRect.x + tex.orchestrator.canvasRect.width, 0);
                    sc.y = Mathf.Clamp(sc.y, 0, canvasRect.y - tex.orchestrator.canvasRect.height);
                }
                tex.scrollArea = sc;
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 2)
            {
                StartCoroutine(ResetScrolling());
            }
        }

        IEnumerator ResetScrolling()
        {
            var tex = GetComponent<TEXDraw>();
            var r = tex.scrollArea;
            while (r.position.sqrMagnitude > 0.1f)
            {
                r.position = Vector2.Lerp(r.position, Vector2.zero, 20f * Time.deltaTime);
                tex.scrollArea = r;
                yield return null;
            }
            r.position = Vector2.zero;
            tex.scrollArea = r;
        }
    }
}
