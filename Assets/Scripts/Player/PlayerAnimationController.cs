using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    [SerializeField] private PlayerScript playerScript;

    [Header("Trails")]
    [SerializeField] private TrailRenderer leftHand;
    [SerializeField] private TrailRenderer rightHand;
    [SerializeField] private TrailRenderer groundSlide;
    [SerializeField] private TrailRenderer jumpTrail;

    [Header("Particle")]
    [SerializeField] ParticleSystem particleRunSmoke;
    [SerializeField] ParticleSystem slamDownPuff;
    [SerializeField] ParticleSystem groundSlideSpark;

    void Awake()
    {
        playerScript = GetComponent<PlayerScript>();
        animator = GetComponent<Animator>();
    }

    // -------------------------------------------- //
    void OnEnable()
    {
        PlayerStateMachine.OnStateExit += ExitState;
        PlayerStateMachine.OnStateEnter += EnterState;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        PlayerStateMachine.OnStateExit -= ExitState;
        PlayerStateMachine.OnStateEnter -= EnterState;
    }
    
    // -------------------------------------------- //
    void Update()
    {
        animator.SetBool("onGround", playerScript.onGround);
        animator.SetBool("isMoving", playerScript.moveInput != Vector2.zero);

    }

    // === STATE PLAYER === // 

    // --- Enter state --- //
    void EnterState(StatePlayer newState)
    {
        switch(newState)
        {
            case StatePlayer.Spinning:
                StartTrail(leftHand);
                StartTrail(rightHand);
                animator.SetBool("isSpinning", true);
                break;
            case StatePlayer.SlammingDown:
                animator.SetBool("isSlammingDown", true);
                break;
            case StatePlayer.Idle:
                break;
            case StatePlayer.SlopeSlide:
            case StatePlayer.Running:
                particleRunSmoke.Play();
                break;
            case StatePlayer.Falling:
                animator.SetBool("isFalling", true);    
                break;
            case StatePlayer.SuperJump:
                AudioManager.Instance.PlayAirSpinSound();
                animator.SetBool("isSuperJump", true);
                break;
            case StatePlayer.CanSuperJump:
                slamDownPuff.Play();
                AudioManager.Instance.PlaySlamDownLand();
                break; 
            case StatePlayer.GroundSlide:
                AudioManager.Instance.PlayGroundSlideLoopSound();
                groundSlideSpark.Play();
                StartTrail(groundSlide);   
                animator.SetBool("isGroundSlide", true);
                break;
            case StatePlayer.Wallslide:
                animator.SetBool("isWallslide", true);
                break;
            case StatePlayer.Jumping:
                StartTrail(jumpTrail);
                animator.SetBool("isJumping", true);
                break;
        }
    }

    // --- Exit state --- //
    void ExitState(StatePlayer exitState)
    {
        switch(exitState)
        {
            case StatePlayer.Spinning:
                EndTrail(leftHand);
                EndTrail(rightHand);
                animator.SetBool("isSpinning", false);
                break;
            case StatePlayer.SlammingDown:
                animator.SetBool("isSlammingDown", false);
                break;
            case StatePlayer.SlopeSlide:
            case StatePlayer.Running:
                particleRunSmoke.Stop();
                break;
            case StatePlayer.Falling:
                animator.SetBool("isFalling", false);
                break;
            case StatePlayer.SuperJump:
                animator.SetBool("isSuperJump", false);
                break;
            case StatePlayer.GroundSlide:
                AudioManager.Instance.StopGroundSlideSound();
                groundSlideSpark.Stop();
                EndTrail(groundSlide);
                animator.SetBool("isGroundSlide", false);
                break;
            case StatePlayer.Wallslide:
                animator.SetBool("isWallslide", false);
                break;
            case StatePlayer.Jumping:
                EndTrail(jumpTrail);
                animator.SetBool("isJumping", false);
                break;
        }
    }

    // === VFX methods === // 
    void StartTrail(TrailRenderer trail)
    {
        trail.emitting = true;
    }

    void EndTrail(TrailRenderer trail)
    {
        trail.emitting = false;
    }
     
    // === ANIMATION EVENTS FOR TRIGGER SOUND === // 
    public void RunStep()
    {
        AudioManager.Instance.PlayRunSound();
    }

    // --- jump event set on jump animation --- //
    public void Jumping()
    {
        AudioManager.Instance.PlayJumpSound();
    }

    // --- airSpin event set on airSpin animation --- //
    public void AirSpinAnimationEvent ()
    {
        AudioManager.Instance.PlayAirSpinSound();
    }

}
