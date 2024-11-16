using UnityEngine;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    [AddComponentMenu("UI/Effects/Extensions/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color m_GradientTop = Color.white;
        [SerializeField]
        private Color m_GradientBottom = Color.black;
        [SerializeField]
        private bool m_SingleChar = false;

        public Color GradientTop { get { return m_GradientTop; } set { m_GradientTop = value; graphic.SetAllDirty(); } }
        public Color GradientBottom { get { return m_GradientBottom; } set { m_GradientBottom = value; graphic.SetAllDirty(); } }

        public override void ModifyMesh(VertexHelper vh)
        {
            int count = vh.currentVertCount;
            if (!IsActive() || count == 0)
            {
                return;
            }

            UIVertex uiVertex = new UIVertex();

            if (!m_SingleChar)
            {
                float bottomY = int.MaxValue;
                float topY = int.MinValue;

                for (int i = 0; i < 2; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (uiVertex.position.y > topY)
                    {
                        topY = uiVertex.position.y;
                    }
                }

                int count1 = vh.currentVertCount;
                if (count1 > 1)
                {
                    for (int i = count1 - 1; i > count1 - 3; i--)
                    {
                        vh.PopulateUIVertex(ref uiVertex, i);
                        if (uiVertex.position.y < bottomY)
                        {
                            bottomY = uiVertex.position.y;
                        }
                    }
                }
                else
                {
                    vh.PopulateUIVertex(ref uiVertex, 0);
                    bottomY = uiVertex.position.y;
                }

                byte alpha = uiVertex.color.a;
                float uiElementHeight = topY - bottomY;
                for (int i = 0; i < count; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    uiVertex.color = Color.Lerp(m_GradientBottom, m_GradientTop, (uiVertex.position.y - bottomY) / uiElementHeight).gamma;
                    uiVertex.color.a = alpha;
                    vh.SetUIVertex(uiVertex, i);
                }

            }
            else
            {
                int charVertCount = 4;
                int charCount = vh.currentVertCount / charVertCount;
                for(int i = 0; i < charCount; i++)
                {

                    float bottomY = int.MaxValue;
                    float topY = int.MinValue;
                    for (int j = 0; j < charVertCount; j++)
                    {
                        vh.PopulateUIVertex(ref uiVertex, i * charVertCount + j);
                        if (uiVertex.position.y > topY)
                        {
                            topY = uiVertex.position.y;
                        }
                        if (uiVertex.position.y < bottomY)
                        {
                            bottomY = uiVertex.position.y;
                        }
                    }
                    float uiElementHeight = topY - bottomY;
                    byte alpha = uiVertex.color.a;
                    for (int j = 0; j < charVertCount; j++)
                    {
                        vh.PopulateUIVertex(ref uiVertex, i * charVertCount + j);
                        uiVertex.color = Color.Lerp(m_GradientBottom, m_GradientTop, (uiVertex.position.y - bottomY) / uiElementHeight).gamma;
                        uiVertex.color.a = alpha;
                        vh.SetUIVertex(uiVertex, i * charVertCount + j);
                    }
                }

            }


        }
    }
}