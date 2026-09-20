using System;
using Unity.Cinemachine;
using UnityEngine;

public class SwapCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera Player3DView;
    [SerializeField] private CinemachineCamera Player2Dview;

    public static event Action CameraTransition;
    
    private void OnTriggerEnter(Collider other)
    {
        CameraTransition?.Invoke();
        Player2Dview.Priority = 2;
        Player3DView.Priority = 1;
    }

}
