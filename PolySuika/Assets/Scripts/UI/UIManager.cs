using PrimeTween;
using Sortify;
using UnityEngine;
using UnityEngine.Events;
using WanzyeeStudio;

// Responsible for enabling/disabling panels
// Scaling of world space UI based on resolution
public class UIManager : BaseSingleton<UIManager>
{
    [Header("References")]
    public UIPanel[] PanelArray;   
    public Transform Camera;

    [BetterHeader("Variables")]
    public float AnimDuration = 3f;

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
                    AnimateUI(PanelArray[i].GetCameraPos());
                }else
                {
                    Camera.SetPositionAndRotation(new Vector3(Camera.position.x, PanelArray[i].GetCameraPos(), Camera.position.z), Camera.rotation);
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
