using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIToggle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite EnableToggle;
    [SerializeField] private Sprite DisableToggle;
    [SerializeField] private Image TargetImage;

    // Privates
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
    public virtual void DisableVisual()
    {
        if (TargetImage != null
            && DisableToggle != null)
        {
            TargetImage.sprite = DisableToggle;
        }
    }

    public virtual void EnableVisual()
    {
        if (TargetImage != null
            && EnableToggle != null)
        {
            TargetImage.sprite = EnableToggle;
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (TargetImage == null)
        {
            TargetImage = GetComponent<Image>();
        }
    }
#endif
}
