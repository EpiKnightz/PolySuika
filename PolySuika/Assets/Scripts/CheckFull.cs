using PrimeTween;
using UnityEngine;

public class CheckFull : MonoBehaviour
{
    private const int MERGABLE_LAYER = 3;
    public float maxCountdown = 3f;
    public MeshRenderer warningMesh;
    private Material warningMaterial;
    private Tween warningTween;

    private bool bIsCountdown = false;
    private float currentCountdown = 0f;

    public event System.Action OnLoseTrigger;

    private void Start()
    {
        OnLoseTrigger += UIManager.instance.Lose;
        warningMaterial = warningMesh.material;
        warningMaterial.SetFloat("_Alpha", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Mergable mergable = other.GetComponent<Mergable>();
        if (mergable != null)
        {
            // Start countdown
            bIsCountdown = true;
            print("Countdown started");

            warningTween = Tween.Custom(1, 0, duration: 0.5f, ease: Ease.InOutSine, cycles: -1, cycleMode: CycleMode.Yoyo
                , onValueChange: newVal => { if (bIsCountdown) warningMaterial.SetFloat("_Alpha", newVal); });
        }
    }

    // Still have trigger error here
    private void OnTriggerExit(Collider other)
    {
        Mergable mergable = other.GetComponent<Mergable>();
        if (mergable != null)
        {
            // Need to check if there are still mergables inside the trigger
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f,
                                                        transform.rotation, MERGABLE_LAYER);
            if (colliders.Length == 1)
            {
                print("Countdown stopped");
                // Stop countdown
                bIsCountdown = false;
                currentCountdown = 0f;

                warningMaterial.SetFloat("_Alpha", 0);
                warningTween.Stop();
            }
        }
    }

    private void Update()
    {
        if (bIsCountdown)
        {
            currentCountdown += Time.deltaTime;
            if (currentCountdown > maxCountdown)
            {
                print("Countdown finished - You Lose");
                OnLoseTrigger?.Invoke();
                bIsCountdown = false;
            }
        }
    }
}