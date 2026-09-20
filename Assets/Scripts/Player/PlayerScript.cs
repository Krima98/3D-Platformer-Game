using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerScript : MonoBehaviour
{
    public static event Action OnPlayerDeath;
    public static event Action PausePress;
    public Renderer head;
    public Renderer body;

    [Header("Reference")]
    [SerializeField] private Transform playerCam3D;
    [SerializeField] private Transform playerCam2D;
    InputPlayerActions playerInput;
    CharacterController characterController;
    PlayerStateMachine stateMachine;

    [Header("Variables to save on startup")]
    private int ignorePlayer;
    float originalHeight;
    private Vector3 originalCenter;

    [Header("GroundSlide")]
    [SerializeField] private bool roofHitwhileSlide;
    public bool slidePress;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f; 
    private float coyoteTimeCounter; 

    [Header("SLOPE HANDLER")]
    [SerializeField] private float inputSpeed = 3f;
    [SerializeField] private float slopeDownForce = -6f;
    [SerializeField] private float steepSlopeDownForce = -10f;

    [Header("GroundHandler ray")]
    public bool onGround { get; private set; }
    [SerializeField] private float groundRaySphereLength = 0.29f;
    [SerializeField] private float originStart = 0.29f;
    [SerializeField] private bool movingDownSlope;
    [SerializeField] private bool movingUpSlope;
    [SerializeField] private bool steepSlope;
    [SerializeField] float walkableSlopeAngle = 36f;
    [SerializeField] private Vector3 moveProjectOnFloor;
    [SerializeField] private Vector3 groundNormal; 
    [SerializeField] private float groundAngle;

    [Header("radius ground ray")]
    [SerializeField] private float radiusMulti = 1f;
    [SerializeField] private float rayDistance = 1f;

    [Header("WallHandler ray")]
    [SerializeField] bool touchingSlideableWall = false;
    [SerializeField] private float maxWallslideAngle = 80f;
    [SerializeField] Vector3 wallHitNormal;
    [SerializeField] private float wallRayLenght = 0.2f;
    [SerializeField] private float startRayHightWall = 0.9f;

    [Header("Stamina")]
    [SerializeField] private float staminaDrainAmount;

    [Header("CAMERA TRIGGER")]
    public bool using2Dcam = false;

    [Header("WALLSLIDE")]
    [SerializeField] private float wallJumpHeight = 3f;
    [SerializeField] private float wallJumpForce = 20f;
    [SerializeField] private float wallMovementSpeed = 1.5f;
    
    [Header("BOOLS")]
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isSlammingDown = false;
    [SerializeField] bool lockPlayerMeshRotation = false;
    [SerializeField] public bool canSlam = false; 
    [SerializeField] public bool superJump = false;
    
    [Header("Jumping")]
    [SerializeField, Range(0f, 10f)] private float jumpHeight;
    [SerializeField] private float SuperJumpHeight = 5f;

    [Header("Gravity")]
    private Vector3 playerVelocity;
    [SerializeField] public float gravity = -30f;
    [SerializeField] private float gravitySlam = -25f;
    [SerializeField] private float gravitySpin = 0.2f;
    [SerializeField] private float gravityWallslide = -2f;
    [SerializeField] float maxFallSpeed = -3f; 
    [SerializeField] float gravityDownForce = -6f;

    [Header("Movement")]
    public float extraSpeed;
    public float maxExtraSpeed = 10f;
    private float currentRotationSpeed;
    public Vector2 moveInput { get; private set; }
    private Vector3 moveDirection;
    private Vector3 move;
    [SerializeField, Range(0f, 100f)] private float acceleration = 100f;
    [SerializeField, Range(0f, 100f)] private float deceleration = 100f;

    [Header("AIR CONTROLL")]
    [SerializeField, Range(0f, 1f)] private float airControl = 0.5f;
    [SerializeField] private float movementSpeed = 15f;
    [SerializeField] private float rotationSpeed = 15f; 
    [SerializeField, Range(0f, 10f)] private float decayExtraSpeedAmount = 4f;

    [Header("Slam Jump Buffer")]
    [SerializeField] private float slamJumpBufferTime = .25f; // 0.25 seconds buffer
    private float slamJumpBufferCounter = 0f;

    [Header("groundSlide")]
    [SerializeField] GameObject playerRig;
    private Quaternion playerNormalRotation;
    [SerializeField] float slideHeight = 0.7f;
    [SerializeField, Range(0f, 10f)] float extraSpeedBooster = 10f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    // -------------------------------------------- //
    void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Jump.performed += OnJump; 
        playerInput.Player.SlamToFloor.performed += OnSlam;
        playerInput.Player.Jump.canceled += OnJumpRelease;
        playerInput.Player.Respawn.performed += OnRespawnPress;
        playerInput.Player.Pause.performed += OnPausePress;

        SwapCamera.CameraTransition += CameraTransition;
        PlayerStateMachine.OnStateExit += ExitState;
        PlayerStateMachine.OnStateEnter += EnterState;
        GameStateMachine.OnStateEnter += EnterGameState;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Jump.performed -= OnJump;
        playerInput.Player.SlamToFloor.performed -= OnSlam;
        playerInput.Player.Jump.canceled -= OnJumpRelease;
        playerInput.Player.Respawn.performed -= OnRespawnPress;
        playerInput.Player.Pause.performed -= OnPausePress;

        SwapCamera.CameraTransition -= CameraTransition;
        PlayerStateMachine.OnStateExit -= ExitState;
        PlayerStateMachine.OnStateEnter -= EnterState;
        GameStateMachine.OnStateEnter -= EnterGameState;
    }

    // -------------------------------------------- //
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = new InputPlayerActions();
        stateMachine = GetComponent<PlayerStateMachine>();
    
        playerNormalRotation = playerRig.transform.localRotation;
        currentStamina = maxStamina;
    }

    // -------------------------------------------- //
    void Start()
    {
        SetSkin();
        StoreStartValues();
    }

    // -------------------------------------------- //
    void Update()
    {   
        SetPlayerState();
        HandleExtraSpeed();
        UpdateState();
        GroundHandler();
        CheckIfCanSuperJump();
        WallHandler();
        HandleMovement();       
    }

    // ===== RAY HANDLER =====
    // sender ut sphere cast som setter onGround True, hvis true, sender den ut en raycast rett ned for vinkel av bakken.
    void GroundHandler()
    {
        float radius = characterController.radius * radiusMulti; 
        Vector3 origin = transform.position + Vector3.up * originStart;
        onGround = Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit sphereHit, groundRaySphereLength, ignorePlayer, QueryTriggerInteraction.Ignore);

        if(!onGround)
        {
            SetAirStatus();
            return;
        };

        canSlam = false;
        isSlammingDown = false;
        coyoteTimeCounter = coyoteTime;
        CalculateSlopeLogic();
    }

    // -------------------------------------------- //
    void SetAirStatus()
    {
        movingUpSlope = false;
        movingDownSlope = false;
        steepSlope = false;
        coyoteTimeCounter -= Time.deltaTime;
    }

    // -------------------------------------------- //
    void CalculateSlopeLogic()
    {
        Vector3 origin = transform.position + Vector3.up * originStart;
        bool raycastHit = Physics.Raycast(origin, Vector3.down, out RaycastHit rayHit, rayDistance, ignorePlayer, QueryTriggerInteraction.Ignore);

        if (raycastHit)
        {
            groundAngle = Vector3.Angle(Vector3.up, rayHit.normal);
            groundNormal = rayHit.normal;

            if(groundAngle <= walkableSlopeAngle) 
            {
                steepSlope = false;

                // Ai for å finne groundDot, for å vite om karakteren går ned eller opp en bakke.
                moveProjectOnFloor = Vector3.ProjectOnPlane(moveDirection, rayHit.normal).normalized;
                float groundDot = Vector3.Dot(moveProjectOnFloor, Vector3.up);
                movingUpSlope = groundDot > 0.1f;
                movingDownSlope = groundDot < -0.1f;
                return; 
            } else 
            {
                steepSlope = true;
            }  
        }
    }
    
    // -------------------------------------------- //
    void WallHandler()
    {
        Vector3 startPos = transform.position + Vector3.up * startRayHightWall;
        float sphereRadius = characterController.radius * 0.8f;

        bool wallRaycast = Physics.SphereCast(startPos, sphereRadius, transform.forward, out RaycastHit wallHit, wallRayLenght, ignorePlayer, QueryTriggerInteraction.Ignore);

        if(wallRaycast)
        {
            wallHitNormal = wallHit.normal;
            bool noWallSlideLayer = wallHit.collider.gameObject.layer == LayerMask.NameToLayer("noWallSlide");
            
            float wallAngle = Vector3.Angle(wallHitNormal, Vector3.up);
            bool canWallSlideOn = wallAngle >= maxWallslideAngle;

            touchingSlideableWall = canWallSlideOn && !onGround && playerVelocity.y < 0f && !noWallSlideLayer;
        } else
        {
            touchingSlideableWall = false;
        }
    }

