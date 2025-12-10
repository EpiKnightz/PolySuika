using PrimeTween;
using UnityEngine;
using Utilities;

public class Mergable : MonoBehaviour
{
    private int Tier = 0;
    private bool isMerging = false;

    public event IntEvent EOnMergeTrigger;
    public event Vector3Event EOnMergePosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (isMerging || Tier == TierManager.instance.GetMaxTier() - 1)
        {
            return;
        }
        // Check if the colliding object has a Mergable component
        Mergable otherMergable = collision.gameObject.GetComponent<Mergable>();
        if (otherMergable != null
            && otherMergable.GetTier() == Tier
            && otherMergable.isMerging == false)
        {
            isMerging = true;
            otherMergable.isMerging = true;
            //otherMergable.enabled = false;
            otherMergable.GetComponent<Collider>().enabled = false;
            this.GetComponent<Collider>().enabled = false;
            Vector3 mergePosition = (otherMergable.transform.position + transform.position) / 2;
            GameObject NewObject = TierManager.instance.SpawnAdvance(Tier + 1,
                mergePosition);
            EOnMergeTrigger?.Invoke(Tier);
            EOnMergePosition?.Invoke(mergePosition);
            //,Quaternion.Lerp(otherMergable.transform.rotation,transform.rotation, 0.5f));
            //Mergable NewMergable = NewObject.GetComponent<Mergable>();

            Destroy(otherMergable.gameObject);
            Destroy(gameObject);
        }
    }

    public void SetTier(int tier)
    {
        Tier = tier;
    }

    public int GetTier()
    {
        return Tier;
    }
}