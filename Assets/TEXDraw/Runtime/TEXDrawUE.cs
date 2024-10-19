#if TEXDRAW_UIE
using System;
using UnityEngine.UIElements;
using TexDrawLib;
using UnityEngine;
using Unity.Collections;
using System.Reflection;

namespace TexDrawLib
{
    public class TEXDrawUE : VisualElement, ITEXDraw
    {
        public new class UxmlFactory : UxmlFactory<TEXDrawUE, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            // The text property is exposed to UXML.
            UxmlStringAttributeDescription m_TextAttribute = new UxmlStringAttributeDescription()
            {
                name = "text",
                defaultValue = "TEXDraw",
            };

            // The Init method is used to assign to the C# progress property from the value of the progress UXML
            // attribute.
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                (ve as TEXDrawUE).text = m_TextAttribute.GetValueFromBag(bag, cc);
            }
        }

        private const string ussClassName = "texdraw";

        public TEXDrawUE() : this("TEXDraw")
        {
        }

        System.Func<MeshGenerationContext, int, int, Texture, Material, int, MeshWriteData> allocateFun;

        public TEXDrawUE(string text)
        {
            AddToClassList(ussClassName);
            // Register a callback to generate the visual content of the control.
            generateVisualContent += context => GenerateVisualContent(context);

            m_orchestrator = new TexOrchestrator();
            m_Text = text;

            // This special call is behind internal gate... (Facepalm)
            MethodInfo allocateHandler = typeof(MeshGenerationContext).GetMethod("Allocate",
            BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(allocateHandler != null);
            // internal MeshWriteData Allocate(int vertexCount, int indexCount, Texture texture, Material material, MeshFlags flags)
            allocateFun = (System.Func<MeshGenerationContext, int, int, Texture, Material, int, MeshWriteData>)
                allocateHandler.CreateDelegate(typeof(System.Func<MeshGenerationContext, int, int, Texture, Material, int, MeshWriteData>));
        }



        public TEXPreference preference { get { return TEXPreference.main; } }

        private string m_Text;
        private Rect m_ScrollArea = new Rect();
        private TexRectOffset m_Padding = new TexRectOffset(2, 2, 2, 2);
        private TexDrawLib.Overflow m_Overflow = TexDrawLib.Overflow.Hidden;
        private TexOrchestrator m_orchestrator;


        /// <summary>
        /// A text string
        /// </summary>
        public string text
        {
            // The progress property is exposed in C#.
            get => m_Text;
            set
            {
                // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                // generateVisualContents callback.
                m_Text = value;
                MarkDirtyRepaint();
            }
        }

        public float size { get => ((IResolvedStyle)this).fontSize; set { throw new NotImplementedException(); } } 
        public Color color { get => ((IResolvedStyle)this).color; set { throw new NotImplementedException(); } } 
        public Vector2 alignment { get => translateAlignment(((IResolvedStyle)this).unityTextAlign); set { throw new NotImplementedException(); } }
        public Rect scrollArea { get => m_ScrollArea; set { if (m_ScrollArea != value) { m_ScrollArea = value; MarkDirtyRepaint(); } } }
        public TexRectOffset padding { get => m_Padding; set { m_Padding = value; MarkDirtyRepaint(); } }
        public TexDrawLib.Overflow overflow { get => m_Overflow; set { if (m_Overflow != value) { m_Overflow = value; MarkDirtyRepaint(); } } }

        public TexOrchestrator orchestrator => m_orchestrator;

        static void GenerateVisualContent(MeshGenerationContext context)
        {
            TEXDrawUE element = (TEXDrawUE)context.visualElement;
            element.DrawMeshes(context);
        }

        Vector2 translateAlignment(TextAnchor anchor)
        {
            switch(anchor)
            {
                case TextAnchor.UpperLeft: return new Vector2(0, 1);
                case TextAnchor.UpperCenter: return new Vector2(0.5f, 1);
                case TextAnchor.UpperRight: return new Vector2(1, 1);
                case TextAnchor.MiddleLeft: return new Vector2(0, 0.5f);
                case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight: return new Vector2(1, 0.5f);
                case TextAnchor.LowerLeft: return new Vector2(0, 0);
                case TextAnchor.LowerCenter: return new Vector2(0.5f, 0);
                case TextAnchor.LowerRight: return new Vector2(1, 0);

            }
            return new Vector2(0, 0);
        }

        string manipulateText(string text, FontStyle style)
        {
            switch(style)
            {
                case FontStyle.Normal:
                    break;
                case FontStyle.Bold:
                    text = "\\bf " + text;
                    break;
                case FontStyle.Italic:
                    text = "\\sl " + text;
                    break;
                case FontStyle.BoldAndItalic:
                    text = "\\bf\\slshape " + text;
                    break;
            }
            return text;
        }

        void DrawMeshes(MeshGenerationContext context)
        {
    #if UNITY_EDITOR
            if (preference.editorReloading)
            {
                return;
            }
    #endif
            var style = ((IResolvedStyle)this);

            orchestrator.initialColor = style.color;
            orchestrator.initialSize = style.fontSize;
            orchestrator.pixelsPerInch = TEXConfiguration.main.Document.pixelsPerInch;
            orchestrator.alignment = translateAlignment(style.unityTextAlign);

            orchestrator.ResetParser();
            orchestrator.parserState.Document.retinaRatio = Mathf.Max(orchestrator.parserState.Document.retinaRatio, 1);
            orchestrator.latestAtomCache = orchestrator.parser.Parse(m_Text, orchestrator.parserState);
            var r = contentRect;
            orchestrator.InputCanvasSize(r, new Rect(), new TexRectOffset(), Vector2Int.zero, m_Overflow);
            orchestrator.Parse(manipulateText(text, style.unityFontStyleAndWeight));
            orchestrator.Box();
            orchestrator.Render();
     

            var vertexes = orchestrator.rendererState.vertexes;
            var mat = preference.GetDefaultMaterialForUIE();

            for (int i = 0; i < vertexes.Count; i++)
            {
                var v = vertexes[i];
                var f = preference.fonts[v.m_Font];
            
                var w = allocateFun(context, v.m_Positions.Count, v.m_Indicies.Count, f.Texture(), mat, 0);
                for (int j = 0; j < v.m_Positions.Count; j++)
                {
                    var x = v.m_Positions[j].x;
                    var y = Mathf.Lerp(r.yMax, r.yMin, Mathf.InverseLerp(r.yMin, r.yMax, v.m_Positions[j].y));
                    w.SetNextVertex(new Vertex()
                    {
                        position = new Vector3(x, y, Vertex.nearZ),
                        uv = v.m_Uv0S[j],
                        tint = v.m_Colors[j],
                    });
                }
                for (int j = 0; j < v.m_Indicies.Count; j++)
                {
                    w.SetNextIndex((ushort)v.m_Indicies[j]);
                }
            }
        }

        public void SetTextDirty()
        {
            MarkDirtyRepaint();
        }
    }
}

#endif