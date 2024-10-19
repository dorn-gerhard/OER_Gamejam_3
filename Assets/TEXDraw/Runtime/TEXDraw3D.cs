#if TEXDRAW_DEBUG && (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define TEXDRAW_PROFILE
#endif

using System;
using System.Collections.Generic;
using TexDrawLib.Core;
using UnityEngine;

namespace TexDrawLib
{
    [AddComponentMenu("TEXDraw/TEXDraw 3D", 2), ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [HelpURL("https://willnode.gitlab.io/texdraw/")]
#if TEXDRAW_DEBUG
    [SelectionBase]
#endif
    public class TEXDraw3D : MonoBehaviour, ITEXDraw
    {

        public TEXPreference preference { get { return TEXPreference.main; } }

        [NonSerialized]
        private List<TEXDraw3DRenderer> m_Renderers = new List<TEXDraw3DRenderer>();

        [NonSerialized]
        private bool m_TextDirty = true;

        [NonSerialized]
        private bool m_BoxDirty = true;

        [NonSerialized]
        private bool m_OutputDirty = true;

        [SerializeField, TextArea(5, 10)]
        private string m_Text = "$$TEXDraw$$";

        [SerializeField]
        private float m_Size = 36f;

        [SerializeField]
        private float m_PixelsPerUnit = 1f;

        [SerializeField]
        private Color m_Color = Color.white;

        [SerializeField]
        private Vector2 m_Alignment = Vector2.one * 0.5f;

        [SerializeField]
        private Rect m_ScrollArea = new Rect();

        [SerializeField]
        private TexRectOffset m_Padding = new TexRectOffset(2, 2, 2, 2);

        [SerializeField]
        private Overflow m_Overflow = Overflow.Hidden;

        [SerializeField]
        private Material m_Material;

        public virtual string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (m_Text != value)
                {
                    m_Text = value;
                    m_TextDirty = true;
                }
            }
        }

        public virtual float size
        {
            get
            {
                return m_Size;
            }
            set
            {
                if (m_Size != value)
                {
                    m_Size = Mathf.Max(value, 0f);
                    m_TextDirty = true;
                }
            }
        }

        public virtual Color color
        {
            get
            {
                return m_Color;
            }
            set
            {
                if (m_Color != value)
                {
                    m_Color = value;
                    m_TextDirty = true;
                }
            }
        }

        public virtual Material material
        {
            get
            {
                return m_Material;
            }
            set
            {
                if (m_Material != value)
                {
                    m_Material = value;
                    m_OutputDirty = true;
                }
            }
        }

        public virtual Vector2 alignment
        {
            get => m_Alignment;
            set
            {
                if (m_Alignment != value)
                {
                    m_Alignment = value;
                    m_TextDirty = true;
                }
            }
        }

        public virtual TexRectOffset padding
        {
            get => m_Padding;
            set
            {
                m_Padding = value;
                m_BoxDirty = true;
            }
        }

        public virtual Rect scrollArea
        {
            get => m_ScrollArea;
            set
            {
                if (m_ScrollArea != value)
                {
                    if (m_ScrollArea.size != value.size)
                        m_BoxDirty = true;
                    else
                    {
                        // need to inject manually cause scroll position is not injected on render
                        var rect = new Rect(Vector2.zero, rectTransform.rect.size);
                        rect.center = Vector2.zero;
                        orchestrator.InputCanvasSize(rectTransform.rect, value, m_Padding, new Vector2Int(), m_Overflow);
                    }
                    m_ScrollArea = value;
                    m_OutputDirty = true;
                }
            }
        }

        public virtual Overflow overflow
        {
            get => m_Overflow;
            set
            {
                if (m_Overflow != value)
                {
                    m_Overflow = value;
                    m_BoxDirty = true;
                }
            }
        }

        public virtual float pixelsPerUnit
        {
            get
            {
                return m_PixelsPerUnit;
            }
            set
            {
                if (m_PixelsPerUnit != value)
                {
                    m_PixelsPerUnit = Mathf.Max(value, 0f);
                    m_TextDirty = true;
                }
            }
        }

        Rect _internalRect
        {
            get
            {
                var sz = rectTransform.rect.size;
                return new Rect(sz * -0.5f, sz);
            }
        }

        public List<TEXDraw3DRenderer> renderers => m_Renderers;

#if UNITY_EDITOR

        [ContextMenu("Open Preference")]
        private void OpenPreference()
        {
            UnityEditor.Selection.activeObject = preference;
        }

