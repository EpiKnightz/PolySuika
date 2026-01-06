using PrimeTween;
using UnityEngine;

public class UIListButton : UIToggle
{
    [Header("References")]
    public RectTransform ListButton;
    public RectTransform[] ListItems;
    //public RectTransform HomeButton;

    [Header("Listen To")]
    public VoidEventChannelSO[] ECOnTriggerHideList;
    public IntEventChannelSO ECOnTriggerShowList;

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

    private void OnEnable()
    {
        for (int i = 0; i < ECOnTriggerHideList.Length; i++)
        {
            ECOnTriggerHideList[i].Sub(HideList);
        }
        ECOnTriggerShowList.Sub(OnFinalScore);
    }

    private void OnDisable()
    {
        for (int i = 0; i < ECOnTriggerHideList.Length; i++)
        {
            ECOnTriggerHideList[i].Unsub(HideList);
        }
        ECOnTriggerShowList.Unsub(OnFinalScore);
    }

    private void HideList()
    {
        if (UIStateEnable)
        {
            OnClick();
        }
    }

    private void OnFinalScore(int Score)
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
            Tween.LocalPosition(ListItems[i], ListButton.localPosition + DesiredItemsPos[i], 0.25f, ease: Ease.InOutSine);
        }
    }

    public override void DisableVisual()
    {
        // Hide buttons
        for (int i = 0; i < ListItems.Length; i++)
        {
            Tween.LocalPosition(ListItems[i], ListButton.localPosition, 0.25f, ease: Ease.InOutSine);
        }
    }
}
