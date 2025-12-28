using UnityEngine;


public class UIToggle : MonoBehaviour
{
    protected bool UIStateEnable = true;

    public void OnClick()
    {
        UIStateEnable = !UIStateEnable;
        if (UIStateEnable)
        {
            EnableVisual();
        }
        else
        {
            DisableVisual();
        }
        ToggleAction(UIStateEnable);
    }

    public virtual void ToggleAction(bool newState) { }
    public virtual void EnableVisual() { }
    public virtual void DisableVisual() { }
}
