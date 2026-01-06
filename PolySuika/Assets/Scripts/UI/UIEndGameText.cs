using TMPro;
using UnityEngine;

public class UIEndGameText : MonoBehaviour
{
    //
    public TextMeshPro textMesh;

    [Header("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;
    public VoidEventChannelSO ECOnLoseTrigger;

    private void OnEnable()
    {
        ECOnRestartTriggered.Sub(ResetText);
        ECOnLoseTrigger.Sub(Lose);
    }

    private void OnDisable()
    {
        ECOnRestartTriggered.Unsub(ResetText);
        ECOnLoseTrigger.Unsub(Lose);
    }

    private void Awake()
    {
        textMesh.enabled = false;
    }

    private void Win()
    {
        textMesh.enabled = true;
        textMesh.text = "You Win!";
    }

    public void Lose()
    {
        textMesh.enabled = true;
        textMesh.text = "Game Over!";
    }

    public void ResetText()
    {
        textMesh.enabled = false;
    }
}