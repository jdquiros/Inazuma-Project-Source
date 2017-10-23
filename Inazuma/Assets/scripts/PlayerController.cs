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
                                                //AND before the apex of the jump is reached

    private float xVelocity = 0;                //-maxHorizontalVelocity < xVelocity < maxHorizontalVelocity
    private float yVelocity = 0;                //-maxVerticalVelocity < yVelocity < maxVerticalVelocity
    private bool canJump = false;               //can the player jump
    private bool jumping = false;               //is the player current jumping (jumping has different physics than general gravity)
    private bool jumpKeyHeld = false;           //tracks if the player was holding the jump key last frame, to run code when the player releases it
    private float jumpApexTimer = 0;            //time until char reaches apex of jump.  value based on jumpForce

    private void Awake()
    {
        charController = gameObject.GetComponent<Prime31.CharacterController2D>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (charController.isGrounded)
            canJump = true;
        if (Input.GetKeyDown(upButton))
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
        //Prime31.CharacterController2D.CharacterCollisionState2D colState = charController.collisionState;
        //print(colState.ToString());
        //EventSystem es;
        //charController.onControllerCollidedEvent;


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
                if (!Input.GetKey(upButton) && jumpKeyHeld)
                {
                    //you release the jump key before you reach the apex
                    jumpApexTimer = 0;
                    yVelocity *= jumpButtonReleaseFactor;
                }
                jumpKeyHeld = Input.GetKey(upButton);
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
}
