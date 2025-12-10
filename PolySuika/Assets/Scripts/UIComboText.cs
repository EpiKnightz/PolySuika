using TMPro;
using UnityEngine;

public class UIComboText : MonoBehaviour
{
    public TextMeshPro ComboText;

    public void OnScoreMultiChange(int multi)
    {
        if (multi > 1)
        {
            ComboText.enabled = true;
            ComboText.text = "x" + multi.ToString();            
        }
        else
        {
            ComboText.enabled = false;
        }
    }

    public void OnComboEnd()
    {
        ComboText.enabled = false;
    }
}
