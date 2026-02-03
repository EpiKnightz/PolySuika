using Reflex.Attributes;
using UnityEngine;

public class UIBackHandler : MonoBehaviour
{
    // Back key currently didn't work on Android, so no need to inject the dependency here
    // Still, in perfect world, back key would go back to menu, then quit the app
    //[Inject] private readonly IUIManager UIManager;

    public void OnClick()
    {
        // if (UIManager != null
        //     && UIManager.GetCurrentActivePanel() != PanelType.Menu)
        // {
        //     UIManager.SwitchActivePanel(PanelType.Menu, true);
        // }
        // else
        // {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
        else
        {
            Application.Quit();
        }
        //}
    }
}
