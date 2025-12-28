using UnityEngine;
using Utilities;
using PrimeTween;

public class UIChangeSetButtons : MonoBehaviour
{
    public float ButtonClickCooldown = 1;
    
    public IntEvent DChangeDataSet;
    
    // Private
    private bool ClickEnable = true;

    private void Start()
    {
        var dataMan = FindAnyObjectByType<DataManager>();
        if (dataMan != null)
        {
            DChangeDataSet += dataMan.OffsetCurrentLevelSet;
        }
    }

    public void OnClick(bool bIsRight)
    {
        if (ClickEnable)
        {

            if (bIsRight)
            {
                DChangeDataSet?.Invoke(1);
            }
            else
            {
                DChangeDataSet?.Invoke(-1);
            }
            ClickEnable = false;
            Tween.Delay(ButtonClickCooldown, EnableClick);
        }
    }

    public void EnableClick()
    {
        ClickEnable = true; 
    }


}
