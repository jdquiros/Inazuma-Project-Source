using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    // Use this for initialization
    
    Prime31.CharacterController2D charController;

    public KeyCode leftButton = KeyCode.A;                      //deprecated.  go to unity editor -> project settings -> Input
    public KeyCode rightButton = KeyCode.D;                     //deprecated
    public KeyCode upButton = KeyCode.W;                        //deprecated
    public KeyCode downButton = KeyCode.S;                      //deprecated
    public KeyCode jumpButton = KeyCode.Space;                  //deprecated
    public KeyCode attackButton = KeyCode.J;                    //deprecated
    public KeyCode lungeButton = KeyCode.K;                     //deprecated
    public KeyCode dashButton = KeyCode.LeftShift;              //deprecated
    public KeyCode restartButton = KeyCode.R;                   //deprecated
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
        Paralyzed, Free, Dash, Lunge, OnLadder, Hover
    }
    public MovementState movementState = MovementState.Free;
    
    private bool allowPlayerInput;              //can the character be controlled?
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
    private bool isLungeAttacking = false;
    [Range(0.707f, 1f)]
    public float lungeDiagonalScale = 1f;       //when set to 1, diagonal lunges move at Rad(2) speed (which is the distance of a (1,1) vector)
                                                //this value changes that speed to the distance of a (lungeDiagonalScale,lungeDiagonalScale) vector
                                                //distance of (0.707,0.707) is 1
                                                //distance of (1,1) is 1.414

    public float velocityRestrictionRate = 0f;  //rate that velocity > maxVelocity returns to maxVelocity

    public float instantDropDistance = 0.2f;    //instantly moves player down this distance when dropping through a platform.  If the character does not drop, increase this value

    private float xVelocity = 0;                //-maxHorizontalVelocity < xVelocity < maxHorizontalVelocity
    private float yVelocity = 0;                //-maxVerticalVelocity < yVelocity < maxVerticalVelocity
    private bool canJump = false;               //can the player jump
    private bool jumping = false;               //is the player current jumping (jumping has different physics than general gravity)
    private bool jumpKeyHeld = false;           //tracks if the player was holding the jump key last frame, to run code when the player releases it
    private float jumpApexTimer = 0;            //time until char reaches apex of jump.  value based on jumpForce

    private Vector3 forcedMoveVector;           //this vector is the direction of a dash/lungedash
    int enemyHits = 0;                          //# of enemies hit in a single attack

    private enum Direction
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

    public float knockbackBackVelocity;                             //player's x velocity is set to this when knocked back
    public float knockbackUpVelocity;                               //player's y velocity is set to this when knocked back
    public float hitStunDuration;                                   //player does not accept input for this duration (after being hit)
    public float hitInvincibilityDuration;                          //duration for the hitstun period
    private bool invulnerable = false;                              //can the player be damaged (forceDamagePlayer(int x) will bypass this (eg spikes))
    private bool landedThisFrame = false;
    private bool playerHitThisFrame = false;
    private bool xAxisMaxed = false;                                //is the xAxis of the left analog stick maxed out (either direction counts)
    private bool yAxisMaxed = false;                                //is the xAxis of the left analog stick maxed out (either direction counts)
    private bool rightStickMaxed = false;                            //is the right analog stick maxed out (any direction)
    public float axisDeadZone = 0.05f;                              //general tolerance for axis-related code (used both as an upper and lower bound threshold)
    private IEnumerator hoverCoroutine;
    private bool flipSwing = false;
    public float spawnAnimDuration = .2f;
    private bool inSpawnAnimation = false;

    private void Awake()
    {
        charController = gameObject.GetComponent<Prime31.CharacterController2D>();
        attackHitBoxReport = GetComponentInChildren<HitBoxReport>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        
    }
    void Start()
    {
        transform.position = Checkpoint.GetCurrentCheckpointPos();
        charController.warpToGrounded();
        if (GameState.compareState(GameState.State.InGame))
        {
            StartCoroutine(spawnPlayer(spawnAnimDuration));
        }
        allowPlayerInput = true;
        fadeSound = fadeAndStop(footstepSoundFadeDuration,source);
        hoverCoroutine = hoverForDuration(lungeHoverDuration);
        //charController.warpToGrounded();
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject pExplosion = (GameObject)Instantiate(Resources.Load("ZigZagParticleExplosion"));
            pExplosion.transform.position = transform.position;
            foreach(TrailRenderer tr in pExplosion.GetComponentsInChildren<TrailRenderer>())
            {
                tr.Clear();
            }
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GameObject beam = (GameObject)Instantiate(Resources.Load("GrowingBeam"));
            beam.transform.position = transform.position;
        }
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
        switch (movementState) {
            case MovementState.Free:
                if (Input.GetButtonDown("Jump"))
                {
                    //print("grounded: " + charController.isGrounded + "; jumping: " + jumping + "; canJump: " + canJump);
                    if ((charController.isGrounded  || jumpInAirTimer > 0) && !jumping && canJump)
                    {
                        jump();
                    }
                }
                
                if (isMovingHorizontal && isGrounded())
                {
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
                if (maxedYAxisThisFrame(Direction.Down))
                {   
                    //needs improvement to only activate once
                    attemptDropThroughPlatform();
                }
                if (Input.GetButtonDown("Dash") && canDash)
                {
                    source.Stop();

                    dash(facingDirection);
                }
                checkForAttackInput();
                if ((((maxedYAxisThisFrame(Direction.Up)) && isGrounded())
                    ||((yAxisMaxed && ladderGrabTimer <= 0))) 
                    && movementState == MovementState.Free 
                    && inLadder)
                {
                    movementState = MovementState.OnLadder;
                    transform.position = new Vector3(ladderBounds.center.x, transform.position.y, transform.position.z);
                    
                    charController.ignoreOneWayPlatformsThisFrame = true;
                    if (maxedYAxisThisFrame(Direction.Down))
                    {
                        attemptDropThroughPlatform();
                    }
                    xVelocity = 0;


                }
                if (jumpInAirTimer > 0)
                    jumpInAirTimer -= Time.deltaTime;
                else
                    jumpInAirTimer = 0;
                
                break;  
		    case MovementState.Paralyzed:
			    if (playerDead && (Input.GetKeyDown (restartButton) || Input.GetButtonDown("Submit"))) 
			    {
				    respawn ();
			    }
                spriteRenderer.color = Color.black;
		        break;
            case MovementState.OnLadder:
                updateLadderMovement();
                canJump = true;
                preventCooldown = false;
                break;
            case MovementState.Dash:
                checkForAttackInput();
                if(Input.GetButtonDown("Jump") && (charController.isGrounded || jumpInAirTimer > 0 || charController.isMovingUpSlope()) && !jumping && canJump)
                {
                    jump();
                    endDash();
                }
                if (wasGrounded && !isGrounded() && !jumping)
                {
                    jumpInAirTimer = jumpInAirDuration;
                }
                if (jumpInAirTimer > 0)
                    jumpInAirTimer -= Time.deltaTime;
                else
                    jumpInAirTimer = 0;

                break;
            case MovementState.Lunge:
                checkForAttackInput();
                break;
            case MovementState.Hover:
                if (isGrounded())
                {
                    movementState = MovementState.Free;
                    StopCoroutine(hoverCoroutine);
                }
                if (Input.GetButtonDown("Dash") && canDash)
                {
                    StopCoroutine(hoverCoroutine);
                    dash(facingDirection);
                }
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

        xAxisMaxed = Mathf.Abs(Input.GetAxis("Horizontal")) >= .95f;
        yAxisMaxed = Mathf.Abs(Input.GetAxis("Vertical")) >= .95f;
        rightStickMaxed = Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis("RStickHorizontal"), Input.GetAxis("RStickVertical"))) >= (1 - axisDeadZone);
    }
    private void checkForAttackInput()
    {
        
        if (Input.GetButtonDown("Attack"))
        {
            if (canAttack)
            {
                source.Stop();

                StartCoroutine(attack(getAimVector(aimDirection).normalized));
                if (attacksEndDashes)
                {
                    endDash();
                }
            }

        }
        if (Input.GetButtonDown("Lunge") || maxedRightStickThisFrame())
        {
            if (canAttack)
            {
                source.Stop();

                aimDirection = calculateAimDirection();
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
            isMovingHorizontal = false;
        }
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
            if (wasGrounded && !isGrounded() && !jumping)
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
    }
    private void updatePosition()
    {
        moveHorizontal(xVelocity * Time.deltaTime);
        moveVertical(yVelocity * Time.deltaTime);
    }
    private void updateHorizontalVelocity()
    {
        float xVelToAdd = 0f;

        switch (movementState)
        {
            case (MovementState.Free):
                //regular movement, not dashing
                if (holdingDirection(Direction.Left))
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
                else if (holdingDirection(Direction.Right))
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
                    xVelToAdd *= airAccelerationFactor;
                //if char is in the air, it would add acceleration * airAccelerationFactor * time.deltaTime;


                xVelocity = xVelocity + xVelToAdd;      //set speeds

                //clamp to speed maximums
                if (xVelocity > maxHorizontalVelocity)           //gradually decrease speed to max speed
                {
                    xVelocity += -velocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += velocityRestrictionRate * Time.deltaTime;
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
                    xVelocity += -velocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += velocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Min(xVelocity, -maxHorizontalVelocity);
                }
                break;
            case (MovementState.Hover):
                if (holdingDirection(Direction.Left))
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
                else if (holdingDirection(Direction.Right))
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
                    xVelToAdd *= airAccelerationFactor;
                //if char is in the air, it would add acceleration * airAccelerationFactor * time.deltaTime;


                xVelocity = xVelocity + xVelToAdd;      //set speeds

                //clamp to speed maximums
                if (xVelocity > maxHorizontalVelocity)           //gradually decrease speed to max speed
                {
                    xVelocity += -velocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Max(xVelocity, maxHorizontalVelocity);
                }
                else if (xVelocity < (-maxHorizontalVelocity))
                {
                    xVelocity += velocityRestrictionRate * Time.deltaTime;
                    xVelocity = Mathf.Min(xVelocity, -maxHorizontalVelocity);
                }
                break;
        }


    }
    private void updateVerticalVelocity()
    {
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
                        if (!Input.GetButton("Jump") && jumpKeyHeld)
                        {
                            //you release the jump key before you reach the apex
                            jumpApexTimer = 0;
                            yVelocity *= jumpButtonReleaseFactor;
                        }
                        jumpKeyHeld = Input.GetButton("Jump");
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
                if (isGrounded() && !jumping)
                {
                    yVelocity = -25f;       //downward force so you stick to slopes
                }
                else
                {
                    
                    yVelocity += yVelToAdd;
                    if (yVelocity > maxVerticalVelocity)           //gradually decrease speed to max speed
                    {
                        yVelocity += -velocityRestrictionRate * Time.deltaTime;
                        yVelocity = Mathf.Max(yVelocity, maxVerticalVelocity);
                    }
                    else if (yVelocity < (-maxVerticalVelocity))
                    {
                        yVelocity += velocityRestrictionRate * Time.deltaTime;
                        yVelocity = Mathf.Min(yVelocity, -maxVerticalVelocity);
                    }
                }
                break;
            case (MovementState.Dash):
                if (isDashing)           //ignore gravity during dash
                {
                    yVelocity = -0.001f;    //ensure you stay grounded
                    jumping = false;        //cancel jump physics
                    jumpApexTimer = 0;
                }
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
                if (isGrounded() && !jumping)
                {
                    yVelocity = -25f;       //downward force so you stick to slopes
                }
                break;
            case (MovementState.Hover):
                if (yVelocity > 0)           //gradually decrease speed to 0
                {
                    yVelocity += -velocityRestrictionRate * Time.deltaTime;
                    yVelocity = Mathf.Max(yVelocity, 0);
                }
                else if (yVelocity < 0)
                {
                    yVelocity += velocityRestrictionRate * Time.deltaTime;
                    yVelocity = Mathf.Min(yVelocity, 0);
                }
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
                        canDash = true;
                        spriteRenderer.color = Color.yellow;
                    }
                }
                break;
            case (MovementState.Hover):
                if (!canDash)
                {
                    if (dashCooldownTimer > 0)
                    {
                        dashCooldownTimer -= Time.deltaTime;
                    }
                    else if (!preventCooldown)       //do not finish cooldown until you are grounded
                    {
                        dashCooldownTimer = 0;
                        canDash = true;
                        spriteRenderer.color = Color.yellow;
                    }
                }
                break;
        
        }
    
    }
    private void updateLadderMovement()
    {
        charController.ignoreOneWayPlatformsThisFrame = true;
        xVelocity = 0;
        spriteRenderer.color = Color.green;
        transform.position = new Vector3(ladderBounds.center.x, transform.position.y, transform.position.z);    //ensure player is centered on ladder
        source.clip = climbSound;
        if (Input.GetButtonDown("Jump"))
        {
            //jump off of ladder
            movementState = MovementState.Free;
            ladderGrabTimer = ladderGrabCooldown;
            spriteRenderer.color = Color.yellow;
            if (!holdingDirection(Direction.Down))
            {
                jump();
            }
        } else if (holdingDirection(Direction.Up))
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
        } else if (holdingDirection(Direction.Down))
        {
            //climb down
            if (isGrounded())
            {
                movementState = MovementState.Free;
                ladderGrabTimer = ladderGrabCooldown / 2;
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
        if (movementState == MovementState.Free || movementState == MovementState.OnLadder || movementState == MovementState.Hover)                             //you cannot change your facing direction while dashing
        {
            
                if (holdingDirection(Direction.Right))
                    facingDirection = Direction.Right;
                else if (holdingDirection(Direction.Left))
                    facingDirection = Direction.Left;
            
        }
        
            aimDirection = calculateAimDirection();
 
            
        

    }
    private Direction calculateAimDirection()
    {
        if ((Mathf.Abs(Input.GetAxis("RStickHorizontal")) < axisDeadZone && Mathf.Abs(Input.GetAxis("RStickVertical")) < axisDeadZone))
        {
            
            if (Input.GetKey(leftButton) && Input.GetKey(upButton) && Input.GetKey(rightButton))
                return Direction.Up;
            if (Input.GetKey(upButton) && Input.GetKey(rightButton) && Input.GetKey(downButton))
                return Direction.Right;
            if (Input.GetKey(rightButton) && Input.GetKey(downButton) && Input.GetKey(leftButton))
                return Direction.Down;
            if (Input.GetKey(downButton) && Input.GetKey(leftButton) && Input.GetKey(upButton))
                return Direction.Left;
            if (Input.GetKey(upButton) && Input.GetKey(rightButton))         //upright
                return Direction.UpRight;
            if (Input.GetKey(upButton) && Input.GetKey(leftButton))         //upleft
                return Direction.UpLeft;
            if (Input.GetKey(downButton) && Input.GetKey(rightButton))       //downright
                return Direction.DownRight;
            if (Input.GetKey(downButton) && Input.GetKey(leftButton))        //downLeft
                return Direction.DownLeft;
            if (Input.GetKey(upButton))                                     //up
                return Direction.Up;
            if (Input.GetKey(rightButton))                                   //right
                return Direction.Right;
            if (Input.GetKey(downButton))                                    //down
                return Direction.Down;
            if (Input.GetKey(leftButton))                                    //left
                return Direction.Left;
        }
        else
        {
            //diagonals must be tested before singular directions, because single directions are subsets of diagonals
            
                float stickAngle = (Mathf.Atan2(Input.GetAxis("RStickVertical"), Input.GetAxis("RStickHorizontal")) * Mathf.Rad2Deg);
                if (stickAngle < 0) stickAngle += 360;
                for (int i = 0; i < 8; ++i)
                {
                    if (Mathf.Abs(i * 45 - stickAngle) <= 22.5f)
                    {
                        return (Direction)i;
                    }
                }
            
        }
        return facingDirection;         //If no key input, default to where your character is facing

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
        dashTimer = 0;
        minDashTimer = 0;
        dashCooldownTimer = dashCooldown;
        isDashing = false;
        movementState = MovementState.Free;
        spriteRenderer.color = Color.gray;
    }
    private void endLunge()
    {
        invulnerable = false;
        dashTimer = 0;
        isDashing = false;
        spriteRenderer.color = Color.yellow;
        canAttack = true;
        StopCoroutine(hoverCoroutine);

        hoverCoroutine = hoverForDuration(lungeHoverDuration);
        StartCoroutine(hoverCoroutine);
        
    }
    private IEnumerator hoverForDuration(float time)
    {
        movementState = MovementState.Hover;
        yield return new WaitForSeconds(time);
        movementState = MovementState.Free;
    }
    void onHitBoxCollision(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().damageEnemy(1, other.transform.position - transform.position);
            ++enemyHits;
            if (enemyHits == 1 && isLungeAttacking)
            {
                lungeDash(getAimVector(dashDirection));
                isLungeAttacking = false;
            }
        } else if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            other.gameObject.GetComponent<Bullet>().hitByPlayer();
            ++enemyHits;
            if (enemyHits == 1 && isLungeAttacking)
            {
                lungeDash(getAimVector(dashDirection));
                isLungeAttacking = false;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.CompareTag("Checkpoint"))
		{
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
        
          spriteRenderer.color = Color.cyan;                                                  //windup: cyan
        
        yield return new WaitForSeconds(attackWindUp);
        spawnAttackTrail(aimDirection);

        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg*Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
          spriteRenderer.color = Color.red;                                                   //active frames: red
        soundEffectPlayer.PlayOneShot(attackSound);
        yield return new WaitForSeconds(attackDuration);        
          spriteRenderer.color = Color.gray;                                                  //cooldown: gray
        
        isAttacking = false;
        yield return new WaitForSeconds(timeBetweenAttacks);
          spriteRenderer.color = Color.yellow;                                                //default: yellow
        canAttack = true;
    }
    private IEnumerator lungeAttack(Vector3 aimVector)
    {
        isLungeAttacking = true;
        canAttack = false;
        canDash = false;
        dashCooldownTimer = attackWindUp + attackDuration;
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(attackWindUp);
        spawnAttackTrail(aimDirection);
        enemyHits = 0;
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg * Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
        spriteRenderer.color = Color.red;
        soundEffectPlayer.PlayOneShot(attackSound);

        yield return new WaitForSeconds(attackDuration);
        spriteRenderer.color = Color.gray;
        isLungeAttacking = false;

        yield return new WaitForSeconds(timeBetweenAttacks);
        spriteRenderer.color = Color.yellow;
        canAttack = true;

    }
    private void dash(Vector3 direction)
    {

        spawnTrail(1000);
        forcedMoveVector = direction;
        canDash = false;
        isDashing = true;
        canAttack = false;
        dashTimer = dashDuration;
        minDashTimer = minDashTime;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
        soundEffectPlayer.PlayOneShot(dashSound);
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
        canAttack = false;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
        soundEffectPlayer.PlayOneShot(dashSound);
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
        movementState = MovementState.Lunge;
        xVelocity = forcedMoveVector.x * lungeMaxVelocity;
        yVelocity = forcedMoveVector.y * lungeMaxVelocity;
        soundEffectPlayer.PlayOneShot(lungeSound);
        invulnerable = true;
        preventCooldown = false;
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
        if (!isGrounded())
        {
            flipSwing = yVelocity < 0;
            flipSwing = xVelocity < 0 ? !flipSwing : flipSwing;
        }
        trailObject.GetComponent<SwordTrail>().startSwing(1800, new Vector2(startAngle,startAngle+90),transform.position,offset,flipSwing);
        flipSwing = !flipSwing;
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
        spriteRenderer.color = Color.white;
        if(!playerDead)
            movementState = MovementState.Free;

        yield return new WaitForSeconds(invincibilityDuration-duration);
        invulnerable = false;
        spriteRenderer.color = Color.yellow;

    }
    public void attemptDamagePlayer(int dmg)
    {
        if(!invulnerable)
            setHealth(health - dmg);
        
    }
    public void forceDamagePlayer(int dmg)
    {
        setHealth(health - dmg);
    }
    private void setHealth(int hp)
    {
        health = hp;
        if (health <= 0 && !playerDead)
        {
            //this code runs only once when the player dies
            playerDead = true;
            print("Player is Dead");
            allowPlayerInput = false;
            spriteRenderer.color = Color.black;
            movementState = MovementState.Paralyzed;
        }
    }
    private void respawn()
    {

        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
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
        beam.transform.position = transform.position;
        movementState = MovementState.Paralyzed;
        spriteRenderer.enabled = false;
        inSpawnAnimation = true;
        yield return new WaitForSeconds(duration);
        inSpawnAnimation = false;
        spriteRenderer.enabled = true;
        movementState = MovementState.Free;
    }
    private bool maxedYAxisThisFrame(Direction dir)
    {
        switch (dir)
        {
            case (Direction.Up):
                return Input.GetAxis("Vertical") >= (1 - axisDeadZone) && !yAxisMaxed;
            case (Direction.Down):
                return Input.GetAxis("Vertical") <= (-1 + axisDeadZone) && !yAxisMaxed; 
        }
        print("maxedYAxisThisFrame: Invalid Direction->" + dir);
        return false;
    }
    private bool maxedXAxisThisFrame(Direction dir)
    {
        switch (dir)
        {
            case (Direction.Right):
                return Input.GetAxis("Horizontal") >= (1-axisDeadZone) && !xAxisMaxed;
            case (Direction.Left):
                return Input.GetAxis("Horizontal") <= (-1+axisDeadZone) && !xAxisMaxed;
        }
        print("maxedXAxisThisFrame: Invalid Direction->" + dir);
        return false;
    }

    private bool maxedRightStickThisFrame()
    {
        return Vector2.Distance(Vector2.zero,new Vector2(Input.GetAxis("RStickHorizontal"),Input.GetAxis("RStickVertical"))) >= (1 - axisDeadZone) && !rightStickMaxed;
    }
    private bool unMaxedRightStickThisFrame()
    {
        return Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis("RStickHorizontal"), Input.GetAxis("RStickVertical"))) <= (1 - axisDeadZone) && rightStickMaxed;
    }
    private bool holdingDirection(Direction dir)
    {
        switch (dir)
        {
            case (Direction.Right):
                return Input.GetAxis("Horizontal") > axisDeadZone;
            case (Direction.Left):
                return Input.GetAxis("Horizontal") < -axisDeadZone;
            case (Direction.Up):
                return Input.GetAxis("Vertical") > axisDeadZone;
            case (Direction.Down):
                return Input.GetAxis("Vertical") < -axisDeadZone;

            case (Direction.UpRight):
                return Input.GetAxis("Horizontal") > axisDeadZone && Input.GetAxis("Vertical") > -axisDeadZone;
            case (Direction.DownRight):
                return Input.GetAxis("Vertical") < -axisDeadZone && Input.GetAxis("Horizontal") > axisDeadZone;
            case (Direction.DownLeft):
                return Input.GetAxis("Vertical") < -axisDeadZone && Input.GetAxis("Horizontal") < -axisDeadZone;
            case (Direction.UpLeft):
                return Input.GetAxis("Vertical") > axisDeadZone && Input.GetAxis("Horizontal") < -axisDeadZone;
        }

        return false; //should never run
    }
    public int getFacingDirection()
    {
        return (int)facingDirection;
    }
    public int getAimDirectin()
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
}
