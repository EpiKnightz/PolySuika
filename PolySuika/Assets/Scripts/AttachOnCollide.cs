using UnityEngine;

public class AttachOnCollide : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Attachable"))
        {
            //print("Collided with an attachable object:" + collision.rigidbody.GetRelativePointVelocity(transform.position).magnitude);
            if (collision.rigidbody.GetRelativePointVelocity(transform.position).magnitude < 1)
            {
                // Attach the collided object to this object
                collision.transform.SetParent(transform);
                collision.collider.material = gameObject.GetComponent<Collider>().material;
                collision.gameObject.tag.Remove(0);
                collision.gameObject.AddComponent<AttachOnCollide>();
            }
        }
    }
}
