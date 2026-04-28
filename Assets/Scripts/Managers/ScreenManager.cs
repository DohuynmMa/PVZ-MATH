using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    // 目标宽高比 (16:9)
    private const float TargetAspectRatio = 16f / 9f;
    private bool _isProcessing = false;
    private Resolution _preferredResolution;
    private Vector2Int _windowedSize;
    private const float Cooldown = 0.5f;
    private float _lastChangeTime;

    private void Awake()
    {
        Instance = this;
        _preferredResolution = Screen.resolutions[Screen.resolutions.Length - 1];
    }

    private void Start()
    {
        _windowedSize = new Vector2Int(Screen.width, Screen.height);
        _lastChangeTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _lastChangeTime < Cooldown)
            return;
        if (!Screen.fullScreen)
        {
            CheckWindowedSizeChange();
        }
    }


    private void CheckWindowedSizeChange()
    {
        if (_isProcessing) return;
        if (Screen.width != _windowedSize.x || Screen.height != _windowedSize.y)
        {
            _isProcessing = true;
            AdjustWindowedAspectRatio();
            _windowedSize = new Vector2Int(Screen.width, Screen.height);
            _lastChangeTime = Time.time;
            _isProcessing = false;
        }
    }

    public void ToggleFullScreen()
    {
        if (_isProcessing) return;
        _isProcessing = true;

        if (Screen.fullScreen)
        {
            Screen.SetResolution(_windowedSize.x, _windowedSize.y, false);
        }
        else
        {
            _windowedSize = new Vector2Int(Screen.width, Screen.height);
            Screen.SetResolution(
                _preferredResolution.width,
                _preferredResolution.height,
                FullScreenMode.FullScreenWindow,
                _preferredResolution.refreshRateRatio
            );
        }

        _lastChangeTime = Time.time;
        _isProcessing = false;
    }

    private void AdjustWindowedAspectRatio()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(currentAspect - TargetAspectRatio) < 0.02f)
            return;
        int newWidth = Mathf.RoundToInt(Screen.height * TargetAspectRatio);
        int newHeight = Mathf.RoundToInt(Screen.width / TargetAspectRatio);
        if (Mathf.Abs(newWidth - Screen.width) < Mathf.Abs(newHeight - Screen.height))
        {
            Screen.SetResolution(newWidth, Screen.height, false);
        }
        else
        {
            Screen.SetResolution(Screen.width, newHeight, false);
        }
    }
}
