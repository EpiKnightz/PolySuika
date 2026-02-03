using UnityEngine;
using UnityEngine.EventSystems;

public class UIShowGraphicOnCondition<T> : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIBehaviour[] GraphicList;

    [Header("Variables")]
    [SerializeField] protected T EqualEnableCondition;
    [SerializeField] private bool IsShowOnEqual = true;

    [Header("Listen To")]
    [SerializeField] private EventChannelSO<T> ConditionEvent;

    private void OnDestroy()
    {
        ConditionEvent.Unsub(CheckCondition);
    }

    protected void Awake()
    {
        ConditionEvent.Sub(CheckCondition);
        SetGraphicList(true);
    }

    private void CheckCondition(T value)
    {
        SetGraphicList(Compare(value));
    }

    protected virtual bool Compare(T value)
    {
        return value.Equals(EqualEnableCondition);
    }

    private void SetGraphicList(bool isEqual)
    {
        bool isEnable = isEqual ? IsShowOnEqual : !IsShowOnEqual;
        if (GraphicList.Length > 0
            && GraphicList[0].enabled != isEnable)
        {
            for (int i = 0; i < GraphicList.Length; i++)
            {
                GraphicList[i].enabled = isEnable;
            }
        }
    }
}
