using UnityEngine;

public class RemoveColliderWhenLose : MonoBehaviour
{
    public Collider targetCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var check = FindAnyObjectByType<CheckFull>();
        if (check != null)
        {
            check.EOnLoseTrigger += OnLoseTrigger;
        }
    }

    void OnLoseTrigger()
    {
        targetCollider.enabled = false;
    }
}
