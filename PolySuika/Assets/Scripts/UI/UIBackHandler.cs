using Reflex.Attributes;
using UnityEngine;

public class UIBackHandler : MonoBehaviour
{
    [Inject] private readonly IUIManager UIManager;

    public void OnClick()
    {
        if (UIManager != null
            && UIManager.GetCurrentActivePanel() != PanelType.Menu)
        {
            UIManager.SwitchActivePanel(PanelType.Menu, true);
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("Back button clicked");
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
