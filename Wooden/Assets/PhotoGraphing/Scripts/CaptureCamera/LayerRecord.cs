using System.Collections.Generic;
using UnityEngine;

namespace BounderHelper
{
    public class LayerRecord : MonoBehaviour
    {
        //private static LayerMask[] allLayer;
        private static List<int> allLayer=new List<int>();
        private static void RecordLayer(Transform objT)
        {
            allLayer.Add(objT.gameObject.layer);
        }
        public static void ChangeAllLayer(Transform objT, int layer)
        {
            Transform[] trans = objT.GetComponentsInChildren<Transform>();
            for (int i = 0; i < trans.Length; i++)
            {
                RecordLayer(trans[i]);
                trans[i].gameObject.layer = layer;
            }
        }
        public static void RecoverAllLayer(Transform objT)
        {
            Transform[] trans = objT.GetComponentsInChildren<Transform>();
            for (int i = 0; i < trans.Length; i++)
            {
                trans[i].gameObject.layer = allLayer[i];
            }
            allLayer.Clear();
        }
    }
}
