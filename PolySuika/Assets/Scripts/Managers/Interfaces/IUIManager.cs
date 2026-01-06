using UnityEngine.Events;

public interface IUIManager
{
    PanelType GetCurrentActivePanel();
    void SwitchActivePanel(PanelType activePanel, bool IsAnimate = false);
    UnityEvent<PanelType> EOnAnimFinished { get; }
}