using System;
using UnityEngine;

public class LevelTwo : MonoBehaviour
{

    private int currentKeys = 0;
    [SerializeField] private int keysNeeded = 3;

    [Header("Animations")]
    [SerializeField] private Animator rightDoor;
    [SerializeField] private Animator leftDoor;

    void OnEnable()
    {
        keyTrigger.keyPickup += OnKeyPickup;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        keyTrigger.keyPickup -= OnKeyPickup;
    }

    // --- Events --- //
    private void OnKeyPickup(int value)
    {
        currentKeys += value;
        AudioManager.Instance.PlayCollectKeySound();
        if(currentKeys == keysNeeded)
        {
            rightDoor.SetTrigger("doorUnlocked");
            leftDoor.SetTrigger("doorUnlocked");
        }
    }

}
