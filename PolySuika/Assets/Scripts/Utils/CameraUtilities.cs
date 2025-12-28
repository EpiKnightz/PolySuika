using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class CameraUtilities : MonoBehaviour
{
    [Header("Broadcast On")]
    public IntEventChannelSO ECResolutionChange = null;

    public float target_fov = 14.25f;

    private int current_h = 2400;
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
        if (Camera == null)
        {
            Camera = GetComponent<Camera>();
        }
        SetRes();
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
    }

    private void SetRes()
    {
        current_h = Screen.height;
        Camera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(target_fov, Camera.aspect);
        if (Screen.width != 0)
        {
            int CalculatedHeight = (Screen.height * BaseWidth) / Screen.width;
            ECResolutionChange.Invoke(CalculatedHeight);
        }        
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Screen.height != current_h)
        {
            SetRes();
        }
    }
#endif
}