void OnDrawGizmosSelected()
{
    // Henter baseradius fra CharacterController (eller bruker 0.5f som nød-løsning i editoren)
    float baseRadius = characterController != null ? characterController.radius : 0.5f;


    // ==========================================
    // 1. VEGG-SJEKK (WallHandler)
    // ==========================================
    Vector3 wallStart = transform.position + Vector3.up * startRayHightWall;
    float wallRadius = baseRadius * 0.8f;

    // Grønn hvis du sklir på vegg, rød hvis ikke
    Gizmos.color = touchingSlideableWall ? Color.green : Color.red;
    Gizmos.DrawRay(wallStart, transform.forward * wallRayLenght);
    Gizmos.DrawWireSphere(wallStart + transform.forward * wallRayLenght, wallRadius);


    // ==========================================
    // 2. BAKKE-SJEKK (GroundHandler)
    // ==========================================
    Vector3 groundStart = transform.position + Vector3.up * originStart;
    float groundRadius = baseRadius * radiusMulti;

    // Grønn hvis du står på bakken, rød hvis du er i lufta
    Gizmos.color = onGround ? Color.green : Color.red;
    Gizmos.DrawRay(groundStart, Vector3.down * groundRaySphereLength);
    Gizmos.DrawWireSphere(groundStart + Vector3.down * groundRaySphereLength, groundRadius);


    // ==========================================
    // 3. BAKKE-VINKEL / SLOPE (CalculateSlopeLogic)
    // ==========================================
    Vector3 slopeStart = transform.position + Vector3.up * originStart;
    bool slopeHit = Physics.Raycast(slopeStart, Vector3.down, out RaycastHit rayHit, rayDistance, ignorePlayer, QueryTriggerInteraction.Ignore);

    if (slopeHit)
    {
        // Gul hvis bakken er trygg, lilla (magenta) hvis den er for bratt (steepSlope)
        float currentGroundAngle = Vector3.Angle(Vector3.up, rayHit.normal);
        Gizmos.color = (currentGroundAngle <= walkableSlopeAngle) ? Color.yellow : Color.magenta;

        // Tegner selve strålen og en liten "pinne" som viser vinkelen på bakken (normalen)
        Gizmos.DrawLine(slopeStart, rayHit.point);
        Gizmos.DrawRay(rayHit.point, rayHit.normal * 0.4f);
    }
    else
    {
        // Hvit linje hvis den overhode ikke når ned til bakken
        Gizmos.color = Color.white;
        Gizmos.DrawRay(slopeStart, Vector3.down * rayDistance);
    }
}

    // ==== MOVEMENT =====
    void HandleMovement()
    {
        
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        // linket til 2 forskjellige kameraer i Unity prosjektet
        Transform cam = using2Dcam ? playerCam2D : playerCam3D;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        
        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;        

        move = CalculateMovement(); // horisontal bevegelse (x,z)

        Gravity(); // vertikal

        if(moveInput != Vector2.zero)
        {
            RotatePlayer(moveDirection);
        }
        Vector3 finalMove = (move + playerVelocity) * Time.deltaTime;     
        characterController.Move(finalMove);
    }

    // -------------------------------------------- //
    void Gravity()
    {
        // CollisionFlags.Above er funnet av Ai, som gjør at chrachterController sin kollisjonsvolum kan se om noe treffer over.
        // I teorien kan en vanlig raycast bli sendt ut for å sjekke dette.
        bool hitRoof = (characterController.collisionFlags & CollisionFlags.Above) != 0 && playerVelocity.y > 0f;
        
        if (hitRoof)
        {
            playerVelocity.y = 0f;
        }

        switch(stateMachine.currentState)
        {
            case StatePlayer.Spinning:
                playerVelocity.y += gravity * gravitySpin * Time.deltaTime;
                playerVelocity.y = Mathf.Clamp(playerVelocity.y, maxFallSpeed, jumpHeight);
                break;
            case StatePlayer.Wallslide:
                playerVelocity.y = playerVelocity.y < 0f ? gravityWallslide : playerVelocity.y += gravity * Time.deltaTime;
                break;
            case StatePlayer.SlammingDown:
                playerVelocity.y = gravitySlam;
                break;
            default:
                playerVelocity.y += gravity * Time.deltaTime;    
                break;
        } 

        if (onGround && playerVelocity.y < 0f)
        {
            playerVelocity.y = gravityDownForce;
        } 
    }

    // ==== STATE HANDELING ====
    // setter player state avhenig av situasjon spilleren er i.
    void SetPlayerState()
    {
        bool spinButtonPress = playerInput.Player.AirSpin.IsPressed();
        bool hasStamina = currentStamina > 0f;
        bool isMoving = moveInput != Vector2.zero;
        slidePress = playerInput.Player.GroundSlide.IsPressed();
        bool wallSlide = touchingSlideableWall;
        bool landFromSlam = slamJumpBufferCounter > 0f;

        if(onGround)
        {
            if(landFromSlam)
            {
                stateMachine.SetState(StatePlayer.CanSuperJump);
                return;
            }

            if(steepSlope)
            {
                stateMachine.SetState(StatePlayer.SlopeSlide);
                return;
            }

            if(slidePress && isMoving && !movingUpSlope || roofHitwhileSlide)
            {
                stateMachine.SetState(StatePlayer.GroundSlide);
                return;   
            }

            if(isMoving)
            {
                stateMachine.SetState(StatePlayer.Running);
                return;
            }

            stateMachine.SetState(StatePlayer.Idle);
        }

        if(!onGround)
        {
            if(wallSlide)
            {
                stateMachine.SetState(StatePlayer.Wallslide);
                return; 
            }

            if(isSlammingDown)
            {
                stateMachine.SetState(StatePlayer.SlammingDown);
                return;
            }

            if(hasStamina && spinButtonPress)
            {
                stateMachine.SetState(StatePlayer.Spinning);
                return;
            }

            if(superJump)
            {
                stateMachine.SetState(StatePlayer.SuperJump);
                return;
            }

            if(playerVelocity.y > 0f || isJumping && playerVelocity.y > 0f)
            {
                stateMachine.SetState(StatePlayer.Jumping);
                return;
            }

            if (playerVelocity.y < 0f) 
            {
                stateMachine.SetState(StatePlayer.Falling);
                return;
            }
        }
    }

    // -------------------------------------------- //
    void CheckIfCanSuperJump()
    {
        if (canSlam)
        {
            slamJumpBufferCounter = slamJumpBufferTime;
            return;
        } 

        if (slamJumpBufferCounter > 0f)
        {
            slamJumpBufferCounter -= Time.deltaTime;   
        } 
    }

    // ------------------------------ // 
    public void PlayerDeath()
    {
        gameObject.SetActive(false);
        OnPlayerDeath?.Invoke();
    }

    // opptaterer spesefikke metoder hver frame, kun for ulike tilstander
    void UpdateState()
    {
        switch(stateMachine.currentState)
        {
            case StatePlayer.Spinning:
                StaminaDrain(staminaDrainAmount * Time.deltaTime);
                break;
            case StatePlayer.GroundSlide:
                UpdateGroundSlide();
                break;
            default:
                break;
        }
    }

    // --------------------------- //
    void EnterSlideState()
    {
        characterController.height = slideHeight;
        characterController.center = new Vector3(originalCenter.x, slideHeight / 2f, originalCenter.z);
    }

    // skyter raycast over hodet til spilleren som sjekker om spilleren er under noe eller ikke
    void UpdateGroundSlide()
    {
        bool rayOverHead = Physics.Raycast(transform.position, Vector3.up, 2f, ignorePlayer, QueryTriggerInteraction.Ignore); // 2f må være en variabel

        if(rayOverHead)
        {
            roofHitwhileSlide = true;
            playerInput.Player.Jump.Disable();
        } else
        {
            roofHitwhileSlide = false;
            playerInput.Player.Jump.Enable();
        }        
    }

    // skrur av playerInput basert på gamestate
    void EnterGameState(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.Paused:
                playerInput.Disable();
                break;
            case GameState.Playing:
                playerInput.Enable();
                break;
            case GameState.LevelComplete:
                playerInput.Player.Pause.Disable();
                playerInput.Player.Respawn.Disable();
                break;
        }
    }

    // --------------------------- //
    void SetRotationWallslide()
    {
        Vector3 wallDireciton = new Vector3(-wallHitNormal.x, 0f, -wallHitNormal.z);
        transform.forward = wallDireciton;
    }

    // ============ Helper methods ===============
    // retunerer movement vardi avhenig av bakkegrad, eller på bakken eller i luften
    Vector3 CalculateMovement()
    {
        bool isMoving = moveInput != Vector2.zero;

        float movementSpeed = CurrentMovementSpeed();
        movementSpeed += extraSpeed;

        Vector3 targetMove = moveDirection * movementSpeed;

        // bakken er bratt 
        if(groundAngle > 60f || steepSlope)
        {
            float downForceOnSlope = groundAngle > 60f ? steepSlopeDownForce : slopeDownForce; 

            // Mathf.Abs er hentet fra AI
            Vector3 slopeDir = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized * Mathf.Abs(downForceOnSlope); 
            Vector3 moveOnSlope = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized * inputSpeed;

            move = slopeDir + moveOnSlope;
            return move;         
        }

        if(onGround) // på bakken
        { 
            targetMove = Vector3.ProjectOnPlane(targetMove, groundNormal);
            move = isMoving ? Vector3.MoveTowards(move, targetMove, acceleration * Time.deltaTime) : Vector3.MoveTowards(move, Vector3.zero, deceleration * Time.deltaTime);
            return move;  
        } else // i lufta
        { 
            move = isMoving ? Vector3.MoveTowards(move, targetMove, acceleration * airControl * Time.deltaTime) : Vector3.MoveTowards(move, Vector3.zero, deceleration * Time.deltaTime);         
        }
        return move;
    }

    // --------------------------- //
    float CurrentMovementSpeed()
    {
        switch (stateMachine.currentState)
        {
            case StatePlayer.Wallslide:
                return wallMovementSpeed;
            default:
                return movementSpeed;
        }
    }

    // --------------------------- //
    void RotatePlayer(Vector3 direction)
    {   
        currentRotationSpeed = lockPlayerMeshRotation ? 0f : rotationSpeed;
        transform.forward = Vector3.Slerp(transform.forward, direction, currentRotationSpeed * Time.deltaTime);

        switch(stateMachine.currentState)
        {
            case StatePlayer.GroundSlide:
                // AI laget denne og neste linje, - 95f er å kontre rotasjonen fra import av modellen
                Quaternion targetRotation = Quaternion.Euler(groundAngle - 95f, 0, 0);
                playerRig.transform.localRotation = Quaternion.Slerp(playerRig.transform.localRotation, targetRotation, Time.deltaTime * 20f);
                break;
        }   
    }

    // ============ start setup methods =========== //

    void SetSkin()
    {
        SkinDataSO skin = GameManager.Instance.currentSkin;
        head.material = skin.materialHead;
        body.material = skin.materialBody;
    }

    // -------------------------------------------------------------- //
    void StoreStartValues()
    {
        originalHeight = characterController.height;
        originalCenter = characterController.center;
        ignorePlayer = ~LayerMask.GetMask("Player");   
    }

    // ============ Stamina and extra movement speed ============ //

    public void StaminaDrain(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
    }

    // -------------------------------------------------------------- //
    void RefillStamina()
    {
        currentStamina = maxStamina;
    }

    // -------------------------------------------------------------- //
    void HandleExtraSpeed()
    {
        if(movingDownSlope && slidePress)
        {
            ExtraSpeedBoost();
        } else
        {
            DecayExtraSpeed();
        }

        extraSpeed = Mathf.Clamp(extraSpeed, 0f, maxExtraSpeed);
    }

    // -------------------------------------------------------------- //
    void ExtraSpeedBoost()
    {
        extraSpeed += extraSpeedBooster * Time.deltaTime;
    }

    // -------------------------------------------------------------- //
    void DecayExtraSpeed()
    {
        if (extraSpeed > 0f) 
        {
            extraSpeed -= decayExtraSpeedAmount * Time.deltaTime;
        }
    }

    // ============ Movement abilites methods ============ //

    public void Jump(float height)
    {       
        playerVelocity.y = Mathf.Sqrt(-2f * gravity * height); 
    }

    // -------------------------------------------------------------- //
    void WallJumpPush(float pushForce) 
    {
        Vector3 jumpDirection = wallHitNormal.normalized;
        move = jumpDirection * pushForce;
    }

    // =========== State machine events ============ //

    // --------------------------------------- //
    void EnterState(StatePlayer newState)
    {
        switch(newState)
        {
            case StatePlayer.Idle:
            case StatePlayer.Running:
                RefillStamina();
                break;
            case StatePlayer.Spinning:
                lockPlayerMeshRotation = true;
                break;
            case StatePlayer.GroundSlide:
                RefillStamina();
                EnterSlideState();
                break;
            case StatePlayer.Wallslide:
                lockPlayerMeshRotation = true;
                playerInput.Player.SlamToFloor.Disable();
                SetRotationWallslide();
                break;
        }
    }

    // --------------------------------------- //
    void ExitState(StatePlayer exitState)
    {
        switch(exitState)
        {
            case StatePlayer.Spinning:
                lockPlayerMeshRotation = false;
                break;
            case StatePlayer.Jumping:
                isJumping = false;
                break;
            case StatePlayer.SlammingDown:
                isSlammingDown = false;
                break;
            case StatePlayer.SuperJump:
                superJump = false;
                break;
            case StatePlayer.GroundSlide:
                characterController.height = originalHeight;
                characterController.center = originalCenter;
                playerRig.transform.localRotation = playerNormalRotation; // reseter rotasjonen for slope
                break;
            case StatePlayer.Wallslide:
                lockPlayerMeshRotation = false;
                playerInput.Player.SlamToFloor.Enable();
                break;
        }
    }

    // =========== Input events =========== //
    // -------------------------------------------------------------- //
    private void OnPausePress(InputAction.CallbackContext context)
    {
        PausePress?.Invoke();
    }

    // -------------------------------------------------------------- //
    void OnSlam(InputAction.CallbackContext context)
    {
        if (!onGround && !isSlammingDown)
        {
            isSlammingDown = true;
            canSlam = true;
        }
    }

    // -------------------------------------------------------------- //
    void OnJump(InputAction.CallbackContext context)
    {   
         
        switch(stateMachine.currentState)
        {
            case StatePlayer.CanSuperJump:
                Jump(SuperJumpHeight);
                superJump = true;
                slamJumpBufferCounter = 0f;
                break;
            case StatePlayer.Wallslide:
                WallJumpPush(wallJumpForce);
                Jump(wallJumpHeight);
                break;
            case StatePlayer.Idle:
            case StatePlayer.Running:
            case StatePlayer.SlopeSlide:
            case StatePlayer.GroundSlide:
                isJumping = true;
                Jump(jumpHeight);
                coyoteTimeCounter = 0f;  
                break;
            case StatePlayer.Falling:
                if(coyoteTimeCounter > 0f)
                {
                    isJumping = true;
                    Jump(jumpHeight); 
                }
                break;
        }
    }

    // -------------------------------------------------------------- //
    void OnJumpRelease(InputAction.CallbackContext context)
    {
        switch(stateMachine.currentState)
        {
            case StatePlayer.SuperJump:
                return;
            default:
                if (playerVelocity.y > 0f)
                {
                    playerVelocity.y *= 0.5f; 
                }
                break;
        }
    }

    // -------------------------------------------------------------- //
    void OnRespawnPress(InputAction.CallbackContext context)
    {
        PlayerDeath();
    }

    // -------------------------------------------------------------- //
    void CameraTransition()
    {
        using2Dcam = true;
    }

}
