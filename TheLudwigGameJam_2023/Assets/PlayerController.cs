using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows.Speech;

public class PlayerController : MonoBehaviour
{

    Player_Controlls controls;

    [SerializeField] private Color debugColor;
    [SerializeField] private Color debugColor2;


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [Header("References")]

    [SerializeField] private GameObject CootsSpriteGameObject;
    [SerializeField] private Transform GroundedCheckRight;
    [SerializeField] private Transform GroundedCheckCenter;
    [SerializeField] private Transform GroundedCheckLeft;


    [SerializeField] private Wallcheck leftWallCheck;
    [SerializeField] private Wallcheck rightWallCheck;

   


    [SerializeField] private PhysicsMaterial2D groundedMaterial;
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float regularDrag;
    [SerializeField] private float xMaxSpeed;
    [SerializeField] private float horizontalSpeedClamp;
    [SerializeField] private float xSlowdownMultiplyer;

    [Header("Slide Settings")]

    [SerializeField] private float slidingDrag;
    [SerializeField] private float slidingStaleTime;
    [SerializeField] private PhysicsMaterial2D slidingMaterial;




    [Header("Jump Settings")]


    [SerializeField] private float jumpForce;
    [SerializeField] private float horizontalJumpForce;
    [SerializeField] private float jumpHoldMultiplyer;

    [SerializeField] private AnimationCurve jumpHoldMultiplyerAnimationCurve;
    [SerializeField] private float multiJumpTime;

    [SerializeField] private bool useMultiJump;
    
    private enum JumpMultiyOnStage  { None, Start, Hold, Both };
    [SerializeField] private JumpMultiyOnStage jumpMultiyOnStage_;
    [SerializeField] private float secondJumpMultiplyer;
    [SerializeField] private float thirdJumpMultiplyer;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpTimeWait;
    [SerializeField] private float fallMultiplyer;
    [SerializeField] private AnimationCurve fallMultiplyerAnimationCurve;
    [SerializeField] private bool useFallClamp;
    [SerializeField] private float fallMultiplyerClamp;



    [Header("Air Settings")]
    [SerializeField] private float airCorrectionMultiplyer;
    [SerializeField] private AnimationCurve airCorrectionAnimationCurve_Back;
    [SerializeField] private float airSpeedMultiplyer;
    [SerializeField] private AnimationCurve airCorrectionAnimationCurve_Forwards;
    [SerializeField] private float correctionDeadzone;
    [SerializeField] private float airDrag;


    private enum waveDashType { None, Impulse, Hold};
    private enum dashType { None, Regular, Redirect, Both};
    [Header("Dash Settings")]
    [SerializeField] private dashType dashType_;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float waveDashBonusSpeed;
    [SerializeField] private float waveDashBonusTime;
    [SerializeField] private float waveDashBonusClamp;
    [SerializeField] private float dashDrag;
    [SerializeField] private waveDashType waveDashType_;
    [SerializeField] private bool useFailedWaveDash;
    [SerializeField] private float failedWavedashDrag;
    [SerializeField] private float failedWavedashTime;

    [Header("WallSlide Settings")]
    [SerializeField] private float wallClimbMultiplyer;
    [SerializeField] private float wallSlideMutliplyer;
    [SerializeField] private float wallSlideClamp;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallJumpHorizontalForce;
    [SerializeField] private float wallFallJumpMultiplyer;


    private float failedWavedashCounter = 0;
    private bool failedWavedash = false;


    private float waveDashCounter;

    private bool isFacingLeft = false;

    private int currentJumpNum = 0;

    [Header("Input Buffer Settings")]
    [SerializeField] private float inputBuffer;
    [SerializeField] private float coyoteTime;



    [Header("Sprites")]
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private Sprite runningSprite;
    [SerializeField] private Sprite jumpSprite;
    [SerializeField] private Sprite fallingSprite;
    [SerializeField] private Sprite idolSprite;
    [SerializeField] private Sprite slideSprite;
    [SerializeField] private Sprite wallSlideSprite;


