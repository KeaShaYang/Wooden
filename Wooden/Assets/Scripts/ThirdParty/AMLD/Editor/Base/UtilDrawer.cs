/*
作者：黄云龙
说明：绘制通用函数
日期：2021-04-23
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class UtilDrawer
{
    public static void DrawBox(Vector3 center, Quaternion rotation, Vector3 scale, Color color)
    {
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetTRS(center, rotation, scale);
        DrawBox(matrix, color);
    }

    public static void DrawBox(Matrix4x4 matrix, Color color)
    {
        Matrix4x4 oldMatrix = Handles.matrix;
        Handles.matrix = matrix;
        Color oldColor = Handles.color;
        Handles.color = color;
        Handles.DrawWireCube(Vector3.zero, Vector3.one);
        Handles.color = oldColor;
        Handles.matrix = oldMatrix;
    }

}
