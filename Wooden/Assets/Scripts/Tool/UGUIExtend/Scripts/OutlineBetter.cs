using UnityEngine;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    public class OutlineBetter : ShadowGamma
    {
#if UNITY_EDITOR
        protected override void Reset()
        {
            effectDistance = new Vector2(1, 1);
        }
#endif
        
        private const byte MaxSplit = 16;
        public static byte split { get; private set; }

        public static float[] sinValues;
        public static float[] cosValues;

        public static void SetSplitCount(byte count)
        {
            if (split == count) return;
            split = count;

            if (sinValues == null || cosValues == null)
            {
                sinValues = new float[MaxSplit];
                cosValues = new float[MaxSplit];
            }
            
            float angle = 2f * Mathf.PI / split;
            for (int i = 0; i < split; i++)
            {
                sinValues[i] = Mathf.Cos(angle * i);
                cosValues[i] = Mathf.Sin(angle * i);
            }
        }

        static OutlineBetter() { SetSplitCount(MaxSplit); }
        
        protected OutlineBetter(){}

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            vh.GetUIVertexStream(tmpUIVertexList);

            var neededCpacity = tmpUIVertexList.Count * (split + 1);
            if (tmpUIVertexList.Capacity < neededCpacity)
                tmpUIVertexList.Capacity = neededCpacity;

            Color gammaColor = effectColor.gamma;

            var start = 0;
            var end = tmpUIVertexList.Count;
            for (int i = 0; i < split; i++)
            {
                float offsetX = cosValues[i] * effectDistance.x;
                float offsetY = sinValues[i] * effectDistance.y;
                ApplyShadowZeroAlloc(tmpUIVertexList, gammaColor, start, tmpUIVertexList.Count, offsetX, offsetY);
                start = end;
                end = tmpUIVertexList.Count;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(tmpUIVertexList);
        }
    }
}