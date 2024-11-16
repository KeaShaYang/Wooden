using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    /// <summary>
    /// 默认图片为纯透明图片的RawImage组件
    /// </summary>
    public class RawImageEx : RawImage
    {
        static Texture2D emptyTexture;

        protected override void Awake()
        {
            base.Awake();
            if (emptyTexture == null) emptyTexture = Texture2D.blackTexture;
        }
        
        public override Texture mainTexture
        {
            get
            {
                if (texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return emptyTexture;
                }

                return texture;
            }
        }

        public override Color color
        {
            get { return base.color.gamma; }
            set { base.color = value; }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Texture tex = mainTexture;
            if (tex != null && tex != emptyTexture)
                base.OnPopulateMesh(vh);
            else
                vh.Clear();
        }
    }
}