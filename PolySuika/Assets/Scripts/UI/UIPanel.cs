using Sortify;
using UnityEngine;
using Utilities;
using static UIManager;

[RequireComponent(typeof(RectTransform))]
public class UIPanel : MonoBehaviour
{
    public RectTransform PanelTransform;
    public float CameraPos;
    public PanelType MenuPanelType;

    [BetterHeader("Broadcast On")]
    public VoidEventChannelSO ECOnClickTriggered = null;
    public VoidEventChannelSO ECOnChangePanelAnimFinished = null;

    // Privates
    private float OriginalHeight;

    private void Awake()
    {
        if (PanelTransform == null)
        {
            PanelTransform = GetComponent<RectTransform>();
        }
        OriginalHeight = PanelTransform.rect.height;
    }

    public UIPanel(RectTransform panelTransform, float cameraPos, PanelType type)
    {
        PanelTransform = panelTransform;
        CameraPos = cameraPos;
        MenuPanelType = type;
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
}