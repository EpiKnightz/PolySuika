using Sortify;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class UIPanel : MonoBehaviour
{
    [SerializeField] private RectTransform PanelTransform;
    [SerializeField] private PanelType MenuPanelType;
    [SerializeField] private float CameraPos;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnClickTriggered = null;
    public VoidEventChannelSO ECOnChangePanelAnimFinished = null;
    public VoidEventChannelSO ECOnPanelHidden = null;

    public UnityAction<PanelType, bool> EOnSwitchPanelTriggered;

    public void OnClick()
    {
        EOnSwitchPanelTriggered?.Invoke(MenuPanelType, true);
    }

    public void InvokeSwitchPanelTriggered()
    {
        ECOnClickTriggered?.Invoke();
    }

    public void InvokeAnimFinished(PanelType activePanelType)
    {
        if (activePanelType == MenuPanelType)
        {
            ECOnChangePanelAnimFinished?.Invoke();
        }
    }

    public bool IsSameType(PanelType panelType)
    {
        return panelType == MenuPanelType;
    }

    public void ShowUIPanel()
    {
        EnablePanel(true);
    }

    public void HideUIPanel(PanelType activePanelType)
    {
        if (activePanelType != MenuPanelType)
        {
            EnablePanel(false);
            ECOnPanelHidden?.Invoke();
        }
    }

    public void EnablePanel(bool enabled = true)
    {
        PanelTransform.gameObject.SetActive(enabled);
    }

    public float GetCameraPos()
    {
        return CameraPos;
    }

    private void OnValidate()
    {
        if (PanelTransform == null)
        {
            PanelTransform = GetComponent<RectTransform>();
        }
    }
}