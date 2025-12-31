using UnityEngine.Events;

public interface IUIManager
{
    void SwitchActivePanel(PanelType activePanel, bool IsAnimate = false);
    UnityEvent<PanelType> EOnAnimFinished { get; }
}