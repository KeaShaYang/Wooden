#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[InitializeOnLoad]
public class UseCustomDrawMode
{
    private static bool delegateSceneView = false;

    static UseCustomDrawMode()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (!delegateSceneView && SceneView.sceneViews.Count > 0)
        {
            SceneView.onSceneGUIDelegate += OnScene;
            delegateSceneView = true;
        }

        if (SceneView.sceneViews.Count == 0)
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            delegateSceneView = false;
        }

    }

    private static void OnScene(SceneView sceneview)
    {
        RunDrawMode();
    }

    static bool AcceptedDrawMode(SceneView.CameraMode cameraMode)
    {
        return true;
    }


    static void RunDrawMode()
    {
        if (!CustomDrawModeAssetObject.SetUpObject()) return;

        SceneView.ClearUserDefinedCameraModes();
        for (int i = 0; i < CustomDrawModeAssetObject.cdma.customDrawModes.Length; i++)
        {
            if (
                CustomDrawModeAssetObject.cdma.customDrawModes[i].name != "" &&
                CustomDrawModeAssetObject.cdma.customDrawModes[i].category != ""
            )
                SceneView.AddCameraMode(
                CustomDrawModeAssetObject.cdma.customDrawModes[i].name,
                CustomDrawModeAssetObject.cdma.customDrawModes[i].category);
        }
        ArrayList sceneViewArray = SceneView.sceneViews;
        foreach (SceneView sceneView in sceneViewArray)
        {
            sceneView.onValidateCameraMode -= AcceptedDrawMode;
            sceneView.onValidateCameraMode += AcceptedDrawMode;
        }


        ArrayList sceneViewsArray = SceneView.sceneViews;
        foreach (SceneView sceneView in sceneViewsArray)
        {
            bool useCustom = false;
            for (int i = 0; i < CustomDrawModeAssetObject.cdma.customDrawModes.Length; i++)
            {
                var drawMode = CustomDrawModeAssetObject.cdma.customDrawModes[i];
                if (drawMode.name != "")
                    if (sceneView.cameraMode.name == drawMode.name)
                    {
                        if (drawMode.shader != null)
                        {
                            if (drawMode.replaceRenderType)
                            {
                                sceneView.SetSceneViewShaderReplace(drawMode.shader, "RenderType");
                            }
                            else
                            {
                                sceneView.SetSceneViewShaderReplace(drawMode.shader, "");
                            }
                            useCustom = true;
                        }
                        break;
                    }
            }
            if (!useCustom)
            {
                sceneView.SetSceneViewShaderReplace(null, "");
            }
        }

    }
}
#endif