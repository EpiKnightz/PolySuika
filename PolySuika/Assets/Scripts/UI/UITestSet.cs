using Sortify;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UITestSet : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;
    public UnityEvent unityEvent;

    [BetterHeader("Listen To")]
    public IntEventChannelSO ECOnSetChange;

    //private void OnEnable()
    //{
    //    ECOnSetChange.EOnEvent += UpdateText;
    //}

    //private void OnDisable()
    //{
    //    ECOnSetChange.EOnEvent -= UpdateText;
    //}

    //void UpdateText(int value)
    //{
    //    TextMeshProUGUI.SetText(value.ToString());
    //}
}
