using UnityEngine;

public class FollowPlatform : MonoBehaviour
{
    [SerializeField] Transform platfrom;
    [SerializeField] string playerTag;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            other.transform.SetParent(platfrom);
        }
    }

    // -------------------------------------------- //
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            other.transform.SetParent(null);
        }
    }
}
