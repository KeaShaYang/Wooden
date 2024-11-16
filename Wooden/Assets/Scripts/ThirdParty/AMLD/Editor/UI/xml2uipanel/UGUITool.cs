using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UGUITool  
{
    private const string kStandardSpritePath       = "UI/Skin/UISprite.psd";
    private const string kBackgroundSpritePath     = "UI/Skin/Background.psd";
    private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
    private const string kKnobPath                 = "UI/Skin/Knob.psd";
    private const string kCheckmarkPath            = "UI/Skin/Checkmark.psd";
    private const string kDropdownArrowPath        = "UI/Skin/DropdownArrow.psd";
    private const string kMaskPath                 = "UI/Skin/UIMask.psd";

    static DefaultControls.Resources s_StandardResources;

    static DefaultControls.Resources GetStandardResources()
    {
        if (s_StandardResources.standard == null)
        {
            s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
            s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
            s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
            s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
            s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
            s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
        }
        return s_StandardResources;
    }

    [MenuItem("AMLD/UI工具/生成Slider")]
    static void CreateSlider()
    {
        Object[] objs = Selection.objects;
        if (objs.Length < 2) return;
        GameObject background = objs[0] as GameObject;
        if (!background) return;
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        
        GameObject root = new GameObject("Slier", typeof(RectTransform));
        Slider slider = root.AddComponent<Slider>();
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.parent = backgroundRect.parent;
        rootRect.localPosition = backgroundRect.localPosition;
        rootRect.SetSiblingIndex(backgroundRect.GetSiblingIndex());
        float width = backgroundRect.rect.width;
        float height = GetMaxHeight(objs);
        rootRect.sizeDelta = new Vector2(width, height);

        float f = backgroundRect.rect.height / rootRect.rect.height;
        backgroundRect.parent = rootRect;
        backgroundRect.anchorMin = new Vector2(0, (1 - f) * 0.5f);
        backgroundRect.anchorMax = new Vector2(1, (1 + f) * 0.5f);
        backgroundRect.sizeDelta = new Vector2(0, 0);
        
        GameObject fillArea = CreateUIObject("Fill Area", root);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        GameObject fill = objs[1] as GameObject;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        f = fillRect.rect.height / rootRect.rect.height;
        fillAreaRect.anchorMin = new Vector2(0, (1 - f) * 0.5f);
        fillAreaRect.anchorMax = new Vector2(1, (1 + f) * 0.5f);
        fillAreaRect.anchoredPosition = new Vector2(0, 0);
        f = rootRect.rect.width - fillRect.rect.width;
        fillAreaRect.sizeDelta = new Vector2(-f, 0);
        
        fillRect.parent = fillAreaRect;
        fillRect.sizeDelta = new Vector2(0, 0);
        
        slider.fillRect = fill.GetComponent<RectTransform>();
        
        if (objs.Length > 2)
        {
            GameObject handleArea = CreateUIObject("Handle Slide Area", root);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            GameObject handle = objs[2] as GameObject;
            RectTransform handleRect = handle.GetComponent<RectTransform>();

            f = handleRect.rect.height / rootRect.rect.height;
            handleAreaRect.anchorMin = new Vector2(0, (1 - f) * 0.5f);
            handleAreaRect.anchorMax = new Vector2(1, (1 + f) * 0.5f);
            handleAreaRect.sizeDelta = fillAreaRect.sizeDelta;
            
            handleRect.parent = handleAreaRect;
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(handleRect.rect.width, 0);
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handle.GetComponent<Image>();
        }
        slider.direction = Slider.Direction.LeftToRight;
    }

    static float GetMaxHeight(Object[] objs)
    {
        int length = objs.Length;
        float result = 0;
        for (int i = 0; i < length; i++)
        {
            GameObject obj = objs[i] as GameObject;
            if (obj)
            {
                RectTransform rectTran = obj.GetComponent<RectTransform>();
                if (rectTran)
                {
                    float height = rectTran.rect.height;
                    if (height > result) result = height;
                }
            }
        }

        return result;
    }
    
    static GameObject CreateUIObject(string name, GameObject parent)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<RectTransform>();
        SetParentAndAlign(go, parent);
        return go;
    }
    
    static void SetParentAndAlign(GameObject child, GameObject parent)
    {
        if (parent == null)
            return;

        child.transform.SetParent(parent.transform, false);
        SetLayerRecursively(child, parent.layer);
    }
    
    static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursively(t.GetChild(i).gameObject, layer);
    }

    static Camera GetOrCreateUICamera()
    {
        Camera cam = Object.FindObjectOfType<Camera>();
        if (!cam)
        {
            GameObject obj = new GameObject("UICamera", typeof(Camera));
            cam = obj.GetComponent<Camera>();
        }

        cam.cullingMask = 1 << LayerMask.NameToLayer("UI");
        cam.useOcclusionCulling = false;
        cam.orthographic = true;
        cam.allowHDR = false;
        cam.allowMSAA = false;
        cam.clearFlags = CameraClearFlags.Depth;
            
        return cam;
    }

    public static Text CreateText(Transform parent)
    {
        GameObject go = DefaultControls.CreateText(GetStandardResources());
        go.transform.SetParent(parent);
        return go.GetComponent<Text>();
    }
    public static TextGamma CreateTextGamma(Transform parent)
    {
        GameObject go = DefaultControls.CreateText(GetStandardResources());
        go.transform.SetParent(parent);
        Text text = go.GetComponent<Text>();
        Object.DestroyImmediate(text);
        return go.AddComponent<TextGamma>();
    }
    public static Image CreateImage(Transform parent)
    {
        GameObject go = DefaultControls.CreateImage(GetStandardResources());
        go.transform.SetParent(parent);
        return go.GetComponent<Image>();
    }
    public static RawImage CreateRawImage(Transform parent)
    {
        GameObject go = DefaultControls.CreateRawImage(GetStandardResources());
        go.transform.SetParent(parent);
        return go.GetComponent<RawImage>();
    }

    public static Button CreateButton(Transform parent)
    {
        GameObject go = DefaultControls.CreateButton(GetStandardResources());
        go.transform.SetParent(parent);
        return go.GetComponent<Button>();
    }

    public static Toggle CreateToggle(Transform parent)
    {
        GameObject go = DefaultControls.CreateToggle(GetStandardResources());
        go.transform.SetParent(parent);
        return go.GetComponent<Toggle>();
    }
    
    public static ScrollRect CreateScrollView(Transform parent)
    {
        GameObject go = DefaultControls.CreateScrollView(GetStandardResources());
        go.transform.SetParent(parent);
        Transform tran = go.transform.Find("Viewport");
        Object.DestroyImmediate(tran.GetComponent<Mask>());
        tran.gameObject.AddComponent<RectMask2D>();
        return go.GetComponent<ScrollRect>();
    }

    static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            parent = GetOrCreateCanvasGameObject();
        }

        string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
        element.name = uniqueName;
        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
        Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
        GameObjectUtility.SetParentAndAlign(element, parent);
        if (parent != menuCommand.context) // not a context click, so center in sceneview
            SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

        Selection.activeGameObject = element;
    }
    
    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in selection or its parents? Then use just any canvas..
        canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in the scene at all? Then create a new one.
        return CreateNewUI();
    }
    
    public static GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        root.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

        // if there is no event system add one...
        CreateEventSystem(false);
        return root;
    }
    
    static void CreateEventSystem(bool select, GameObject parent = null)
    {
        var esys = Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);
            esys = eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }

        if (select && esys != null)
        {
            Selection.activeGameObject = esys.gameObject;
        }
    }

    static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
    {
        // Find the best scene view
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null && SceneView.sceneViews.Count > 0)
            sceneView = SceneView.sceneViews[0] as SceneView;

        // Couldn't find a SceneView. Don't set position.
        if (sceneView == null || sceneView.camera == null)
            return;

        // Create world space Plane from canvas position.
        Vector2 localPlanePosition;
        Camera camera = sceneView.camera;
        Vector3 position = Vector3.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
        {
            // Adjust for canvas pivot
            localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
            localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

            localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
            localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

            // Adjust for anchoring
            position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
            position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

            Vector3 minLocalPosition;
            minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
            minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

            Vector3 maxLocalPosition;
            maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
            maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

            position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
            position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
        }

        itemTransform.anchoredPosition = position;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localScale = Vector3.one;
    }
}
