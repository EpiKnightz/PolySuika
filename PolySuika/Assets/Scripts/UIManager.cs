using TMPro;
using UnityEngine;
using WanzyeeStudio;

public class UIManager : BaseSingleton<UIManager>
{
    public TextMeshPro textMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void Win()
    {
        textMesh.text = "You Win!";
    }

    public void Lose()
    {
        textMesh.enabled = true;
        textMesh.text = "You Lose!";
    }
}