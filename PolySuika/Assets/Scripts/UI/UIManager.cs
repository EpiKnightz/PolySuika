using PrimeTween;
using Reflex.Core;
using Sortify;
using UnityEngine;
using UnityEngine.Events;
public enum PanelType
{
    Menu,
    Action,
    Leaderboard,
    Credit,
}
// Responsible for enabling/disabling panels
// Scaling of world space UI based on resolution
public class UIManager : MonoBehaviour, IUIManager, IInstaller
{
    [Header("References")]
    public UIPanel[] PanelArray;
    public Transform Camera;

    [BetterHeader("Variables")]
    public float AnimDuration = 3f;

    public UnityEvent<PanelType> EOnAnimFinished { get; } = new();
    private PanelType CurrentActivePanel = PanelType.Menu;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this, typeof(IUIManager));
    }

    private void Start()
    {
        foreach (var panel in PanelArray)
        {
            panel.EOnSwitchPanelTriggered += SwitchActivePanel;
        }
        SwitchActivePanel(PanelType.Menu);
    }

    private void OnDestroy()
    {
        foreach (var panel in PanelArray)
        {
            panel.EOnSwitchPanelTriggered -= SwitchActivePanel;
        }
    }

    public void SwitchActivePanel(PanelType activePanel, bool IsAnimate = false)
    {
        if (IsAnimate)
        {
            if (Tween.GetTweensCount(Camera) > 0)
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
                // Show the active panel and animate
                PanelArray[i].InvokeSwitchPanelTriggered();
                PanelArray[i].ShowUIPanel();
                if (IsAnimate)
                {
                    AnimateUI(PanelArray[i].GetCameraPos());
                    EOnAnimFinished.AddListener(PanelArray[i].InvokeAnimFinished);
                }
                else
                {
                    SetCameraPos(PanelArray[i].GetCameraPos());
                }
            }
            else
            {
                // Hide other panels after animation
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

    private Tween AnimateUI(float posY, float duration = -1)
    {
        if (duration == -1)
        {
            duration = AnimDuration;
        }
        return Tween.LocalPositionY(Camera, posY, duration, ease: Ease.InOutSine)
                                .OnComplete(() => EOnAnimFinished?.Invoke(CurrentActivePanel));
    }

    private void SetCameraPos(float posY)
    {
        Camera.localPosition = new Vector3(Camera.localPosition.x, posY, Camera.localPosition.z);
    }

    public PanelType GetCurrentActivePanel()
    {
        return CurrentActivePanel;
    }
}