    [Header("Dash Objects")]
    [SerializeField] private Transform dashObj1;
    [SerializeField] private Transform dashObj2;
    [SerializeField] private Transform dashObj3;

    [Header("Particals")]

    [SerializeField] private ParticleSystem secondJumpPartical;
    [SerializeField] private ParticleSystem thirdJumpPartical;


    private Vector2 joyStickInput;
    private Vector2 joyStickInputNormilized;

    private bool hasDashed = false;

    private float curJumpMultiplyer = 1;

    private bool instaCancelJumpInput = false;


    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float checkRadius = .01f;

    [HideInInspector]public bool isSliding = false;

    private bool grounded = true;

    private float jumpTimeCounter = 0;
    private bool isJumping = false;

    private float jumpInputCounter = 0;


    private float multiJumpCounter = 0;

    private float slideStaleCounter = 0;

    private float jumpingDirection = 0;


    private float timeSenseAirBorn = 0;


    private bool hasLandedInSlide = false;

    private Transform cootsSpriteTransform;

    // Start is called before the first frame update
    float InputX;
    private void Awake()
    {
        //controls = new Player_Controlls();
        rb = GetComponent<Rigidbody2D>();
        sr = CootsSpriteGameObject.GetComponent<SpriteRenderer>();
        cootsSpriteTransform = CootsSpriteGameObject.GetComponent<Transform>();
        rb.drag = regularDrag;

        //sr.color = debugColor;

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        jumpInputCounter -= Time.deltaTime;
        checkWallCollision();
        updateDrag();
        CheckSlide();
        //Move_Player();
        CheckGrounded();
        
        InputBufferCalculations();
        CheckFacing();
        UpdateSprite();
        if (grounded) hasDashed = false;


    }

    private float spriteFallingDeadzone = 1;

