using Sortify;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraUtilities : MonoBehaviour
{
    [BetterHeader("Variables")]
    [SerializeField] private float TargetFOV = 14.2f;

    [BetterHeader("Broadcast On")]
    public IntEventChannelSO ECResolutionChange = null;

    // Privates
    private int CurrentHeight = 2400;
    private int BaseWidth = 1080;

    [SerializeField] private Camera Camera;

#if UNITY_EDITOR
    private void Awake()
    {
        if (Camera == null)
        {
            Camera = GetComponent<Camera>();
        }
        SetRes();
    }
#endif

    private void Start()
    {
        SetRes();
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
    }

    private void SetRes()
    {
        CurrentHeight = Screen.height;
        Camera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(TargetFOV, Camera.aspect);
        if (Screen.width != 0)
        {
            int CalculatedHeight = Screen.height * BaseWidth / Screen.width;
            ECResolutionChange.Invoke(CalculatedHeight);
        }
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Screen.height != CurrentHeight)
        {
            SetRes();
        }
    }

    private void OnValidate()
    {
        if (Camera == null)
        {
            Camera = GetComponent<Camera>();
        }
    }
#endif
}
