using UnityEngine;

namespace BounderHelper
{
    public class CalculateBounder
    {

        /// <summary>
        /// 计算物体的边界，主要计算中心与大小
        /// </summary>
        /// <param name="obj">需要计算的物体</param>
        /// <returns></returns>
        public static Bounds ComputeObjBounds(Transform obj)
        {
            Renderer[] rens = obj.GetComponentsInChildren<Renderer>();

            int length = rens.Length;

            if (length > 0)
            {
                Vector3 minPoint = rens[0].bounds.min;
                Vector3 maxPoint = rens[0].bounds.max;
                for (int i = 1; i < length; i++)
                {
                    if (minPoint.x > rens[i].bounds.min.x)
                        minPoint.x = rens[i].bounds.min.x;
                    if (minPoint.y > rens[i].bounds.min.y)
                        minPoint.y = rens[i].bounds.min.y;
                    if (minPoint.z > rens[i].bounds.min.z)
                        minPoint.z = rens[i].bounds.min.z;

                    if (maxPoint.x < rens[i].bounds.max.x)
                        maxPoint.x = rens[i].bounds.max.x;
                    if (maxPoint.y < rens[i].bounds.max.y)
                        maxPoint.y = rens[i].bounds.max.y;
                    if (maxPoint.z < rens[i].bounds.max.z)
                        maxPoint.z = rens[i].bounds.max.z;
                }

                Bounds bound = new Bounds()
                {
                    center = new Vector3(minPoint.x + (maxPoint.x - minPoint.x) / 2f, minPoint.y + (maxPoint.y - minPoint.y) / 2f, minPoint.z + (maxPoint.z - minPoint.z) / 2f),
                    size = new Vector3(maxPoint.x - minPoint.x, maxPoint.y - minPoint.y, maxPoint.z - minPoint.z)
                };

                return bound;
            }

            return new Bounds();
        }
    }

    /// <summary>
    /// 物体的边界
    /// </summary>
    public struct Bounds
    {
        public Vector3 center;
        public Vector3 size;
    }
}
