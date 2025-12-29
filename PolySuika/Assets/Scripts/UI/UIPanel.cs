using Sortify;
using UnityEngine;
using Utilities;
using static UIManager;

[RequireComponent(typeof(RectTransform))]
public class UIPanel : MonoBehaviour
{
    [SerializeField] private RectTransform PanelTransform;
    [SerializeField] private PanelType MenuPanelType;
    [SerializeField] private float CameraPos;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnClickTriggered = null;
    public VoidEventChannelSO ECOnChangePanelAnimFinished = null;

    private void Awake()
    {
        if (PanelTransform == null)
        {
            PanelTransform = GetComponent<RectTransform>();
        }
    }

    public void OnClick()
    {
        UIManager.instance.SwitchActivePanel(MenuPanelType, true);
        ECOnClickTriggered?.Invoke();
        UIManager.instance.EOnAnimFinished.AddListener((MenuPanelType) => ECOnChangePanelAnimFinished?.Invoke());
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
}