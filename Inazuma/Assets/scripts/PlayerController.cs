using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    // Use this for initialization
    Prime31.CharacterController2D charController;

    public KeyCode leftButton = KeyCode.LeftArrow;
    public KeyCode rightButton = KeyCode.RightArrow;
    public KeyCode upButton = KeyCode.UpArrow;
    public KeyCode downButton = KeyCode.DownArrow;
    public KeyCode jumpButton = KeyCode.Space;
    public KeyCode attackButton = KeyCode.V;
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
    public float maxHorizontalVelocity = 0;     //should be positive
    public float maxVerticalVelocity = 0;       //should be positive

    public float acceleration = 0;              //ACTIVE acceleration (when attempting to move in direction of travel)
    public float deceleration = 0;              //ACTIVE deceleration (when attempting to move against direction of travel)
    public float passiveDeceleration = 0;       //basically friction.  also subject to airAccelerationFactor
    public float airAccelerationFactor = 1f;    //from 0.0 to 1.0
    public float gravity = 0;                   //should be positive
    public float jumpForce = 0;                 //should be positive
    public float jumpButtonReleaseFactor = 1f;  //total velocity will be mulitiplied by this factor when the jump button is released
                                                //AND before the apex of the jump is reached (from 0.0 to 1.0)

    public float attackDuration = 0f;           //active time for an attack
    public float attackWindUp = 0f;             //time between key input and active frames
    public float timeBetweenAttacks = 0f;       //delay between the end of one attack and the windup of another
    public float attackHitBoxDist = 0f;         //distance between center of player's gameObject and center of Hitbox.  Does not affect hitBox size or shape
    private bool canAttack = true;                     //can the player attack this frame

    private float xVelocity = 0;                //-maxHorizontalVelocity < xVelocity < maxHorizontalVelocity
    private float yVelocity = 0;                //-maxVerticalVelocity < yVelocity < maxVerticalVelocity
    private bool canJump = false;               //can the player jump
    private bool jumping = false;               //is the player current jumping (jumping has different physics than general gravity)
    private bool jumpKeyHeld = false;           //tracks if the player was holding the jump key last frame, to run code when the player releases it
    private float jumpApexTimer = 0;            //time until char reaches apex of jump.  value based on jumpForce


    private enum Direction
    {
        Right, DownRight,Down, DownLeft, Left, UpLeft, Up, UpRight
    }
    private Direction aimDirection;                                 //NESW direction based on held keys (8 directions)
    private Direction facingDirection = Direction.Right;            //character is facing left or right (character can face a direction with no input)

    private HitBoxReport attackHitBoxReport;

    private void Awake()
    {
        charController = gameObject.GetComponent<Prime31.CharacterController2D>();
        attackHitBoxReport = GetComponentInChildren<HitBoxReport>();
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (charController.isGrounded)
            canJump = true;
        if (Input.GetKeyDown(jumpButton))
        {
            //print("grounded: " + charController.isGrounded + "; jumping: " + jumping + "; canJump: " + canJump);
            if (charController.isGrounded && !jumping && canJump)
            {
                jump();
            }
        }
        updateHorizontalVelocity();
        updateVerticalVelocity();
        updatePosition();
        updateDirections();

        if (Input.GetKeyDown(attackButton))
        {
            if (canAttack)
            {
                StartCoroutine(attack(getAimVector(aimDirection)));
                
            }
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
        charController.move(new Vector2(0, yV));
    }
    private void updatePosition()
    {
        moveHorizontal(xVelocity);
        moveVertical(yVelocity);
    }
    private void updateHorizontalVelocity()
    {
        float xVelToAdd = 0f;

        if (Input.GetKey(leftButton))
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
        else if (Input.GetKey(rightButton))
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
                xVelToAdd = (xVelocity + xVelToAdd < 0) ? -xVelocity : xVelToAdd;    //make you stop instead of reverse direction
            }
            else if (xVelocity < 0)
            {
                xVelToAdd = passiveDeceleration * Time.deltaTime;
                xVelToAdd = (xVelocity + xVelToAdd > 0) ? -xVelocity : xVelToAdd;    //set velocity to zero instead of reversing direction
            }
        }
        if (!charController.isGrounded)
            xVelToAdd *= airAccelerationFactor;
        //if char is in the air, it would add acceleration * airAccelerationFactor * time.deltaTime;
        xVelocity = Mathf.Clamp(xVelocity + xVelToAdd, -maxHorizontalVelocity, maxHorizontalVelocity);
        //ensure xVelocity < maxHorizontalVelocity
    }
    private void updateVerticalVelocity()
    {
        float yVelToAdd = 0;
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
        yVelocity = Mathf.Clamp(yVelocity + yVelToAdd, -maxVerticalVelocity, maxVerticalVelocity);

    }
    private void jump()
    {
        //you can't jump while in the air
        if (!charController.isGrounded)
        {
            print("JUMPERROR: character not grounded");
        }
        yVelocity = jumpForce;
        jumping = true;
        jumpApexTimer = jumpForce / gravity;

    }
    public bool isGrounded()
    {
        return charController.isGrounded;
    }
    private void updateDirections()
    {
        if (Input.GetKey(rightButton))
            facingDirection = Direction.Right;
        else if (Input.GetKey(leftButton))
            facingDirection = Direction.Left;

        aimDirection = getAimDirection();
       
    }
    private Direction getAimDirection()
    {
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
        print("Hit: " + other.name);        //currently triggers when hitbox enters gameObject in EnemyLayer.  Layer interaction can be changed in projectsettings->physics2D
        
    }
    private IEnumerator attack(Vector3 aimVector)
    {
        canAttack = false;
          SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();           //for debug
          spriteRenderer.color = Color.cyan;                                                  //windup: cyan
        
        yield return new WaitForSeconds(attackWindUp);
        attackHitBoxReport.moveHitBox(transform.position + aimVector * attackHitBoxDist, Mathf.Rad2Deg*Mathf.Atan2(aimVector.y, aimVector.x));
        attackHitBoxReport.enableHitBox(attackDuration);
          spriteRenderer.color = Color.red;                                                   //active frames: red
        yield return new WaitForSeconds(attackDuration);        
          spriteRenderer.color = Color.gray;                                                  //cooldown: gray
        yield return new WaitForSeconds(timeBetweenAttacks);
          spriteRenderer.color = Color.yellow;                                                //default: yellow
        canAttack = true;
    }
}
