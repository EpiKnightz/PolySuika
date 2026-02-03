using TMPro;
using UnityEngine;

public class UIEndGameText : MonoBehaviour
{
    //
    public TextMeshPro textMesh;

    [Header("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;
    public VoidEventChannelSO ECOnLoseTrigger;
    public VoidEventChannelSO ECOnGameOver;

    private void OnEnable()
    {
        ECOnRestartTriggered.Sub(ResetText);
        ECOnLoseTrigger.Sub(Lose);
        ECOnGameOver.Sub(TargetReach);
    }

    private void OnDisable()
    {
        ECOnRestartTriggered.Unsub(ResetText);
        ECOnLoseTrigger.Unsub(Lose);
        ECOnGameOver.Unsub(TargetReach);
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

    private void TargetReach()
    {
        textMesh.enabled = true;
        textMesh.text = "Complete!";
    }

    public void Lose()
    {
        textMesh.enabled = true;
        textMesh.text = "Game Over!";
    }

    public void ResetText()
    {
        textMesh.text = ".";
        textMesh.enabled = false;
    }
}