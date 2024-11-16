using System;
using UnityEngine;

public class UITexSetter : Singleton<UITexSetter>
{
    public Action<Texture> setCallback;

    public Texture SetTexture( string texName)
    {
        if (string.IsNullOrEmpty(texName))
        {
            return null;
        }
        int index = texName.IndexOf('_');
        string folderName;
        if (index > 0) folderName = texName.Substring(0, index);
        else folderName = texName;
        string texPath = string.Format("Texture/{0}/{1}", folderName, texName);
        Texture texture = Resources.Load(texPath) as Texture;
        if (null == texture)
            Debug.LogError("sprite¼ÓÔØÊ§°Ü :" + texPath);
        return texture;
    }
    public Sprite SetSprite(string texName)
    {
        if (string.IsNullOrEmpty(texName))
        {
            return null;
        }

        int index = texName.IndexOf('_');
        string folderName;
        if (index > 0) folderName = texName.Substring(0, index);
        else folderName = texName;
        string texPath = string.Format("Atlas/{0}/{1}", folderName, texName);
        object[] sprites = Resources.LoadAll(string.Format("Atlas/{0}",folderName));
        for (int i = 0; i < sprites.Length; i++)
        {

            Texture2D tex = sprites[i] as Texture2D;
            if (tex == null)
            {
                Sprite sp = sprites[i] as Sprite;
                if (sp.name.CompareTo(texName) == 0)
                {
                    return sp;
                }
            }
          
        }
        Debug.LogError("sprite¼ÓÔØÊ§°Ü :" + texPath);
        return null;
    }
}
