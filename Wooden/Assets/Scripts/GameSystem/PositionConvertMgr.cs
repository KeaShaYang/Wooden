using UnityEngine;
public class PositionConvertMgr
{
    public static Vector2 UIPointToScreenPoint(Vector3 worldPoint,Camera uiCam)
    {
        if (null != uiCam)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCam,worldPoint);

        }
        return worldPoint;
    }
    public static Vector2 ScreenToWorld(Vector2 screenPoint, Camera main)
    {
        Vector3 pos = new Vector3(screenPoint.x, screenPoint.y, 0);
        Vector3 worldPoint = main.ScreenToWorldPoint(pos);
        return worldPoint;
    }
}