    void UpdateSprite()
    {
        if (!grounded && !onWall)
        {
            if (rb.velocity.y > spriteFallingDeadzone) sr.sprite = jumpSprite;

            if (rb.velocity.y > -spriteFallingDeadzone && rb.velocity.y < spriteFallingDeadzone)
            {
                sr.sprite = idolSprite;
            }
            if (rb.velocity.y < -spriteFallingDeadzone) sr.sprite = fallingSprite;

        }

        if (!grounded)
        {
            if (onWall)
            {
                if (isFacingLeft) sr.flipX = false;
                else sr.flipX = true;
                sr.sprite = wallSlideSprite;
            }
            else
            {
                if (!isFacingLeft) sr.flipX = false;
                else sr.flipX = true;
            }
        }

        if (grounded)
        {
            sr.sprite = idolSprite;
            if(isSliding) sr.sprite = slideSprite;

            if (rb.velocity.x == 0 && !isSliding) sr.sprite = idolSprite;
            if ( Mathf.Abs(rb.velocity.x) > 0 && !isSliding) sr.sprite = runningSprite;

            if (isFacingLeft) sr.flipX = true;
            else sr.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer_Jump();
        Move_Player();
        if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;
    }



    void CheckFacing()
    {


        if (!leftWallCheck.wallColision && !rightWallCheck.wallColision)
        {
            if (grounded || !grounded)
            {
                if (rb.velocity.x > 0)
                {
                    isFacingLeft = false;
                }
                else if (rb.velocity.x < 0)
                {                
                    isFacingLeft = true;
                }
            }
        }

        
    }

    private Vector2 prevVelocity;
    private bool wasGrouneded = false;
    void CheckGrounded()
    {
        //grounded

        grounded = Physics2D.OverlapCircle(GroundedCheckCenter.position, checkRadius, groundLayer) || Physics2D.OverlapCircle(GroundedCheckLeft.position, checkRadius, groundLayer) || Physics2D.OverlapCircle(GroundedCheckRight.position, checkRadius, groundLayer);

        

        if (!wasGrouneded && grounded)
        {
            HardLandingBonus();
        }
        wasGrouneded = grounded;
        if (!grounded)
        {
            grounded = Physics2D.OverlapCircle(GroundedCheckCenter.position, checkRadius, wallLayer);
            prevVelocity = rb.velocity;
        }
        if (grounded)
        {
            hasJumped = false;
            multiJumpCounter -= Time.deltaTime;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            hasLandedInSlide = false;
        }
    }

    void HardLandingBonus()
    {
        if (Mathf.Abs(prevVelocity.y) > Mathf.Abs(prevVelocity.x) && isSliding)
        {

            float Direction = 1;
            if (isFacingLeft) Direction = -1;

            rb.AddForce(new Vector2(Mathf.Abs(prevVelocity.y) * Direction, 0), ForceMode2D.Impulse);
        }
    }
    void Move_Player()
    {

        if (onWall) {
            if(!isSliding)rb.velocity = new Vector2(rb.velocity.x, joyStickInput.y * wallClimbMultiplyer);
            if (!grounded && !isJumping)rb.velocity = new Vector2(0, rb.velocity.y);
        }
        

        // limits max speed
        //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -xMaxSpeed, xMaxSpeed), Mathf.Clamp(rb.velocity.y, -xMaxSpeed, xMaxSpeed));


        //if jumping right


        if (rb.velocity.x > horizontalSpeedClamp) rb.velocity =new Vector2(horizontalSpeedClamp, rb.velocity.y);
        if (rb.velocity.x < -horizontalSpeedClamp) rb.velocity = new Vector2(-horizontalSpeedClamp, rb.velocity.y);

        if (rb.velocity.y > horizontalSpeedClamp) rb.velocity = new Vector2(-rb.velocity.x, horizontalSpeedClamp);

        float moveInput = InputX;
        if (!grounded)
        {
            //if (jumpingDirection > correctionDeadzone)

            float evaluateNum = Mathf.InverseLerp(0, horizontalSpeedClamp, Mathf.Abs(rb.velocity.x));
            if (!isFacingLeft)
            {
                if (InputX < 0) moveInput *= (airCorrectionMultiplyer *airCorrectionAnimationCurve_Back.Evaluate(evaluateNum));
                if (InputX > 0) moveInput *= (airSpeedMultiplyer * airCorrectionAnimationCurve_Forwards.Evaluate(evaluateNum));
            }
            //else if(jumpingDirection < correctionDeadzone)
            if (isFacingLeft)
            {
                if (InputX > 0) moveInput *= (airCorrectionMultiplyer * airCorrectionAnimationCurve_Back.Evaluate(evaluateNum));
                if (InputX < 0) moveInput *= (airSpeedMultiplyer * airCorrectionAnimationCurve_Forwards.Evaluate(evaluateNum));
            }

            //Debug.Log(moveInput);
        }

        if (onWall && !grounded) moveInput = 0;

        if (grounded)
        {
            if (moveInput < 0 && rb.velocity.x > 0) moveInput *= xSlowdownMultiplyer;
            else if(moveInput > 0 && rb.velocity.x < 0) moveInput *= xSlowdownMultiplyer;
             
        }


        if (!grounded)
        {
            Vector2 test = new Vector2(moveInput * speed * Time.deltaTime, rb.velocity.y);
            rb.AddForce(test);
        }

        if ( (xMaxSpeed > rb.velocity.x && rb.velocity.x >= 0) || (-xMaxSpeed < rb.velocity.x && rb.velocity.x <= 0 ) ) {


            //Debug.Log("Add force");
            Vector2 test = new Vector2(moveInput * speed * Time.deltaTime, rb.velocity.y);

            if (isSliding == false) rb.AddForce(test);
        }

        if (onWall)
        {
            if (!isSliding) rb.velocity = new Vector2(rb.velocity.x, joyStickInput.y * wallClimbMultiplyer);
            if (!grounded && !isJumping) rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    void MovePlayer_Jump()
    {
        if (jumpTimeWaitCounter > 0) jumpTimeWaitCounter -= Time.deltaTime;

        //jump stuff

        if (jumpButtonHeldFor > 0)
        {
            //Debug.Log("button held for");
            jumpButtonHeldFor -= Time.deltaTime;
            isJumping = true;
        }
        if (isJumping)
        {
            if (jumpTimeWaitCounter <= 0)
            {
                if (jumpTimeCounter > 0)
                {
                    //rb.velocity = Vector2.up * jumpForce * 20 * Time.deltaTime;

                    //Debug.Log("ADDING FORCE");
                    float curJumpMultiplyer_ = 1;
                    if (jumpMultiyOnStage_ == JumpMultiyOnStage.Both || jumpMultiyOnStage_ == JumpMultiyOnStage.Hold) curJumpMultiplyer_ = curJumpMultiplyer;

                    float jumpHoldMultiplyer_ = jumpHoldMultiplyer * jumpHoldMultiplyerAnimationCurve.Evaluate(Mathf.Lerp(jumpTime, 0, jumpTimeCounter));
                    rb.AddForce(Vector2.up * jumpForce * jumpHoldMultiplyer_ * curJumpMultiplyer_ * Time.deltaTime);

                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    //Debug.Log("Stopped FORCE");
                    isJumping = false;
                }
            }

            //sr.color = debugColor2;
        }

        if (!isJumping && !grounded)
        {
            if (!useFallClamp || rb.velocity.y > -fallMultiplyerClamp)
            {
                float fallMultiplyer_ = fallMultiplyer * fallMultiplyerAnimationCurve.Evaluate(Mathf.InverseLerp(-2, 0, rb.velocity.y));
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplyer_ - 1) * Time.deltaTime;
                
            }
            //else if (rb.velocity.y < -fallMultiplyerClamp) sr.color = Color.red;
        }
        //else sr.color = debugColor;

    }

    private float jumpButtonHeldFor = 0;
    void InputBufferCalculations()
    {
        //if (grounded && (lastJumpInput - Time.time) < inputBuffer && isJumping == false)
        if (grounded && jumpInputCounter > 0)
        {
            jumpButtonHeldFor =  inputBuffer - jumpInputCounter;
            Debug.Log("Input Buffer");
            Jump(0, true);
            if (instaCancelJumpInput)
            {
                //isJumping = false;
                instaCancelJumpInput = false;
            }
        }
        else instaCancelJumpInput = false;        
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        joyStickInput = ctx.ReadValue<Vector2>();
        InputX = joyStickInput.x;
        joyStickInputNormilized = joyStickInput.normalized;

    }

    public void SlideInput(InputAction.CallbackContext ctx)
    {
        
        //Debug.Log(ctx.phase);
        if (ctx.phase == InputActionPhase.Canceled)
        {

            isSliding = false;            
            //sr.color = debugColor;
        }
        if (ctx.phase == InputActionPhase.Started)
        {
            isSliding = true;
            //sr.color = debugColor2;
        }

    }

    void CheckSlide()
    {
        if (!grounded) timeSenseAirBorn = .2f;
        if (grounded)
        {
            if (failedWavedash) failedWavedashCounter -= Time.deltaTime;
            if (failedWavedashCounter <= 0) failedWavedash = false;
            timeSenseAirBorn -= Time.deltaTime;
            if (!hasLandedInSlide && waveDashCounter > 0 && !isSliding)
            {
                
                failedWavedashCounter = failedWavedashTime;
                failedWavedash = true;
            }

            if (isSliding && slideStaleCounter > 0)
            {
                hasLandedInSlide = true;
                //if (hasWavedashed == false && slideStaleCounter == slidingStaleTime && waveDashCounter > 0)
                if (slideStaleCounter == slidingStaleTime && waveDashCounter > 0) WaveDashBonus();
                
                slideStaleCounter -= Time.deltaTime;
                
            }
        }
        else
        {
            slideStaleCounter = slidingStaleTime;
        }
        if (waveDashCounter > 0) waveDashCounter -= Time.deltaTime;
    }


    void WaveDashBonus()
    {
        //Debug.Log("Wavedash Bonus");

        
        float velocityX = rb.velocity.x;
        if (velocityX > 0 &&velocityX > waveDashBonusClamp) velocityX = -waveDashBonusClamp;
        if (velocityX < 0 && velocityX < -waveDashBonusClamp) velocityX = waveDashBonusClamp;


        rb.AddForce(Vector2.left * velocityX * waveDashBonusSpeed * 0.1f, ForceMode2D.Impulse);
        
    }
    public void AttackInput(InputAction.CallbackContext ctx)
    {
        if (!grounded)
        {
            if (ctx.phase == InputActionPhase.Performed && !hasDashed)
            {
                if (isFacingLeft) Dash(1);
                else Dash(-1);

                sr.sprite = dashSprite;
                hasDashed = true;
            }
        }
    }
    
    void Dash(float direction)
    {
        dashObj1.gameObject.SetActive(false);
        dashObj2.gameObject.SetActive(false);
        dashObj3.gameObject.SetActive(false);

        //RotateSprite();

        currentJumpNum = -1;
        //rb.AddForce(Vector2.left * dashSpeed * direction, ForceMode2D.Impulse);
        
        float redirectSpeed = Mathf.Abs(rb.velocity.x) +  Mathf.Abs(rb.velocity.y);
        if (dashType_ == dashType.Regular || dashType_ == dashType.Both)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            waveDashCounter = waveDashBonusTime;
            rb.AddForce(joyStickInputNormilized * dashSpeed, ForceMode2D.Impulse);
        }
        if (dashType_ == dashType.Redirect || dashType_ == dashType.Both)
        {
            //Debug.Log("Test");
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(joyStickInputNormilized * redirectSpeed , ForceMode2D.Impulse);
        }

        StartCoroutine(DashIndicator());
            rb.drag = dashDrag;

    }


    void RotateSprite()
    {

        if (joyStickInputNormilized.x == 0 && joyStickInputNormilized.y == 0.5) rotate_(28);
        if (joyStickInputNormilized.x == 0 && joyStickInputNormilized.y == -0.5) rotate_(-145);


        if (joyStickInputNormilized.x == 0.5 && joyStickInputNormilized.y == 0) rotate_(-145);
    }

    void rotate_(float rotationz)
    {
        cootsSpriteTransform.eulerAngles = new Vector3(
            cootsSpriteTransform.eulerAngles.x, cootsSpriteTransform.eulerAngles.y,
            rotationz);
    }

    IEnumerator DashIndicator()
    {
        dashObj1.position = gameObject.transform.position;
        dashObj1.gameObject.SetActive(true);
        yield return new WaitForSeconds(.051f);
        dashObj2.position = gameObject.transform.position;
        dashObj2.gameObject.SetActive(true);
        yield return new WaitForSeconds(.051f);
        dashObj3.position = gameObject.transform.position;
        dashObj3.gameObject.SetActive(true);
    }
    public void Jump_Input(InputAction.CallbackContext ctx)
    {


        if (ctx.phase == InputActionPhase.Performed)
        {
            jumpInputCounter = inputBuffer;

            int jumpType = -1;
            if (grounded || (coyoteTimeCounter > 0 && !hasJumped) ) jumpType = 0;
            if (onWall) jumpType = 1;


            if (jumpType!= -1) Jump(jumpType);
        }


        
        if (ctx.phase == InputActionPhase.Canceled)
        {
            instaCancelJumpInput = true;
            isJumping = false;
        }
    }
    float jumpTimeWaitCounter = 0;
    float coyoteTimeCounter = 0;
    private bool hasJumped = false;
    void Jump(int jumpType, bool isInputBuffered=false)
    {
        hasJumped = true;
        //Debug.Log("Jump");
        //jumptypes: -1 = noJump,  0 = regular, 1 = walljump;

        if (jumpType == -1) return;
        jumpTimeWaitCounter = jumpTimeWait;

        bool addedJumpAlready = false;
        if (currentJumpNum < 0)
        {
            addedJumpAlready = true;
            currentJumpNum += 1;
        }
        if (instaCancelJumpInput) Debug.Log("insta cancel jump");
        if (multiJumpCounter > 0 && currentJumpNum >= 0 && !addedJumpAlready) currentJumpNum += 1;
        else currentJumpNum = 0;

        jumpingDirection = InputX;
        curJumpMultiplyer = 1;

        if (!useMultiJump) currentJumpNum = 0;

        float horizontalJumpForce_ = 1;
        float jumpForce_ = 1;

        if (jumpType == 0)
        {
            jumpForce_ = jumpForce;
            horizontalJumpForce_ = horizontalJumpForce;
        }
        else if (jumpType == 1)
        {
            horizontalJumpForce_ = wallJumpHorizontalForce;
            jumpForce_ = wallJumpForce;
            if (rb.velocity.y < 0)
            {
                if (joyStickInput.y > 0 && joyStickInput.y != 0)
                {
                    rb.velocity = new Vector2(0, 0);
                    jumpForce_ *= wallFallJumpMultiplyer;
                }
                else jumpForce_ = 0;
                horizontalJumpForce_ *= Mathf.Clamp(Mathf.Abs(rb.velocity.y), 1, horizontalSpeedClamp);

            }
            currentJumpNum = -1;
        }


        if (currentJumpNum == 1)
        {
            secondJumpPartical.Play();
            //sr.color = Color.green;
            curJumpMultiplyer = secondJumpMultiplyer;
        }
        if (currentJumpNum == 2)
        {
            thirdJumpPartical.Play();
            //sr.color = Color.blue;
            curJumpMultiplyer = thirdJumpMultiplyer;
        }
        if (currentJumpNum >= 2) currentJumpNum = -1;

        //if (currentJumpNum == 0) sr.color = Color.white;

        jumpInputCounter = 0;

        float curJumpMultiplyer_ = 1;
        if (jumpMultiyOnStage_ == JumpMultiyOnStage.Both || jumpMultiyOnStage_ == JumpMultiyOnStage.Start) curJumpMultiplyer_ = curJumpMultiplyer;

        Vector2 force_ = Vector2.up * jumpForce_ * curJumpMultiplyer_;
        if (isInputBuffered) force_ = (Vector2.up * jumpForce_ * curJumpMultiplyer_);
        
        if (isInputBuffered) rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(force_, ForceMode2D.Impulse);


        

        if (isFacingLeft)rb.AddForce(Vector2.left * horizontalJumpForce_ * curJumpMultiplyer_, ForceMode2D.Impulse);
        if (!isFacingLeft)rb.AddForce(Vector2.right * horizontalJumpForce_ * curJumpMultiplyer_, ForceMode2D.Impulse);
        jumpTimeCounter = jumpTime;
        isJumping = true;

        multiJumpCounter = multiJumpTime;
    }


    void updateDrag()
    {
        
        if(failedWavedashCounter > 0)
        {
            //Debug.Log("failed wavedash");
            if (useFailedWaveDash)rb.drag = failedWavedashDrag;
        }
        else
        {

           
            if (grounded == true)
            {
                rb.sharedMaterial = groundedMaterial;
                rb.drag = regularDrag;
                if (isSliding && slideStaleCounter > 0)
                {
                    rb.sharedMaterial = slidingMaterial;
                    rb.drag = slidingDrag;
                }
            }
            else rb.drag = airDrag;

            
        }
        //Debug.Log(rb.drag);
    }


    private bool onWall = false;
    void checkWallCollision()
    {
        if (leftWallCheck.wallColision || rightWallCheck.wallColision)
        {
            hasDashed = false;
            //first frame of collision
            if (!onWall)
            {
                float xSpeed = Mathf.Abs(rb.velocity.x);

                if (xSpeed > wallSlideClamp) xSpeed = wallSlideClamp;

                float direction_ = 1;
                if (joyStickInput.y < 0) direction_ = -1;
                Vector2 direction = new Vector2(0, direction_);

                if (Mathf.Abs(rb.velocity.x) > rb.velocity.y) rb.AddForce(direction * xSpeed * wallSlideMutliplyer, ForceMode2D.Impulse);
                rb.velocity = new Vector2(0, rb.velocity.y);
                //sr.color = Color.red;
                onWall = true;
            }
        }
        else
        {
            onWall = false;
            sr.color = Color.white;
        }
        //if (rightWallCheck.wallColision) sr.color = Color.red;

        if (leftWallCheck.wallColision) isFacingLeft = true;
        if (rightWallCheck.wallColision) isFacingLeft = false;

    }

}
