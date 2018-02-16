using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    // Use this for initialization
    
    Prime31.CharacterController2D charController;
    public bool debugColors = false;
    private KeyCode leftButton = KeyCode.A;                     
    private KeyCode rightButton = KeyCode.D;                     
    private KeyCode upButton = KeyCode.W;                        
    private KeyCode downButton = KeyCode.S;                      
    private KeyCode jumpButton = KeyCode.Space;                  
    private KeyCode attackButton = KeyCode.J;                    
    private KeyCode lungeButton = KeyCode.K;                     
    private KeyCode dashButton = KeyCode.LeftShift;             
    private KeyCode restartButton = KeyCode.R;                  
    /*
     *                      ^ positiveY
     *                      |
     *                      |
     *                      |
     *                      |
     * negativeX<------------------------->positiveX
     *                      |
     *                      |
     *                      |
     *                      V negativeY
     */
    public enum MovementState
    {
        Paralyzed, Free, Dash, Lunge, OnLadder, Hover, Grappled, Clinging
    }
    public MovementState movementState = MovementState.Free;

    private bool allowPlayerInput;              //can the character be controlled?
    private int maxHealth;
    public int health = 1;                      
    private bool playerDead = false;
    public float maxHorizontalVelocity = 0;     //should be positive
    public float maxVerticalVelocity = 0;       //should be positive

    public float acceleration = 0;              //ACTIVE acceleration (when attempting to move in direction of travel)
    public float deceleration = 0;              //ACTIVE deceleration (when attempting to move against direction of travel)
    public float passiveDeceleration = 0;       //basically friction.  also subject to airAccelerationFactor
    public float airAccelerationFactor = 1f;    //from 0.0 to 1.0
    public float highDecelerationFactor = 0f;    //acceleration = highDecelerationDuration if speed < highDecelerationFactor
    public float highDecelerationThreshold = 1f; //use highDecelerationFactor wihle speed < highDecelerationThreshold.  Should be > 1 for more resisting force

    public float gravity = 0;                   //should be positive
    public float jumpForce = 0;                 //should be positive
    public float jumpButtonReleaseFactor = 1f;  //total velocity will be mulitiplied by this factor when the jump button is released
                                                //AND before the apex of the jump is reached (from 0.0 to 1.0)

    public float attackDuration = 0f;           //active time for an attack
    public float attackWindUp = 0f;             //time between key input and active frames
    public float timeBetweenAttacks = 0f;       //delay between the end of one attack and the windup of another
    public float attackHitBoxDist = 0f;         //distance between center of player's gameObject and center of Hitbox.  Does not affect hitBox size or shape
    private bool canAttack = true;              //can the player attack this frame.  you cannot attack while dashing
    private bool isAttacking = false;

    public float dashDuration = 0f;             //duration of dash
    private float dashTimer = 0f;
    public float dashAcceleration = 0f;         //acceleration of dash
    public float dashMaxVelocity = 0f;          //maximum velocity during dash
    public float dashCooldown = 0f;             //cooldown between dashes
    private float dashCooldownTimer = 0f;
    public float minDashTime = 0;               //minimum time before player can cancel dash
    private float minDashTimer;
    private bool canDash = true;                //can the player dash this frame.  you cannot dash while attacking
    [HideInInspector]
    public bool isDashing = false;
    private bool preventCooldown = false;           //if true, player must touch the ground before a dash can be fully reset.
                                                   

    public float lungeDuration = 0f;             //duration of dash
    public float lungeAcceleration = 0f;         //acceleration of dash
    public float lungeMaxVelocity = 0f;          //maximum velocity during dash
    public float lungeCooldown = 0f;             //cooldown between dashes

    public float lungeHoverDuration;
    private float hoverTimer;
    private bool isLungeAttacking = false;
    [Range(0.707f, 1f)]
    public float lungeDiagonalScale = 1f;       //when set to 1, diagonal lunges move at Rad(2) * speed (which is the distance of a (1,1) vector)
                                                //this value changes that speed to the distance of a (lungeDiagonalScale,lungeDiagonalScale) vector
                                                //distance of (0.707,0.707) is 1
                                                //distance of (1,1) is 1.414
                                                //basically diagonal lunges go farther if this is higher, relative to flat lunges
    public float grappleMoveSpeed;
    public float grappleMaxDistance;
    public LayerMask grappleLayer;
    public float weakVelocityRestrictionRate = 0f;      //used to gradually reduce player to maximum speed
    public float strongVelocityRestrictionRate = 0f;  //used to rapidly and near instantly reduce player to maximum speed

    public float instantDropDistance = 0.2f;    //instantly moves player down this distance when dropping through a platform.  If the character does not drop, increase this value

    private float xVelocity = 0;                //-maxHorizontalVelocity < xVelocity < maxHorizontalVelocity
    private float yVelocity = 0;                //-maxVerticalVelocity < yVelocity < maxVerticalVelocity
    private bool canJump = false;               //can the player jump
    private bool jumping = false;               //is the player current jumping (jumping has different physics than general gravity)
    private bool jumpKeyHeld = false;           //tracks if the player was holding the jump key last frame, to run code when the player releases it
    private float jumpApexTimer = 0;            //time until char reaches apex of jump.  value based on jumpForce

    private Vector3 forcedMoveVector;           //this vector is the direction of a dash/lungedash
    int enemyHits = 0;                          //# of enemies hit in a single attack

    public enum Direction
    {
        Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
    }
    private Direction aimDirection;                                 //NESW direction based on held keys (8 directions)
    private Direction dashDirection;
    private Direction facingDirection = Direction.Right;            //character is facing left or right (character can face a direction with no input)

    private HitBoxReport attackHitBoxReport;                        //activates hitbox for attacks and reports collisions
    private SpriteRenderer spriteRenderer;

    private bool wasGrounded;

	private Vector3 respawnPos;                                     //move player to this position when spawning
    private bool wasMovingHorizontal = false;
    private bool isMovingHorizontal = false;                                  //actually requires movement.  running into a wall will not cause this to be true;
    private bool isMovingVertical = false;                          //actually requires movement

    private AudioSource source;
    public AudioSource soundEffectPlayer;

    public AudioClip attackSound;
    public AudioClip dashSound;
    public AudioClip lungeSound;
    public AudioClip jumpSound;
    public AudioClip footstepSound;
    public AudioClip climbSound;
    public AudioClip hitTakenSound;
    public AudioClip landingSound;
    public AudioClip dashCooldownSound;
    public AudioClip wallCollisionSound;
    public AudioClip deathSound;
    public AudioClip spawnSound;

    private IEnumerator fadeSound;
    public float footstepSoundFadeDuration = 0f;
    private bool stopStepping = false;
    public float jumpInAirDuration;                                 //lets player jump for VERY short time after falling off a platform.  
    private float jumpInAirTimer;
    public float ladderClimbSpeed;                                  //speed that the player goes up and down ladders
    private Bounds ladderBounds;                                    //has the rectangular bounds of the last ladder touched
    public float ladderGrabCooldown;                                //after you get off a ladder, prevent you from getting on another for duration
                                                                    //this is so you don't get on the ladder a frame after you got off
    private float ladderGrabTimer = 0;      
    private bool inLadder = false;                                  //are you touching a ladder

    public bool attacksEndDashes = false;                           //should attacks end dashes?
    public bool moveToEnemyOnHit = false;
    private bool playCooldownSound = false;

    public float knockbackBackVelocity;                             //player's x velocity is set to this when knocked back
    public float knockbackUpVelocity;                               //player's y velocity is set to this when knocked back
    public float hitStunDuration;                                   //player does not accept input for this duration (after being hit)
    public float hitInvincibilityDuration;                          //duration for the hitstun period
    private bool invulnerable = false;                              //can the player be damaged (forceDamagePlayer(int x) will bypass this (eg spikes))
    private bool landedThisFrame = false;
    private bool playerHitThisFrame = false;

    public float axisDeadZone = 0.1f;                              //general tolerance for axis-related code (used both as an upper and lower bound threshold)
    private bool flipSwing = false;
    public float spawnAnimDuration = .2f;
    private bool inSpawnAnimation = false;
    private Transform grapplePoint;
    public LightningLine line;
    private RaycastHit2D[] debugHits;
    private SceneController sceneController;
    private PlayerInputHandler pInput;
    private Vector3 posLastFrameForGrapple;                     //grapple code needs to manually mess with this value, so it is a separate variable from posLastFrame
    private Vector2 movementVector;
    public Transform aimIndicator;
    private void Awake()
    {
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
        charController = gameObject.GetComponent<Prime31.CharacterController2D>();
        attackHitBoxReport = GetComponentInChildren<HitBoxReport>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        pInput = GetComponent<PlayerInputHandler>();

    }
    void Start()
    {
        maxHealth = health;
        transform.position = Checkpoint.GetCurrentCheckpointPos();
        charController.warpToGrounded();
        if (GameState.compareState(GameState.State.InGame))
        {
            StartCoroutine(spawnPlayer(spawnAnimDuration));
        }
        allowPlayerInput = true;
        fadeSound = fadeAndStop(footstepSoundFadeDuration,source);
        //charController.warpToGrounded();
        debugHits = new RaycastHit2D[0];
        hoverTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
   
        if (playerDead || GameState.compareState(GameState.State.LevelWon))
        {
            movementState = MovementState.Paralyzed;
            source.volume = 0;
        }
        playerHitThisFrame = false;
        landedThisFrame = false;
        
        if(!GameState.compareState(GameState.State.InGame))
        {
            movementState = MovementState.Paralyzed;
        }
        if (charController.isGrounded)
        {
            canJump = true;
            preventCooldown = false;
        }
        if (PauseMenuController.paused)
        {
            source.Stop();
        }
        if (!PauseMenuController.paused)
        {

            
            switch (movementState)
            {
                case MovementState.Free:
                    updateGrappling();
                    if (pInput.jumpButton(PlayerInputHandler.Action.Down))
                    {
                        if ((charController.isGrounded || jumpInAirTimer > 0) && !jumping && canJump)
                        {
                            jump();
                        }
                    }
                   
                    if (isMovingHorizontal && isGrounded())
                    {
                        //For playing footsteps and for fading out the footstep sound gradually
                        source.clip = footstepSound;
                        if (!source.isPlaying)
                        {
                            source.volume = 1;
                            source.Play();
                            source.loop = true;
                            stopStepping = false;
                        }
                    }
                    else
                    {
                        fadeFootsteps();
                        stopStepping = true;

                    }
                    if (pInput.maxedYAxisThisFrame(Direction.Down))
                    {
                        //To fall through one way platforms
                        attemptDropThroughPlatform();
                    }
                    if (pInput.dashButton(PlayerInputHandler.Action.Down) && canDash)
                    {
                        source.Stop();

                        dash(facingDirection);
                    }
                    checkForAttackInput();
                    if ((((pInput.maxedYAxisThisFrame(Direction.Up)) && isGrounded())                  //For grabbing ladders
                        || ((pInput.maxedYAxis() && ladderGrabTimer <= 0)))
                        && movementState == MovementState.Free
                        && inLadder)
                    {
                        movementState = MovementState.OnLadder;
                        transform.position = new Vector3(ladderBounds.center.x, transform.position.y, transform.position.z);

                        charController.ignoreOneWayPlatformsThisFrame = true;
                        if (pInput.maxedYAxisThisFrame(Direction.Down))
                        {
                            attemptDropThroughPlatform();
                        }
                        xVelocity = 0;


                    }
                    

                    break;
                case MovementState.Paralyzed:
                    if (playerDead && pInput.restartButton(PlayerInputHandler.Action.Down))
                    {
                        respawn();
                    }
                    if (debugColors)
                        spriteRenderer.color = Color.black;
                    break;
                case MovementState.OnLadder:
                    updateGrappling();
                    updateLadderMovement();
                    canJump = true;
                    preventCooldown = false;
                    break;
                case MovementState.Dash:
                    checkForAttackInput();
                    if (pInput.jumpButton(PlayerInputHandler.Action.Down)
                        && (charController.isGrounded || jumpInAirTimer > 0 || charController.isMovingUpSlope()) 
                        && !jumping && canJump)
                    {
                        jump();
                        endDash();
                    }
                    if (wasGrounded && !isGrounded() && !jumping)       //true when you start a dash on ground, then end it in the air
                    {
                        jumpInAirTimer = jumpInAirDuration;
                    }

                    break;
                case MovementState.Lunge:

                    break;
                case MovementState.Hover:
                    if (isGrounded())
                    {
                        //If you are touching the ground, end hover state and move to free state
                        movementState = MovementState.Free;
                        endHover();
                        break;
                    }
                    if (pInput.dashButton(PlayerInputHandler.Action.Down) && canDash)
                    {
                        endHover();
                        dash(facingDirection);
                        break;
                    }
                    if(hoverTimer > 0)
                    {
                        hoverTimer -= Time.deltaTime;
                    } else
                    {
                        endHover();
                        movementState = MovementState.Free;
                        break;
                    }
                    checkForAttackInput();
                    break;
                case MovementState.Grappled:
                    updateGrappling();
                    break;
                case MovementState.Clinging:
                    checkForAttackInput();
                    updateGrappling();
                    
                    break;
            }
            if (ladderGrabTimer > 0)
            {
                ladderGrabTimer -= Time.deltaTime;
            }

            wasGrounded = isGrounded();
            updateDashing();
            updateHorizontalVelocity();
            updateVerticalVelocity();
            updatePosition();
            updateDirections();
            wasMovingHorizontal = isMovingHorizontal;
        }
    }
    private void updateGrappling()
    {
        switch (movementState)
        {
            case MovementState.Free:
                grappleRaycast();
                if (Input.GetKeyDown(KeyCode.K))
                {
                    grapple();
                }
                break;
            case MovementState.OnLadder:
                grappleRaycast();
                if (Input.GetKeyDown(KeyCode.K))
                {
                    grapple();
                }
                break;
            case MovementState.Grappled:
                line.setIsDrawing(true);
                if (Vector2.Distance(grapplePoint.position, transform.position) < grappleMoveSpeed * Time.deltaTime)
                {
                    //if reached grapplePoint
                    invulnerable = false;
                    movementState = MovementState.Clinging;
                }
                else if (Vector2.Distance(posLastFrameForGrapple, transform.position) < grappleMoveSpeed * Time.deltaTime / 3)
                {
                    //Run if the player is supposed to be moving, but isnt
                    //Detects if the player is stuck
                    line.setIsDrawing(false);
                    invulnerable = false;
                    movementState = MovementState.Free;
                }
                else
                {
                    //set vector to move by
                    forcedMoveVector = ((Vector2)(grapplePoint.position - transform.position)).normalized * grappleMoveSpeed;

                }
                posLastFrameForGrapple = transform.position;
                break;
            case MovementState.Clinging:
                grappleRaycast();
                line.setIsDrawing(false);
                if (pInput.jumpButton(PlayerInputHandler.Action.Down))
                {
                    //if jump, jump
                    movementState = MovementState.Free;
                    jump();
                }
                else if (pInput.dashButton(PlayerInputHandler.Action.Down))
                {
                    //if dash, dash
                    dash(facingDirection);
                }
                else if (pInput.grappleButton(PlayerInputHandler.Action.Down))
                {
                    //player can grapple
                    grapple();
                }
                break;

        }
    }
    private void checkForAttackInput()
    {


        if (pInput.lungeButton(PlayerInputHandler.Action.Down)
            || (pInput.maxedRightStickThisFrame() && GameState.controlLayout == 0 
            && PlayerInputHandler.controlType == PlayerInputHandler.ControlType.Controller))
        {
            if (canAttack)
            {
                source.Stop();

                aimDirection = pInput.calculateAimDirection(aimDirection);
                dashDirection = aimDirection;
                StartCoroutine(lungeAttack(getAimVector(aimDirection).normalized));
                if (attacksEndDashes)
                {
                    endDash();
                }
            }
        }
    }
    private void moveHorizontal(float xV)
    {
        bool grounded = isGrounded();
        isMovingHorizontal = true;
        Vector3 position = transform.position;

        charController.move(new Vector2(xV, 0));
        if (xV != 0 && Vector3.Distance(position, transform.position) < 0.0005f)          //I don't know how to check for wall collisions in CharacterController2D so this is a janky workaround
        {
            xVelocity = 0;
            isMovingHorizontal = false;
        }
        if(Mathf.Abs(xV) < 0.000005f)
        {
            isMovingHorizontal = false; //tracks if the player is actually moving
        }
        if (wasMovingHorizontal && !isMovingHorizontal && Mathf.Abs(xV) > 0.000005f && grounded && movementState == MovementState.Free)
        {
            //true when hitting walls
            soundEffectPlayer.PlayOneShot(wallCollisionSound);
        }
        movementVector = new Vector2(transform.position.x - position.x, movementVector.y);
    }
    private void moveVertical(float yV)
    {
        isMovingVertical = true;
        Vector3 position = transform.position;
        charController.move(new Vector2(0, yV));
        if (yV > 0 && transform.position.y <= position.y)          //I don't know how to check for wall collisions in CharacterController2D so this is a janky workaround
        {
            yVelocity = 0;
        }
        if (yV != 0 && Vector3.Distance(position, transform.position) < 0.0005f)          //I don't know how to check for wall collisions in CharacterController2D so this is a janky workaround
        {
            isMovingVertical = false;
        }
        if (Mathf.Abs(yV) < 0.000005f)
        {
            isMovingVertical = false;
        }
        if (movementState == MovementState.Free)
        {
            if (wasGrounded && !isGrounded() && !jumping)   //if you fall off of a ledge, your yVelocity should be zero at that instant
            {
                yVelocity = 0;
                transform.position = position;
                jumpInAirTimer = jumpInAirDuration;
            }
        }
        if (!wasGrounded && isGrounded())                     //runs code when landing
        {
            soundEffectPlayer.PlayOneShot(landingSound);
            landedThisFrame = true;
        }
        movementVector = new Vector2(movementVector.x, transform.position.y - position.y);
    }
    private void updatePosition()
    {
        moveHorizontal(xVelocity * Time.deltaTime);
        moveVertical(yVelocity * Time.deltaTime);
    }
    private void updateHorizontalVelocity()
    {
        float restrictRate;
        float xVelToAdd = 0f;

        switch (movementState)
        {
            case (MovementState.Free):
                //regular movement, not dashing
                if (pInput.holdingDirection(Direction.Left))
                {
                    if (xVelocity <= 0)
                    {
                        //accelerating left
                        xVelToAdd = (-acceleration) * Time.deltaTime;
                    }
                    else
                    {
                        //decelerating left
                        xVelToAdd = (-deceleration) * Time.deltaTime;
                    }
                }
                else if (pInput.holdingDirection(Direction.Right))
                {
                    if (xVelocity >= 0)
                    {
                        //accelerating right
                        xVelToAdd = acceleration * Time.deltaTime;
                    }
                    else
                    {
                        //decelerating right
                        xVelToAdd = deceleration * Time.deltaTime;
                    }
                }
                else
                {
                    //not moving left or right.  slow down through passive deceleration
                    if (xVelocity > 0)
                    {
                        xVelToAdd = (-passiveDeceleration) * Time.deltaTime;
                        if (Mathf.Abs(xVelocity) < highDecelerationThreshold)
                            xVelToAdd *= highDecelerationFactor;
                        xVelToAdd = (xVelocity + xVelToAdd < 0) ? -xVelocity : xVelToAdd;    //make you stop instead of reverse direction
                    }
                    else if (xVelocity < 0)
                    {
                        xVelToAdd = passiveDeceleration * Time.deltaTime;
                        if (Mathf.Abs(xVelocity) < highDecelerationThreshold)
                            xVelToAdd *= highDecelerationFactor;
                        xVelToAdd = (xVelocity + xVelToAdd > 0) ? -xVelocity : xVelToAdd;    //set velocity to zero instead of reversing direction
                    }
                }
                if (!charController.isGrounded)
                {
                    xVelToAdd *= airAccelerationFactor;
                    restrictRate = acceleration * airAccelerationFactor + weakVelocityRestrictionRate;
                } else
                {
                    restrictRate = acceleration+weakVelocityRestrictionRate;

                }
                //if char is in the air, it would add acceleration * airAccelerationFactor * time.deltaTime;

                //clamp to speed maximums
                xVelocity = xVelocity + xVelToAdd;
                if (xVelocity > maxHorizontalVelocity)           //gradually decrease speed to max speed
                {
                    xVelocity += -restrictRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += restrictRate * Time.deltaTime;
                    xVelocity = Mathf.Min(xVelocity, -maxHorizontalVelocity);
                }
                    
                
                break;
            case (MovementState.Dash):
                switch (facingDirection)
                {
                    case Direction.Right:
                        xVelocity += dashAcceleration * Time.deltaTime;
                        xVelocity = Mathf.Min(xVelocity, dashMaxVelocity);
                        //cap max speed, else increase it
                        break;
                    case Direction.Left:
                        xVelocity += (-dashAcceleration) * Time.deltaTime;
                        xVelocity = Mathf.Max(xVelocity, -dashMaxVelocity);
                        //cap max speed, else increase it
                        break;
                }
                break;
            case (MovementState.Lunge):
                xVelocity += forcedMoveVector.x * lungeAcceleration * Time.deltaTime;
                xVelocity = Mathf.Clamp(xVelocity, -lungeMaxVelocity, lungeMaxVelocity);
                break;
            case (MovementState.Paralyzed):
                if(isGrounded())
                    xVelocity *= 0.5f;
                if (xVelocity > maxHorizontalVelocity)           //gradually decrease speed to max speed
                {
                    xVelocity += -strongVelocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += strongVelocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Min(xVelocity, -maxHorizontalVelocity);
                }
                break;
            case (MovementState.Hover):
                /*
                if (pInput.holdingDirection(Direction.Left))
                {
                    if (xVelocity <= 0)
                    {
                        //accelerating left
                        xVelToAdd = (-acceleration) * Time.deltaTime;
                    }
                    else
                    {
                        //decelerating left
                        xVelToAdd = (-deceleration) * Time.deltaTime;
                    }
                }
                else if (pInput.holdingDirection(Direction.Right))
                {
                    if (xVelocity >= 0)
                    {
                        //accelerating right
                        xVelToAdd = acceleration * Time.deltaTime;
                    }
                    else
                    {
                        //decelerating right
                        xVelToAdd = deceleration * Time.deltaTime;
                    }
                }
                else
                {
                    //not moving left or right.  slow down through passive deceleration
                    if (xVelocity > 0)
                    {
                        xVelToAdd = (-passiveDeceleration) * Time.deltaTime;
                        if (Mathf.Abs(xVelocity) < highDecelerationThreshold)
                            xVelToAdd *= highDecelerationFactor;
                        xVelToAdd = (xVelocity + xVelToAdd < 0) ? -xVelocity : xVelToAdd;    //make you stop instead of reverse direction
                    }
                    else if (xVelocity < 0)
                    {
                        xVelToAdd = passiveDeceleration * Time.deltaTime;
                        if (Mathf.Abs(xVelocity) < highDecelerationThreshold)
                            xVelToAdd *= highDecelerationFactor;
                        xVelToAdd = (xVelocity + xVelToAdd > 0) ? -xVelocity : xVelToAdd;    //set velocity to zero instead of reversing direction
                    }
                }
                
                if (!charController.isGrounded)
                {
                    xVelToAdd *= airAccelerationFactor;
                    restrictRate = acceleration * airAccelerationFactor + strongVelocityRestrictionRate;
                } else
                {
                    restrictRate = acceleration + strongVelocityRestrictionRate;

                }
                //if char is in the air, it would add acceleration * airAccelerationFactor * time.deltaTime;



                //clamp to speed maximums
                xVelocity = xVelocity + xVelToAdd;
                if (xVelocity > maxHorizontalVelocity)           //gradually decrease speed to max speed
                {
                    xVelocity += -restrictRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += restrictRate * Time.deltaTime;
                    xVelocity = Mathf.Min(xVelocity, -maxHorizontalVelocity);
                }
                */
                restrictRate = 3f;
                xVelocity += -xVelocity * restrictRate * Time.deltaTime;

                break;
            case MovementState.Grappled:
                xVelocity = forcedMoveVector.x;
                break;
            case MovementState.Clinging:
                xVelocity = 0;
                break;
        }


    }
    private void updateVerticalVelocity()
    {
        float restrictRate;
        float yVelToAdd = 0;
        switch (movementState)
        {
            case (MovementState.Free):
                if (!isGrounded())
                {
                   
                    yVelToAdd = (-gravity) * Time.deltaTime;
                    
                }
                if (jumping)
                {
                    if (jumpApexTimer > 0)
                    {
                        //if this is >0, then character is rising until apex.
                        jumpApexTimer -= Time.deltaTime;
                        if (!(pInput.jumpButton(PlayerInputHandler.Action.Pressed)) 
                            && jumpKeyHeld)
                        {
                            //you release the jump key before you reach the apex
                            jumpApexTimer = 0;
                            yVelocity *= jumpButtonReleaseFactor;
                        }
                        jumpKeyHeld = pInput.jumpButton(PlayerInputHandler.Action.Pressed);
                        if (yVelocity < 0)
                        {
                            //you hit the ceiling or something, so the apex timer is not applicable anymore
                            jumpApexTimer = 0;
                        }
                    }
                    else
                    {
                        //once you hit the apex, its back to regular falling
                        jumpApexTimer = 0;
                        jumping = false;
                    }
                }
                if (jumpInAirTimer > 0)
                    jumpInAirTimer -= Time.deltaTime;
                else
                    jumpInAirTimer = 0;
                if (isGrounded() && !jumping)
                {
                    yVelocity = -25f;       //downward force so you stick to slopes
                }
                else
                {
                    /*
                    restrictRate = acceleration + strongVelocityRestrictionRate;
                    if(yVelocity > maxVerticalVelocity)
                    {
                        yVelocity += -restrictRate * Time.deltaTime;
                        yVelocity = Mathf.Max(yVelocity, maxVerticalVelocity);
                    }
                    else if (yVelocity > -maxVerticalVelocity*fastFallMultiplier || Mathf.Abs(yVelocity + yVelToAdd) < Mathf.Abs(yVelocity))
                    {
                        yVelocity = yVelocity + yVelToAdd;      //set speeds
                                                                //Do not change if you are over maximum velocity, UNLESS it would slow you down
                                                                //do not change if you are falling too fast
                    }
                    */
                    yVelocity += yVelToAdd;
                    yVelocity = Mathf.Clamp(yVelocity, -maxVerticalVelocity, maxVerticalVelocity);
                   
                }
                break;
            case (MovementState.Dash):
                if (isDashing)           //ignore gravity during dash
                {
                    yVelocity = -0.001f;    //ensure you stay grounded
                    jumping = false;        //cancel jump physics
                    jumpApexTimer = 0;
                }
                if (jumpInAirTimer > 0)
                    jumpInAirTimer -= Time.deltaTime;
                else
                    jumpInAirTimer = 0;
                break;
            case (MovementState.Lunge):
                yVelocity += forcedMoveVector.y * lungeAcceleration * Time.deltaTime;
                yVelocity = Mathf.Clamp(yVelocity, -lungeMaxVelocity, lungeMaxVelocity);
                jumping = false;
                jumpApexTimer = 0;
                break;
            case (MovementState.Paralyzed):
                if (!isGrounded())
                {
                    yVelToAdd = (-gravity) * Time.deltaTime;
                }
                
                yVelocity += yVelToAdd;
                yVelocity = Mathf.Clamp(yVelocity, -maxVerticalVelocity, maxVerticalVelocity);
                if (isGrounded() && !jumping)
                {
                    yVelocity = -25f;       //downward force so you stick to slopes
                }
                break;
            case (MovementState.Hover):
                /*
                restrictRate = acceleration+strongVelocityRestrictionRate;
                if (yVelocity > 0)           //gradually decrease speed to 0
                {
                    yVelocity += -restrictRate * Time.deltaTime;
                    yVelocity = Mathf.Max(yVelocity, 0);
                }
                else if (yVelocity < 0)
                {
                    yVelocity += restrictRate * Time.deltaTime;
                    yVelocity = Mathf.Min(yVelocity, 0);
                }
                */
                restrictRate = 3;
                yVelocity += -yVelocity * restrictRate * Time.deltaTime;
                break;
            case (MovementState.Grappled):
                yVelocity = forcedMoveVector.y;
                break;
            case (MovementState.Clinging):
                yVelocity = 0;
                break;
        }






    }
    private void updateDashing()
    {

        switch (movementState)
        {
            case (MovementState.Dash):
                if (dashTimer > 0)
                {
                    dashTimer -= Time.deltaTime;
                    if(debugColors)
                        spriteRenderer.color = Color.magenta;
                }
                else
                {
                    endDash();
                }
                if(minDashTimer > 0)
                {
                    minDashTimer -= Time.deltaTime;
                } else
                {
                    canAttack = true;
                }
                break;
            case (MovementState.Lunge):
                if (dashTimer > 0)
                {
                    dashTimer -= Time.deltaTime;
                    if(debugColors)
                        spriteRenderer.color = Color.magenta;
                }
                else
                {
                    endLunge();
                }
                break;
            case (MovementState.Free):
                if (!canDash)
                {
                    if (dashCooldownTimer > 0)
                    {
                        dashCooldownTimer -= Time.deltaTime;
                    }
                    else if(!preventCooldown)       //do not finish cooldown until you are grounded
                    {
                        dashCooldownTimer = 0;
                        if (playCooldownSound)
                        {
                            soundEffectPlayer.PlayOneShot(dashCooldownSound);
                            playCooldownSound = false;
                        }
                        canDash = true;
                        if(debugColors)
                            spriteRenderer.color = Color.yellow;
                    }
                }
                break;
            case (MovementState.Hover):
                
                break;
        
        }
    
    }
    private void updateLadderMovement()
    {
        charController.ignoreOneWayPlatformsThisFrame = true;
        xVelocity = 0;
        if (debugColors)
            spriteRenderer.color = Color.green;
        transform.position = new Vector3(ladderBounds.center.x, transform.position.y, transform.position.z);    //ensure player is centered on ladder
        source.clip = climbSound;
        if (pInput.jumpButton(PlayerInputHandler.Action.Down))
        {
            //jump off of ladder
            movementState = MovementState.Free;
            ladderGrabTimer = ladderGrabCooldown;
            if (debugColors)
                spriteRenderer.color = Color.yellow;
            if (!pInput.holdingDirection(Direction.Down))
            {
                jump();
            }
        } else if (pInput.holdingDirection(Direction.Up))
        {
            //climb up
            if (!source.isPlaying)
            {
                source.clip = climbSound;
                source.volume = 1;
                source.Play();
                source.loop = true;
                stopStepping = false;
            }
            yVelocity = ladderClimbSpeed;
        } else if (pInput.holdingDirection(Direction.Down))
        {
            //climb down
            if (isGrounded())
            {
                movementState = MovementState.Free;
                ladderGrabTimer = ladderGrabCooldown / 2;
                if (debugColors)
                    spriteRenderer.color = Color.yellow;
                yVelocity = 0;
            }
            else
            {
                yVelocity = -ladderClimbSpeed;
                if (!source.isPlaying)
                {
                    source.clip = climbSound;
                    source.volume = 1;
                    source.Play();
                    source.loop = true;
                    stopStepping = false;
                }
            }

        } else
        {
            yVelocity = 0;
            fadeFootsteps();
            stopStepping = true;
        }
        
        if (!ladderBounds.Contains(transform.position))
        {
            //if you go off top or bottom of ladder
            movementState = MovementState.Free;
            ladderGrabTimer = ladderGrabCooldown;
            if (debugColors)
                spriteRenderer.color = Color.yellow;
            yVelocity = 0;
        }
        
    }


    private void jump()
    {
        if (movementState == MovementState.Free || movementState == MovementState.Dash)
        {
            yVelocity = jumpForce;
            canJump = false;
            jumping = true;
            jumpApexTimer = jumpForce / gravity;
            soundEffectPlayer.PlayOneShot(jumpSound);
            jumpInAirTimer = 0;
        }
    }
    private void attemptDropThroughPlatform()
    {
        if (movementState == MovementState.OnLadder || (movementState == MovementState.Free && isGrounded()))
        {
            yVelocity = 0;
            charController.ignoreOneWayPlatformsThisFrame = true;
            charController.move(Vector3.down * instantDropDistance);
        }
    }
    public bool isGrounded()
    {
        return charController.isGrounded;
    }
    private void updateDirections()
    {
        if (movementState == MovementState.Free || movementState == MovementState.OnLadder || movementState == MovementState.Hover || movementState == MovementState.Clinging)                             //you cannot change your facing direction while dashing
        {
            
                if (pInput.holdingDirection(Direction.Right))
                    facingDirection = Direction.Right;
                else if (pInput.holdingDirection(Direction.Left))
                    facingDirection = Direction.Left;
            
        } else if(movementState == MovementState.Grappled)
        {
            facingDirection = (transform.position.x < grapplePoint.position.x) ? Direction.Right : Direction.Left;
        }
        
            aimDirection = pInput.calculateAimDirection(aimDirection);
 
            
        

    }
    
    private Vector3 getAimVector(Direction dir)
    {
        Vector3 result = Vector3.zero;
        switch (dir)
        {
            case Direction.Up:
                result = new Vector3(0, 1, 0);
                break;
            case Direction.UpRight:
                result = new Vector3(lungeDiagonalScale, lungeDiagonalScale, 0);    
                break;
            case Direction.Right:
                result = new Vector3(1, 0, 0);
                break;
            case Direction.DownRight:
                result = new Vector3(lungeDiagonalScale, -lungeDiagonalScale, 0);
                break;
            case Direction.Down:
                result = new Vector3(0, -1, 0);
                break;
            case Direction.DownLeft:
                result = new Vector3(-lungeDiagonalScale, -lungeDiagonalScale, 0);
                break;
            case Direction.Left:
                result = new Vector3(-1, 0, 0);
                break;
            case Direction.UpLeft:
                result = new Vector3(-lungeDiagonalScale, lungeDiagonalScale, 0);
                break;
        }
        return result;
    }
    private void endDash()
    {
        //terminate a dash
        dashTimer = 0;
        minDashTimer = 0;
        dashCooldownTimer = dashCooldown;
        isDashing = false;
        movementState = MovementState.Free;
        if(debugColors)
            spriteRenderer.color = Color.gray;
    }
    private void endLunge()
    {
        //terminate a lunge
        xVelocity = Mathf.Clamp(xVelocity, -maxHorizontalVelocity, maxHorizontalVelocity);
        invulnerable = false;
        dashTimer = 0;
        isDashing = false;
        if(debugColors)
            spriteRenderer.color = Color.yellow;
        canAttack = true;
        canDash = true;
        movementState = MovementState.Hover;
        hoverTimer = lungeHoverDuration;
        dashCooldownTimer = 0;
    }
    private void endHover()
    {
        hoverTimer = 0;
    }
    void onHitBoxCollision(Collider2D other)
    {
        //runs if the child hitbox for the sword intersects something on the "TriggerLayer"
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().damageEnemy(1, other.transform.position - transform.position);
            ++enemyHits;
            if (enemyHits == 1 && isLungeAttacking && movementState == MovementState.Free)      //Only activate the movement portion of the lunge ONCE
            {
                if(moveToEnemyOnHit)
                    transform.position = other.transform.position;
                lungeDash(getAimVector(dashDirection));
                isLungeAttacking = false;
                for (int i = 0; i < 3; i++)
                {
                    spawnHitProjectile(other.transform.position);
                }
            }
        } else if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            other.gameObject.GetComponent<Bullet>().hitByPlayer();
            ++enemyHits;
            if (enemyHits == 1 && isLungeAttacking && movementState == MovementState.Free)
            {
                if(moveToEnemyOnHit)
                transform.position = other.transform.position;
                lungeDash(getAimVector(dashDirection));
                isLungeAttacking = false;
            }
        } else if (other.gameObject.CompareTag("HittableObject"))
        {
            other.gameObject.GetComponent<HittableObject>().hitByPlayer(transform.position);
            ++enemyHits;
            if (enemyHits == 1 && isLungeAttacking && movementState == MovementState.Free)
            {
                if (moveToEnemyOnHit)
                    transform.position = other.transform.position;
                lungeDash(getAimVector(dashDirection));
                isLungeAttacking = false;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.CompareTag("Checkpoint"))
		{
            health = maxHealth;
			respawnPos = Checkpoint.GetCurrentCheckpointPos ();
		}
   		else if(collision.gameObject.CompareTag("Spike")) 
        {
            forceDamagePlayer(1000000000);       //definitely kill the player
        } 
        else if (collision.gameObject.CompareTag("Ladder"))
        {
            inLadder = true;
            ladderBounds = collision.bounds;
            ladderBounds.center = new Vector3(ladderBounds.center.x, ladderBounds.center.y, transform.position.z);      //put bounds on same z position as player transform
        }
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            forceDamagePlayer(1000000000);       //definitely kill the player
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            inLadder = false;
            
        }
    }
    private IEnumerator attack(Vector3 aimVector)
    {
        isAttacking = true;
        canAttack = false;
        canDash = false;
       
        dashCooldownTimer = attackWindUp + attackDuration;
        //for debug
        if(debugColors)
          spriteRenderer.color = Color.cyan;                                                  //windup: cyan
        
        yield return new WaitForSeconds(attackWindUp);

        spawnAttackTrail(aimDirection);
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg*Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
        if(debugColors)
          spriteRenderer.color = Color.red;                                                   //active frames: red
        soundEffectPlayer.PlayOneShot(attackSound);

        yield return new WaitForSeconds(attackDuration); 
        
        if(debugColors)
          spriteRenderer.color = Color.gray;                                                  //cooldown: gray
        isAttacking = false;

        yield return new WaitForSeconds(timeBetweenAttacks);

        if(debugColors)
          spriteRenderer.color = Color.yellow;                                                //default: yellow
        canAttack = true;
    }
    private IEnumerator lungeAttack(Vector3 aimVector)
    {
        isLungeAttacking = true;
        canAttack = false;
        canDash = false;
        dashCooldownTimer = attackWindUp + attackDuration;
        if (debugColors)
            spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(attackWindUp);

        if(movementState != MovementState.Free)
        {
            canAttack = true;
            yield break;
        }
        spawnAttackTrail(aimDirection);

        enemyHits = 0;
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg * Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
        if (debugColors)
            spriteRenderer.color = Color.red;
        soundEffectPlayer.PlayOneShot(attackSound);
        
        yield return new WaitForSeconds(attackDuration);

        if (debugColors)
            spriteRenderer.color = Color.gray;

        yield return new WaitForEndOfFrame();

        isLungeAttacking = false;

        yield return new WaitForSeconds(timeBetweenAttacks);

        if (debugColors)
            spriteRenderer.color = Color.yellow;
        canAttack = true;

    }
    private void dash(Vector3 direction)
    {

        spawnTrail(1000);
        forcedMoveVector = direction;
        canDash = false;
        isDashing = true;
        //canAttack = false;
        dashTimer = dashDuration;
        minDashTimer = minDashTime;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
        soundEffectPlayer.PlayOneShot(dashSound);
        playCooldownSound = true;
        if (!isGrounded())
            preventCooldown = true;

    }
    private void dash(Direction direction)
    {
        spawnTrail(1000);
        minDashTimer = minDashTime;
        forcedMoveVector = getAimVector(direction);
        canDash = false;
        isDashing = true;
        dashTimer = dashDuration;
        //canAttack = false;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
        soundEffectPlayer.PlayOneShot(dashSound);
        playCooldownSound = true;

        if (!isGrounded())
            preventCooldown = true;
    }
    private void lungeDash(Vector3 direction)
    {
        spawnTrail(2000);
        forcedMoveVector = direction;
        canDash = false;
        isDashing = true;
        dashTimer = lungeDuration;
        dashCooldownTimer = lungeDuration;
        movementState = MovementState.Lunge;
        xVelocity = forcedMoveVector.x * lungeMaxVelocity;
        yVelocity = forcedMoveVector.y * lungeMaxVelocity;
        soundEffectPlayer.PlayOneShot(lungeSound);
        invulnerable = true;
        preventCooldown = false;
    }
    private void grapple()
    {
        if (grappleRaycast())
        {
            canJump = true;
            canDash = true;
            invulnerable = true;
            movementState = MovementState.Grappled;
            posLastFrameForGrapple = new Vector3(0, 0, -10000);
        }
        //grapple state ends if you are supposed to move, but are not moving
        //This should never be true on the first frame of grapple, thus we do this
    }
    private bool grappleRaycast()
    {
        //Return true if valid grapple point is found
        //Does not actually cause the player to change state
        //also shows player what they will grapple to
        RaycastHit2D[] hits;
        LayerMask combinedMask = grappleLayer;
        LayerMask groundLayer = LayerMask.NameToLayer("groundLayer");
        grappleLayer |= (1 << groundLayer);
        Vector3 dir = new Vector3(Mathf.Cos(aimIndicator.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(aimIndicator.eulerAngles.z * Mathf.Deg2Rad));
        hits = Physics2D.RaycastAll(transform.position, dir, grappleMaxDistance, combinedMask);
        Transform closestGrapplePoint = null;
        
        float minGrappleDistance = 1;
        float minGroundDistance = 1;
        if (hits.Length > 0){
            debugHits = hits;   //debug information
            for(int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("grappleLayer"))         //if hit is on grappelayer
                {
                    if (hits[i].fraction > 0.01f)       //if the ray didn't start inside the object
                    {
                        
                        if (hits[i].fraction < minGrappleDistance)      //track smallest distance object
                        {
                            minGrappleDistance = hits[i].fraction;
                            closestGrapplePoint = hits[i].transform;
                        }
                    }
                } else if(hits[i].collider.gameObject.layer == groundLayer) //if hit is on groundlayer
                {
                    if(hits[i].fraction > 0.01f)                            //if the ray didn't start inside the object
                    {
                        if(hits[i].fraction < minGroundDistance)            //track smallest distance ground object
                        {
                            minGroundDistance = hits[i].fraction;
                        }
                    }
                }
                
            }
        }
        if(minGroundDistance < minGrappleDistance)              //if the ground was hit before a grapple object
        {
            closestGrapplePoint = null; //do not do grapple movement
        }
        if(closestGrapplePoint != null)
        {
            grapplePoint = closestGrapplePoint;         //set the grapple point to the raycast target
            line.endPointB = grapplePoint;              //set the line end point to the raycast target
            grapplePoint.gameObject.SendMessage("HitByRaycast");
            return true;
        }
        return false;

    }
    private void spawnTrail(float rotationSpeed)
    {
        GameObject trailObject = (GameObject)Instantiate(Resources.Load("DashTrail"));
        trailObject.transform.parent = transform;
        trailObject.transform.position = transform.position;
        trailObject.GetComponent<DashTrailScript>().direction = (facingDirection == Direction.Right) ? -1 : 1;
        trailObject.GetComponent<DashTrailScript>().rotationSpeed = rotationSpeed;
        foreach (TrailRenderer tr in trailObject.GetComponentsInChildren<TrailRenderer>())
        {
            //removes the trail that appears when displacing the dashTrail from default position to transform.position
            tr.Clear();
        }

    }
    private void spawnAttackTrail(Direction dir)
    {
        GameObject trailObject = (GameObject)Instantiate(Resources.Load("SwordSwing"));
        float startAngle = ((int)(dir)) * 45 - 45;
        Vector3 offset;
        if(!isGrounded())
            offset = new Vector3(xVelocity*.075f, yVelocity*0.05f) ;
        else
            offset = new Vector3(xVelocity*.075f, 0,0);
        flipSwing = (facingDirection == Direction.Left);
        trailObject.GetComponent<SwordTrail>().startSwing(1800, new Vector2(startAngle,startAngle+90),transform.position,offset,flipSwing);
    }
    private Vector3 rad2Vec(float rad)
    {
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    private float vecToRad(Vector3 vec)
    {
        return Mathf.Atan2(vec.y, vec.x);
    }
    private int rngSign()
    {
        return (Random.value > 0.5f) ? 1 :-1;
    }
    private void spawnHitProjectile(Vector3 enemyPos)
    {
        GameObject hitProjectile = (GameObject)Instantiate(Resources.Load("HitProjectile"),enemyPos,new Quaternion(0,0,0,0));


        float rngAngle = (Mathf.PI/2 * Random.value+Mathf.PI/4)*rngSign();      //radians
        rngAngle += vecToRad(transform.position - enemyPos);      //radians
        hitProjectile.transform.position = enemyPos + rad2Vec(rngAngle)*5;
        hitProjectile.GetComponent<ConstantMovement>().dirVector = -(Vector3)(Vector2)(hitProjectile.transform.position - enemyPos);

        hitProjectile.GetComponent<ConstantMovement>().speed = 90;
        hitProjectile.transform.rotation = Quaternion.Euler(new Vector3(0,0,vecToRad(hitProjectile.transform.position - enemyPos)*Mathf.Rad2Deg));

    }
    public void knockBackPlayer(Vector3 enemyPos)
    {
        //enemy MUST supply its own location
       
        if ((movementState != MovementState.Paralyzed) && !invulnerable)
        {
           
            yVelocity = knockbackUpVelocity;
            charController.move(new Vector2(0, .2f));
            if (transform.position.x < enemyPos.x)
            {
                xVelocity = -knockbackBackVelocity;
            }
            else
            {
                xVelocity = knockbackBackVelocity;
            }
            isDashing = false;
			isLungeAttacking = false;
            StartCoroutine(stunPlayer(hitStunDuration, hitInvincibilityDuration));
            source.Stop();
            soundEffectPlayer.PlayOneShot(hitTakenSound);
            playerHitThisFrame = true;
        }
        else if (playerDead)
        {
            yVelocity = knockbackUpVelocity;
            charController.move(new Vector2(0, .2f));
            if (transform.position.x < enemyPos.x)
            {
                xVelocity = -knockbackBackVelocity;
            }
            else
            {
                xVelocity = knockbackBackVelocity;
            }
        }
    }
    public IEnumerator stunPlayer(float duration, float invincibilityDuration)
    {
        movementState = MovementState.Paralyzed;
        invulnerable = true;
            spriteRenderer.color = Color.grey;

        yield return new WaitForSeconds(duration);
       
        if(!playerDead)
            movementState = MovementState.Free;
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(invincibilityDuration-duration);

        invulnerable = false;

    }
    public void attemptDamagePlayer(int dmg)
    {
        if(!invulnerable && !playerDead)
            setHealth(health - dmg);
        
    }
    public void forceDamagePlayer(int dmg)
    {
        if(!playerDead)
            setHealth(health - dmg);
        line.setIsDrawing(false);
    }
    private void setHealth(int hp)
    {
        health = hp;
        if (health <= 0 && !playerDead)
        {
            //this code runs only once when the player dies
            playerDead = true;
			isLungeAttacking = false;
            print("Player is Dead");
            allowPlayerInput = false;
            if (debugColors)
                spriteRenderer.color = Color.black;
            movementState = MovementState.Paralyzed;
            soundEffectPlayer.PlayOneShot(deathSound);
        }
    }
    private void respawn()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartCoroutine(sceneController.transitionThenLoad(.08f,false,scene.name));
    }
    public bool isDead()
    {
        return playerDead;
    }
    public void setMovementState(int x)
    {
        movementState = (MovementState)x;
    }
    public void fadeFootsteps()
    {
        if (stopStepping == false)
        {
            StopCoroutine(fadeSound);
            fadeSound = fadeAndStop(footstepSoundFadeDuration, source);
            StartCoroutine(fadeSound);
        }
    }
    
    private void startUniqueCoroutine(IEnumerator func, ref IEnumerator funcVariable)
    {
        StopCoroutine(funcVariable);
        funcVariable = func;
        StartCoroutine(funcVariable);
    }
    public IEnumerator fadeAndStop(float duration, AudioSource audioSource)
    {
        float timer = duration;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            audioSource.volume = timer / duration/2f;
            yield return null;
        }
        audioSource.Stop();
    }
    public IEnumerator spawnPlayer(float duration)
    {
        GameObject beam = (GameObject)Instantiate(Resources.Load("GrowingBeam"));
        GameObject flash = (GameObject)Instantiate(Resources.Load("LightningFlash"));
        flash.transform.position = transform.position;
        beam.transform.position = transform.position;
        movementState = MovementState.Paralyzed;
        if (debugColors)
            spriteRenderer.enabled = false;
        soundEffectPlayer.PlayOneShot(spawnSound);
        inSpawnAnimation = true;

        yield return new WaitForSeconds(duration);

        inSpawnAnimation = false;
        if (debugColors)
            spriteRenderer.enabled = true;
        movementState = MovementState.Free;
    }
    
    public int getFacingDirection()
    {
        return (int)facingDirection;
    }
    public int getAimDirection()
    {
        return (int)aimDirection;
    }
    public Vector2 getVelocity()
    {
        return new Vector2(xVelocity, yVelocity);
    }
    public bool attacking()
    {
        return isAttacking;
    }
    public bool lungeAttacking()
    {
        return isLungeAttacking;
    }
    public bool moving()
    {
        return isMovingVertical || isMovingHorizontal;
    }
    public bool isSpawning()
    {
        return inSpawnAnimation;
    }
    public Vector2 distanceMoved()
    {
        return movementVector;
    }
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.blue;
       
        Gizmos.DrawLine(transform.position,transform.position+grappleMaxDistance*(new Vector3(Mathf.Cos(aimIndicator.eulerAngles.z*Mathf.Deg2Rad),Mathf.Sin(aimIndicator.eulerAngles.z*Mathf.Deg2Rad))));
        for(int i = 0; i < debugHits.Length; i++)
        {
            Gizmos.DrawWireSphere(debugHits[i].transform.position, 1);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, xVelocity / 10);
        */
    }
}
