using UnityEngine;
using System;

public class keyTrigger : MonoBehaviour
{
    public static event Action<int> keyPickup;
    private int value = 1;

    void OnTriggerEnter(Collider other)
    {
        keyPickup?.Invoke(value);
        Destroy(gameObject);
    }

}
