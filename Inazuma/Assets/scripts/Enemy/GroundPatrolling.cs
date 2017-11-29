using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolling : MonoBehaviour
{

    private Prime31.EnemyController2D charController;
    public float xVelocity = 0;
    public int waitDuration;
    public int gravity;
    public Transform waypointA;
    public Transform waypointB;

    private bool isWaiting;
    private float waitTimer;



    // Use this for initialization
    void Start()
    {
        charController = GetComponent<Prime31.EnemyController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        charController.move(new Vector2(0, -gravity * Time.deltaTime));
        if (charController.isGrounded)
        {
            if (!isWaiting)
            {
                moveHorizontal(xVelocity * Time.deltaTime);
            }

            if (waitTimer <= 0)
            {
                isWaiting = false;
            }

            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }


    private void moveHorizontal(float xV)
    {
        charController.move(new Vector2(xV, 0));


        if (transform.position.x < waypointA.position.x && xV < 0)
        {
            waitTimer = waitDuration;
            isWaiting = true;

            xVelocity *= -1;
        }
        else if (transform.position.x > waypointB.position.x && xV > 0)
        {
            waitTimer = waitDuration;
            isWaiting = true;

            xVelocity *= -1;
        }


    }

    public void setWaypoints(Transform A, Transform B)
    {
        waypointA = A;
        waypointB = B;
    }
    public bool waiting() { return isWaiting; }
    public bool isGrounded() { return charController.isGrounded; }
}