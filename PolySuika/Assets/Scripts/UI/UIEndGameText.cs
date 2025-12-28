using TMPro;
using UnityEngine;
using WanzyeeStudio;

public class UIEndGameText : MonoBehaviour
{
    //
    public TextMeshPro textMesh;

    [Header("Listen To")]
    public VoidEventChannelSO ECOnRestartTriggered;

    void Start()
    {
        var checkFull = FindAnyObjectByType<CheckFull>();
        if (checkFull != null)
        {
            checkFull.EOnLoseTrigger += Lose;
        }
    }

    private void OnEnable()
    {
        ECOnRestartTriggered.Sub(ResetText);
    }

    private void OnDisable()
    {
        ECOnRestartTriggered.UnSub(ResetText);
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