        [ContextMenu("Trace Output")]
        private void TraceOutput()
        {
            UnityEditor.EditorGUIUtility.systemCopyBuffer = orchestrator.Trace();
            Debug.Log("The trace output has been copied to clipboard.");
        }

        public void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                SetTextDirty(true);
            };
        }

#endif

        #region Engine

        private void OnEnable()
        {
            if (!preference)
            {
                TEXPreference.Initialize();
            }

            orchestrator = new TexOrchestrator();
            GetComponentsInChildren<TEXDraw3DRenderer>(true, m_Renderers);

            Font.textureRebuilt += OnFontRebuild;
            SetTextDirty(true);
        }

        public void SetTextDirty()
        {
            m_TextDirty = true;
        }

        public void SetTextDirty(bool now)
        {
            m_TextDirty = true;
            if (now)
            {
                Redraw();
            }
        }

        private void LateUpdate()
        {
            if (transform.hasChanged)
            {
                m_BoxDirty = true;
                transform.hasChanged = false;
            }
            if (m_OutputDirty || m_BoxDirty || m_TextDirty)
            {
                Redraw();
            }
        }

        private bool _onRendering = false;

        private TexOrchestrator m_orchestrator;

        public TexOrchestrator orchestrator { get => m_orchestrator; private set => m_orchestrator = value; }

        public void Redraw()
        {
            // Multi-threading issue with OnFontRebuilt
            if (_onRendering)
                return;

            _onRendering = true;

            if (orchestrator == null)
            {
                OnEnable();
                return;
            }
            try
            {
#if UNITY_EDITOR
                if (preference.editorReloading)
                {
                    return;
                }
#endif

                if (m_TextDirty)
                {
                    orchestrator.initialColor = color;
                    orchestrator.initialSize = size;
                    orchestrator.pixelsPerInch = TEXConfiguration.main.Document.pixelsPerInch;
                    orchestrator.alignment = m_Alignment;

                    orchestrator.ResetParser();
                    orchestrator.parserState.Document.retinaRatio = Mathf.Max(orchestrator.parserState.Document.retinaRatio, m_PixelsPerUnit);
                    orchestrator.latestAtomCache = orchestrator.parser.Parse(m_Text, orchestrator.parserState);
                    m_TextDirty = false;
                    m_BoxDirty = true;
                }
                if (m_BoxDirty)
                {
                    orchestrator.InputCanvasSize(_internalRect, m_ScrollArea, m_Padding, Vector2Int.zero, m_Overflow);
                    orchestrator.Box();
                    m_BoxDirty = false;
                    m_OutputDirty = true;
                }
                if (m_OutputDirty)
                {
                    orchestrator.Render();
                    Repaint();
                    m_OutputDirty = false;
                }
            }
            finally
            {
                _onRendering = false;
            }
        }

        public void Repaint()
        {
            if (orchestrator == null)
            {
                Redraw();
                return;
            }

            var vertexes = orchestrator.rendererState.vertexes;
            for (int i = vertexes.Count; i < m_Renderers.Count; i++)
            {
                if (m_Renderers[i])
                {
                    m_Renderers[i].FontMode = -1;
                    m_Renderers[i].ForceRender();
                }
            }
            for (int i = 0; i < vertexes.Count; i++)
            {
                if (i >= m_Renderers.Count)
                    m_Renderers.Add(CreateNewRenderer());
                m_Renderers[i].FontMode = vertexes[i].m_Font;
                m_Renderers[i].Redraw();
                m_Renderers[i].Repaint();
            }
        }

        private RectTransform _cRT;

        public RectTransform rectTransform => _cRT ? _cRT : (_cRT = GetComponent<RectTransform>());


        private TEXDraw3DRenderer CreateNewRenderer()
        {
            var g = new GameObject("TEXDraw 3D Renderer");
#if TEXDRAW_DEBUG
            g.hideFlags = HideFlags.DontSaveInEditor;
#else
        g.hideFlags = HideFlags.HideAndDontSave;
#endif
            var r = g.AddComponent<RectTransform>();
            r.SetParent(transform, false);
            r.anchorMax = Vector2.one;
            r.anchorMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
            r.offsetMin = Vector2.zero;
            var rr = g.AddComponent<TEXDraw3DRenderer>();
            rr.m_TEXDraw = this;
            return rr;
        }


        private void OnDisable()
        {
            Font.textureRebuilt -= OnFontRebuild;
            foreach (var item in m_Renderers)
                if (item)
                {
                    item.FontMode = -1;
                    item.Repaint();
                }
        }

        private void OnFontRebuild(Font f)
        {
            m_TextDirty = true;
        }

        #endregion

    }
}
