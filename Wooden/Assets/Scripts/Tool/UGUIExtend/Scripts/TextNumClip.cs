using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文字个数裁剪效果
/// </summary>
namespace Tool.UGUIExtend.Scripts
{
    [AddComponentMenu("UI/Effects/Extensions/TextNumClip")]
    public class TextNumClip : BaseMeshEffect
    {
        [SerializeField]
        private bool m_IsClip;
        [SerializeField]
        private int m_ShowNum;

        /// <summary>
        /// 有多少字，排除未渲染的
        /// </summary>
        int WordCount;
        /// <summary>
        /// 所有的顶点
        /// </summary>
        static List<UIVertex> vertexList = new List<UIVertex>();
        /// <summary>
        /// 真正渲染的顶点
        /// </summary>
        static List<UIVertex> WordList = new List<UIVertex>();


        public override void ModifyMesh(VertexHelper vh)
        {
            if(!m_IsClip)
            {
                return;
            }
            if (!IsActive() || vh.currentVertCount<4 || vh.currentVertCount%4!=0)
            {
                return;
            }
            vertexList.Clear();
            for (int i =0;i< vh.currentVertCount;i++)
            {
                UIVertex uiVertex = new UIVertex();
                vh.PopulateUIVertex(ref uiVertex, i);
                vertexList.Add(uiVertex);
            }
            //真正渲染的字列表
            WordList.Clear();
            for(int i=0;i<vertexList.Count;i=i+4)//每四个点为一个字
            {
                if(vertexList[i].position == vertexList[i+1].position && vertexList[i+1].position == vertexList[i + 2].position&& vertexList[i+2].position == vertexList[i + 3].position)
                {
                    //如果连续四个点的顶点坐标一样，侧说明该字没有被渲染，排除
                }
                else
                {
                    WordList.Add(vertexList[i]);
                    WordList.Add(vertexList[i+1]);
                    WordList.Add(vertexList[i+2]);
                    WordList.Add(vertexList[i+3]);
                }
            }
            WordCount = WordList.Count / 4;
            int count = m_ShowNum;
            if (count > WordCount)
            {
                count = WordCount;
            }
            if(count >= 0)
            {
                UIVertex uiVertex = new UIVertex();
                //从真正要渲染的字里拿顶点
                for (int i = 0; i < count * 4; i++)
                {
                    //vh.PopulateUIVertex(ref uiVertex, i);
                    vh.SetUIVertex(WordList[i], i);
                }
                //其它的变成空
                for (int i = count * 4; i < vh.currentVertCount; i++)
                {
                    vh.SetUIVertex(uiVertex, i);
                }
            }
        }

        public int GetWordCount()
        {
            return WordCount;
        }

        public void SetIsClip(bool isClip)
        {
            if(m_IsClip!= isClip)
            {
                m_IsClip = isClip;
                graphic.SetAllDirty();
            }
        }

        public void SetClipCount(int count)
        {
            if(m_ShowNum!=count)
            {
                m_ShowNum = count;
                graphic.SetAllDirty();
            }
        }
    }
}
