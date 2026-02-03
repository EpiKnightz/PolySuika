using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIListButton : UIToggle
{
    [Header("References")]
    public RectTransform ListButton;
    public RectTransform[] ListItems;

    [Header("Variables")]
    public float AnimDuration = 0.25f;

    [Header("Listen To")]
    public VoidEventChannelSO[] ECOnTriggerHideList;
    public VoidEventChannelSO[] ECOnTriggerShowList;

    // Privates
    private Vector3[] DesiredItemsPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        UIStateEnable = false;
        DesiredItemsPos = new Vector3[ListItems.Length];
        for (int i = 0; i < ListItems.Length; i++)
        {
            DesiredItemsPos[i] = ListItems[i].localPosition - ListButton.localPosition;
            ListItems[i].localPosition = ListButton.localPosition;
        }
    }

    private void Start()
    {
        DisableButtons();
    }

    private void OnEnable()
    {
        for (int i = 0; i < ECOnTriggerHideList.Length; i++)
        {
            ECOnTriggerHideList[i].Sub(HideList);
        }
        for (int i = 0; i < ECOnTriggerShowList.Length; i++)
        {
            ECOnTriggerShowList[i].Sub(ShowList);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < ECOnTriggerHideList.Length; i++)
        {
            ECOnTriggerHideList[i].Unsub(HideList);
        }
        for (int i = 0; i < ECOnTriggerShowList.Length; i++)
        {
            ECOnTriggerShowList[i].Unsub(ShowList);
        }
    }

    private void HideList()
    {
        if (UIStateEnable)
        {
            OnClick();
        }
    }

    private void ShowList()
    {
        if (!UIStateEnable)
        {
            OnClick();
        }
    }

    public override void EnableVisual()
    {
        // Show buttons
        for (int i = 0; i < ListItems.Length; i++)
        {
            EnableButtons(i, true);
            Tween.LocalPosition(ListItems[i], ListButton.localPosition + DesiredItemsPos[i], AnimDuration, ease: Ease.InOutSine);
        }
    }

    public override void DisableVisual()
    {
        // Hide buttons
        for (int i = 0; i < ListItems.Length; i++)
        {
            Tween.LocalPosition(ListItems[i], ListButton.localPosition, AnimDuration, ease: Ease.InOutSine);
        }
        Tween.Delay(AnimDuration, DisableButtons);
    }

    private void DisableButtons()
    {
        for (int i = 0; i < ListItems.Length; i++)
        {
            EnableButtons(i, false);
        }
    }

    private void EnableButtons(int idx, bool isEnable)
    {
        ListItems[idx].gameObject.SetActive(isEnable);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (ListButton == null)
        {
            ListButton = GetComponent<RectTransform>();
        }
    }
#endif
}
