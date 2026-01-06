using OmniSARTechnologies.LiteFPSCounter;
using UnityEngine;

[RequireComponent(typeof(LiteFPSCounter))]
public class UIDebugTrigger : MonoBehaviour
{
    [Header("References")]
    private LiteFPSCounter FpsCounter;

    [Header("Variables")]
    [SerializeField] private int FingerCountToTrigger = 5;

    private void Awake()
    {
        FpsCounter.gameObject.SetActive(false);
    }

    public void OnFingerCount(int count)
    {
        if (count > FingerCountToTrigger)
        {
            FpsCounter.gameObject.SetActive(!FpsCounter.gameObject.activeSelf);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (FpsCounter == null)
        {
            FpsCounter = GetComponent<LiteFPSCounter>();
        }
    }
#endif
}
