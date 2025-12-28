using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using WanzyeeStudio;

// Responsible for enabling/disabling panels
// Scaling of world space UI based on resolution
public class UIManager : BaseSingleton<UIManager>
{
    public UIPanel[] PanelArray;   
    public Transform Camera;
    public float AnimDuration = 3f;

    //[Header("Listen To")]
    //public IntEventChannelSO ECResolutionChange;

    public UnityEvent<PanelType> EOnAnimFinished;

    public enum PanelType
    {
        Menu,
        Action,
        Leaderboard
    }

    private PanelType CurrentActivePanel = PanelType.Menu;

    void Start()
    {
        SwitchActivePanel(PanelType.Menu);
    }

    //private void OnEnable()
    //{
    //    ECResolutionChange.EOnEvent += OnResolutionChanged;
    //}

    //private void OnDisable()
    //{
    //    ECResolutionChange.EOnEvent -= OnResolutionChanged;
    //}


    //void OnResolutionChanged(int resolution)
    //{
    //    for (int i = 0; i < PanelArray.Length; i++)
    //    {
    //        PanelArray[i].SetRect(resolution);
    //    }
    //}

    public void SwitchActivePanel(PanelType activePanel, bool IsAnimate = false)
    {        
        if (IsAnimate)
        {
            if (Tween.GetTweensCount(Camera) >0)
            {
                // If there is already a tween happening with camera, don't proceed
                return;
            }
            EOnAnimFinished.RemoveAllListeners();
        }
        CurrentActivePanel = activePanel;
        for (int i = 0; i < PanelArray.Length; i++)
        {
            if (PanelArray[i].IsSameType(CurrentActivePanel))
            {
                PanelArray[i].ShowUIPanel();
                if (IsAnimate)
                {
                    AnimateUI(PanelArray[i].CameraPos);
                }else
                {
                    Camera.SetPositionAndRotation(new Vector3(Camera.position.x, PanelArray[i].CameraPos, Camera.position.z), Camera.rotation);
                }
            }
            else
            {
                if (IsAnimate)
                {
                    EOnAnimFinished.AddListener(PanelArray[i].HideUIPanel);
                }
                else
                {
                    PanelArray[i].HideUIPanel(CurrentActivePanel);
                }
            }
        }
    }

    public Tween AnimateUI(float PositionY)
    {
        return Tween.PositionY(Camera, PositionY, AnimDuration, ease: Ease.InOutSine)
                                .OnComplete(() => EOnAnimFinished?.Invoke(CurrentActivePanel));
    }

    public void SetCameraPos(float PositionY)
    {
        Camera.SetPositionAndRotation(new Vector3(Camera.position.x, PositionY, Camera.position.z),
                                        Camera.rotation);
    }
}
