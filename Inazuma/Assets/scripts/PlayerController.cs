﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    // Use this for initialization
    Prime31.CharacterController2D charController;

    public KeyCode leftButton = KeyCode.A;
    public KeyCode rightButton = KeyCode.D;
    public KeyCode upButton = KeyCode.W;
    public KeyCode downButton = KeyCode.S;
    public KeyCode jumpButton = KeyCode.Space;
    public KeyCode attackButton = KeyCode.J;
    public KeyCode LungeButton = KeyCode.K;
    public KeyCode dashButton = KeyCode.LeftShift;
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
        Paralyzed, Free, Dash, Lunge
    }
    private MovementState movementState = MovementState.Free;
    private bool allowPlayerInput;
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

    public float dashDuration = 0f;             //duration of dash
    private float dashTimer = 0f;
    public float dashAcceleration = 0f;         //acceleration of dash
    public float dashMaxVelocity = 0f;          //maximum velocity during dash
    public float dashCooldown = 0f;             //cooldown between dashes
    private float dashCooldownTimer = 0f;
    private bool canDash = true;                //can the player dash this frame.  you cannot dash while attacking
    [HideInInspector]
    public bool isDashing = false;
    public float velocityRestrictionRate = 0f;  //rate that velocity > maxVelocity returns to maxVelocity

    public float instantDropDistance = 0.2f;    //instantly moves player down this distance when dropping through a platform.  If the character does not drop, increase this value

    private float xVelocity = 0;                //-maxHorizontalVelocity < xVelocity < maxHorizontalVelocity
    private float yVelocity = 0;                //-maxVerticalVelocity < yVelocity < maxVerticalVelocity
    private bool canJump = false;               //can the player jump
    private bool jumping = false;               //is the player current jumping (jumping has different physics than general gravity)
    private bool jumpKeyHeld = false;           //tracks if the player was holding the jump key last frame, to run code when the player releases it
    private float jumpApexTimer = 0;            //time until char reaches apex of jump.  value based on jumpForce

    private Vector3 forcedMoveVector;
    int enemyHits = 0;                          //# of enemies hit in a single attack
    private enum Direction
    {
        Right, DownRight, Down, DownLeft, Left, UpLeft, Up, UpRight
    }
    private Direction aimDirection;                                 //NESW direction based on held keys (8 directions)
    private Direction dashDirection;
    private Direction facingDirection = Direction.Right;            //character is facing left or right (character can face a direction with no input)

    private HitBoxReport attackHitBoxReport;                        //activates hitbox for attacks and reports collisions
    private SpriteRenderer spriteRenderer;

    private bool wasGrounded;

    private void Awake()
    {
        charController = gameObject.GetComponent<Prime31.CharacterController2D>();
        attackHitBoxReport = GetComponentInChildren<HitBoxReport>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Start()
    {
        allowPlayerInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (charController.isGrounded)
            canJump = true;
        switch (movementState) {
            case MovementState.Free:
                if (Input.GetKeyDown(jumpButton))
                {
                    //print("grounded: " + charController.isGrounded + "; jumping: " + jumping + "; canJump: " + canJump);
                    if (charController.isGrounded && !jumping && canJump)
                    {
                        jump();
                    }
                }
                if (Input.GetKeyDown(downButton))
                {
                    attemptDropThroughPlatform();
                }
                if (Input.GetKeyDown(dashButton) && canDash)
                {
                    dash(facingDirection);
                }
                if (Input.GetKeyDown(attackButton))
                {
                    if (canAttack)
                    {
                        StartCoroutine(attack(getAimVector(aimDirection)));
                    }
                }
                if (Input.GetKeyDown(LungeButton))
                {
                    if (canAttack)
                    {
                        aimDirection = getAimDirection();
                        dashDirection = aimDirection;
                        StartCoroutine(lungeAttack(getAimVector(aimDirection)));
                    }
                }
                break;
        }
        wasGrounded = isGrounded();
        updateDashing();
        updateHorizontalVelocity();
        updateVerticalVelocity();
        updatePosition();
        updateDirections();
        if (movementState == MovementState.Free)
        {
            if (wasGrounded && !isGrounded() && !jumping)
            {
                yVelocity = 0;
                charController.move(new Vector2(0, 25) * Time.deltaTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
      
            SceneManager.LoadScene("kyle_test_scene");
        }
    }
    private void moveHorizontal(float xV)
    {
        Vector3 position = transform.position;
        charController.move(new Vector2(xV, 0));
        if (xV != 0 && Vector3.Distance(position, transform.position) < 0.0005f)          //I don't know how to check for wall collisions in CharacterController2D so this is a janky workaround
        {
            xVelocity = 0;
        }
    }
    private void moveVertical(float yV)
    {
        Vector3 position = transform.position;
        charController.move(new Vector2(0, yV));
        if (yV > 0 && transform.position.y <= position.y)          //I don't know how to check for wall collisions in CharacterController2D so this is a janky workaround
        {
            yVelocity = 0;
        }
    }
    private void updatePosition()
    {
        moveHorizontal(xVelocity * Time.deltaTime);
        moveVertical(yVelocity * Time.deltaTime);
    }
    private void updateHorizontalVelocity()
    {
        switch (movementState)
        {
            case (MovementState.Free):
                //regular movement, not dashing
                float xVelToAdd = 0f;
                if (Input.GetKey(leftButton) && !Input.GetKey(rightButton))
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
                else if (Input.GetKey(rightButton) && !Input.GetKey(leftButton))
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
                xVelocity += forcedMoveVector.x * dashAcceleration * Time.deltaTime;
                xVelocity = Mathf.Clamp(xVelocity, -dashMaxVelocity, dashMaxVelocity);
                break;
            case (MovementState.Paralyzed):
                xVelocity *= 0.5f;
                break;
        }


    }
    private void updateVerticalVelocity()
    {
        float yVelToAdd = 0;
        switch (movementState)
        {
            case (MovementState.Free):
                if (!charController.isGrounded)
                {
                    yVelToAdd = (-gravity) * Time.deltaTime;
                }
                if (jumping)
                {
                    if (jumpApexTimer > 0)
                    {
                        //if this is >0, then character is rising until apex.
                        jumpApexTimer -= Time.deltaTime;
                        if (!Input.GetKey(jumpButton) && jumpKeyHeld)
                        {
                            //you release the jump key before you reach the apex
                            jumpApexTimer = 0;
                            yVelocity *= jumpButtonReleaseFactor;
                        }
                        jumpKeyHeld = Input.GetKey(jumpButton);
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
                    yVelocity = -25f;       //reset velocity to zero (almost) when you are on the ground.  
                }
                else
                {
                    yVelocity = Mathf.Clamp(yVelocity + yVelToAdd, -maxVerticalVelocity, maxVerticalVelocity);
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
                yVelocity += forcedMoveVector.y * dashAcceleration * Time.deltaTime;
                yVelocity = Mathf.Clamp(yVelocity, -dashMaxVelocity, dashMaxVelocity);
                jumping = false;
                jumpApexTimer = 0;
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
                    dashTimer = 0;
                    dashCooldownTimer = dashCooldown;
                    isDashing = false;
                    canAttack = true;
                    movementState = MovementState.Free;
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
                    dashTimer = 0;
                    isDashing = false;

                    spriteRenderer.color = Color.yellow;
                    movementState = MovementState.Free;
                }
                break;
            case (MovementState.Free):
                if (!canDash)
                {
                    if (dashCooldownTimer > 0)
                    {
                        dashCooldownTimer -= Time.deltaTime;
                    }
                    else
                    {
                        dashCooldownTimer = 0;
                        canDash = true;
                        spriteRenderer.color = Color.yellow;
                    }
                }
                break;
        
        }
    
    }

        
    
    private void jump()
    {
        if (movementState == MovementState.Free)
        {
            yVelocity = jumpForce;
            canJump = false;
            jumping = true;
            jumpApexTimer = jumpForce / gravity;
        }
    }
    private void attemptDropThroughPlatform()
    {
        if (movementState == MovementState.Free && isGrounded())
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
        if (movementState == MovementState.Free)                             //you cannot change your facing direction while dashing
        {
            if (!(Input.GetKey(leftButton) && Input.GetKey(rightButton)))
            {
                if (Input.GetKey(rightButton))
                    facingDirection = Direction.Right;
                else if (Input.GetKey(leftButton))
                    facingDirection = Direction.Left;
            }
            aimDirection = getAimDirection();
        }

    }
    private Direction getAimDirection()
    {
        if (Input.GetKey(leftButton) && Input.GetKey(upButton) && Input.GetKey(rightButton))
            return Direction.Up;
        if (Input.GetKey(upButton) && Input.GetKey(rightButton) && Input.GetKey(downButton))
            return Direction.Right;
        if (Input.GetKey(rightButton) && Input.GetKey(downButton) && Input.GetKey(leftButton))
            return Direction.Down;
        if (Input.GetKey(downButton) && Input.GetKey(leftButton) && Input.GetKey(upButton))
            return Direction.Left;
        if(Input.GetKey(upButton) && Input.GetKey(rightButton))         //upright
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
                result = new Vector3(0.707f, 0.707f, 0);    //distance is approximately 1f, but diagonal.  we shouldn't need this but why not
                break;
            case Direction.Right:
                result = new Vector3(1, 0, 0);
                break;
            case Direction.DownRight:
                result = new Vector3(0.707f, -0.707f, 0);
                break;
            case Direction.Down:
                result = new Vector3(0, -1, 0);
                break;
            case Direction.DownLeft:
                result = new Vector3(-0.707f, -0.707f, 0);
                break;
            case Direction.Left:
                result = new Vector3(-1, 0, 0);
                break;
            case Direction.UpLeft:
                result = new Vector3(-0.707f, 0.707f, 0);
                break;
        }
        return result;
    }
    void onHitBoxCollision(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("enemyLayer"))
        {
            ++enemyHits;
            if (enemyHits == 1)
                lungeDash(getAimVector(dashDirection));
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("hazardLayer"))
        {
            damagePlayer(1000000000);       //definitely kill the player
        }
    }
    private IEnumerator attack(Vector3 aimVector)
    {
        canAttack = false;
        canDash = false;
                     //for debug
          spriteRenderer.color = Color.cyan;                                                  //windup: cyan
        
        yield return new WaitForSeconds(attackWindUp);
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg*Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
          spriteRenderer.color = Color.red;                                                   //active frames: red
        yield return new WaitForSeconds(attackDuration);        
          spriteRenderer.color = Color.gray;                                                  //cooldown: gray
        if (dashCooldownTimer <= 0)
        {
            canDash = true;                                                                       //player can dash during attack cooldown
        }
        yield return new WaitForSeconds(timeBetweenAttacks);
          spriteRenderer.color = Color.yellow;                                                //default: yellow
        canAttack = true;
    }
    private IEnumerator lungeAttack(Vector3 aimVector)
    {
        movementState = MovementState.Lunge;
        canAttack = false;
        canDash = false;
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(attackWindUp);
        enemyHits = 0;
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg * Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(attackDuration);
        spriteRenderer.color = Color.gray;
        
        if(dashCooldownTimer <= 0)
        {
            canDash = true;
        }
        yield return new WaitForSeconds(timeBetweenAttacks);
        spriteRenderer.color = Color.yellow;
        canAttack = true;
    }
    private void dash(Vector3 direction)
    {
        forcedMoveVector = direction;
        canDash = false;
        isDashing = true;
        dashTimer = dashDuration;
        canAttack = false;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
    }
    private void dash(Direction direction)
    {
        forcedMoveVector = getAimVector(direction);
        canDash = false;
        isDashing = true;
        dashTimer = dashDuration;
        canAttack = false;
        movementState = MovementState.Dash;
        xVelocity = 0;
        yVelocity = 0;
    }
    private void lungeDash(Vector3 direction)
    {
        forcedMoveVector = direction;
        canDash = false;
        isDashing = true;
        dashTimer = dashDuration;
        canAttack = false;
        movementState = MovementState.Lunge;
        xVelocity = forcedMoveVector.x * dashMaxVelocity;
        yVelocity = forcedMoveVector.y * dashMaxVelocity;
      
    }
    public void damagePlayer(int dmg)
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
    public bool isDead()
    {
        return playerDead;
    }
    
}
