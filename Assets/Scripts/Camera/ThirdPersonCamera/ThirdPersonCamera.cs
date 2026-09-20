using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System;

public class ThirdPersonCamera : MonoBehaviour
{

    [Header("Camera controll values")]
    [SerializeField, Range(1f, 10f)] private float zoomSpeed = 4f;
    [SerializeField] private float zoomLeerpSpeed = 10f;
    [SerializeField, Range(3f, 10f)] private float minDistance = 3f;
    [SerializeField, Range(5f, 30f)] private float maxDistance = 15f;

    private InputPlayerActions playerInput;
    private CinemachineOrbitalFollow orbital;
    private CinemachineInputAxisController axisController;

    private float currentZoom;
    private float targetZoom;
    MouseSensitivity mouseSensitivity;

    // === Startup === // 
    void Awake()
    {
        playerInput = new InputPlayerActions();
        orbital = GetComponent<CinemachineOrbitalFollow>();
        axisController = GetComponent<CinemachineInputAxisController>();

        targetZoom = currentZoom = orbital.Radius;
    }
    
    // -------------------------------------------- //
    void OnEnable()
    {
        playerInput.Enable();
        playerInput.Camera.MouseZoom.performed += ZoomCamera;
        PauseView.OnSettingChange += SetSensitivity;
    }
    // -------------------------------------------- //
    void OnDisable()
    {
        playerInput.Disable();
        playerInput.Camera.MouseZoom.performed -= ZoomCamera;
        PauseView.OnSettingChange -= SetSensitivity;
    }

    // -------------------------------------------- //
    void Start()
    {
        SetSensitivity();
    }

    // -------------------------------------------- //
    void Update()
    {
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLeerpSpeed);
        orbital.Radius = currentZoom;
    }

    // === Methods === //
    void SetSensitivity()
    {
        mouseSensitivity = SaveReadManager.Instance.gameData.mouseSensitivity;

        // AI brukt til å finne ut hvordan man finner X og Y aksen for gain cinemachine component (styrer sensetivity)
        axisController.Controllers[0].Input.Gain = mouseSensitivity.sensitivity;
        axisController.Controllers[1].Input.Gain = -mouseSensitivity.sensitivity;
    }

    // -------------------------------------------- //   
    private void ZoomCamera(InputAction.CallbackContext context)
    {
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance); 
    }
}
