﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
public class CanvasHelper : MonoBehaviour
{
    static bool allowOnAndroid = false;
    static bool allowOnIos = true;

    public static UnityEvent onOrientationChange = new UnityEvent();
    public static UnityEvent onResolutionChange = new UnityEvent();
    public static bool isLandscape { get; private set; }

    private static List<CanvasHelper> helpers = new List<CanvasHelper>();

    private static bool screenChangeVarsInitialized = false;
    private static ScreenOrientation lastOrientation = ScreenOrientation.Portrait;
    private static Vector2 lastResolution = Vector2.zero;
    private static Rect lastSafeArea = Rect.zero;

    private Canvas canvas;
    private RectTransform rectTransform;

    [SerializeField] List<RectTransform> safeAreaTransforms;

    void Awake()
    {
        if (!helpers.Contains(this))
            helpers.Add(this);

        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if (!screenChangeVarsInitialized)
        {
            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;
            lastSafeArea = Screen.safeArea;

            screenChangeVarsInitialized = true;
        }
    }

    void Start()
    {
        ApplySafeArea();
    }

    void Update()
    {
        if (helpers != null && helpers.Count > 0 && helpers[0] != this)
            return;

        if (Application.isMobilePlatform)
        {
            if (Screen.orientation != lastOrientation)
                OrientationChanged();

            if (Screen.safeArea != lastSafeArea)
                SafeAreaChanged();
        }
        else
        {
            //resolution of mobile devices should stay the same always, right?
            // so this check should only happen everywhere else
            if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
                ResolutionChanged();
        }
    }

    void ApplySafeArea()
    {
        if (!IsAllowSetupSafeArea || safeAreaTransforms == null || safeAreaTransforms.Count == 0)
            return;

        var safeArea = Screen.safeArea;
        Debug.LogError("Safe Area = " + safeArea.ToString());
        //float oldHeight = safeArea.height;
        //safeArea.height = safeArea.width / 9.0f * 18.0f;
        //safeArea.y = safeArea.y + (oldHeight - safeArea.height) * 0.5f;

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        foreach (var safeAreaTransform in safeAreaTransforms)
        {
            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
        }

        // Debug.Log(
        // "ApplySafeArea:" +
        // "\n Screen.orientation: " + Screen.orientation +
        // #if UNITY_IOS
        // "\n Device.generation: " + UnityEngine.iOS.Device.generation.ToString() +
        // #endif
        // "\n Screen.safeArea.position: " + Screen.safeArea.position.ToString() +
        // "\n Screen.safeArea.size: " + Screen.safeArea.size.ToString() +
        // "\n Screen.width / height: (" + Screen.width.ToString() + ", " + Screen.height.ToString() + ")" +
        // "\n canvas.pixelRect.size: " + canvas.pixelRect.size.ToString() +
        // "\n anchorMin: " + anchorMin.ToString() +
        // "\n anchorMax: " + anchorMax.ToString());
    }

    void OnDestroy()
    {
        if (helpers != null && helpers.Contains(this))
            helpers.Remove(this);
    }

    private static void OrientationChanged()
    {
        //Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

        lastOrientation = Screen.orientation;
        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        isLandscape = lastOrientation == ScreenOrientation.LandscapeLeft || lastOrientation == ScreenOrientation.LandscapeRight || lastOrientation == ScreenOrientation.LandscapeLeft;
        onOrientationChange.Invoke();

    }

    private static void ResolutionChanged()
    {
        if (lastResolution.x == Screen.width && lastResolution.y == Screen.height)
            return;

        //Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        isLandscape = Screen.width > Screen.height;
        onResolutionChange.Invoke();
    }

    private static void SafeAreaChanged()
    {
        if (lastSafeArea == Screen.safeArea)
            return;

        //Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

        lastSafeArea = Screen.safeArea;

        for (int i = 0; i < helpers.Count; i++)
        {
            helpers[i].ApplySafeArea();
        }
    }

    public static Vector2 GetCanvasSize()
    {
        return helpers[0].rectTransform.sizeDelta;
    }

    public static Vector2 GetSafeAreaSize()
    {
        for (int i = 0; i < helpers.Count; i++)
        {
            if (helpers[i].safeAreaTransforms != null && helpers[i].safeAreaTransforms.Count > 0)
            {
                return helpers[i].safeAreaTransforms[0].sizeDelta;
            }
        }

        return GetCanvasSize();
    }

    bool IsAllowSetupSafeArea
    {
        get
        {
#if UNITY_ANDROID
            return allowOnAndroid;
#else
            return allowOnIos;
#endif
        }
    }
}