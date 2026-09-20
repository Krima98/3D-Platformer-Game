using UnityEngine;
using UnityEngine.Events;

public class TriggerUniversal : MonoBehaviour
{

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            onTriggerEnter.Invoke();
        }
    }

    // -------------------------------------------- //
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            onTriggerExit.Invoke();
        }
    }  

}
