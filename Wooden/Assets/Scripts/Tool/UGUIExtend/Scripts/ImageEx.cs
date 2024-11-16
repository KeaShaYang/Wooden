using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    public class ImageEx : Image
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
                if (overrideSprite != null)
                    return overrideSprite.texture;
                if (material != null && material.mainTexture != null)
                    return material.mainTexture;
                return emptyTexture;
            }
        }

        public override Color color
        {
            get { return base.color.gamma; }
            set { base.color = value; }
        }
    }
}