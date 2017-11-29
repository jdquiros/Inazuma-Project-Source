﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrolling : MonoBehaviour
{

    public float speed;
    public float waitDuration;
    private bool isWaiting;
    private float waitTimer;

    public Transform waypointA;
    public Transform waypointB;

    private Vector3 destination;
    private Vector3 enemyPos;
    private PlayerController playerController;
    private Enemy enemy;


    // Use this for initialization
    void Start()
    {
        if(waypointA != null)
            destination = waypointA.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.compareState(GameState.State.MainMenu))
        {
            if (destination != null)
            {
                if (!isWaiting)
                {
                    MoveToPoint(destination);
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
    }

    void MoveToPoint(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Vector3 moveVector = direction * speed * Time.deltaTime;


        if (Vector3.Distance(transform.position, target) < speed * Time.deltaTime)
        {
            transform.position = target;
            waitTimer = waitDuration;
            isWaiting = true;
            if (destination == waypointA.position) {
                destination = waypointB.position;
            } else
            {
                destination = waypointA.position;
            }
        }
        else
        {
            transform.position += moveVector;
        }
    }
    public void setWaypoints(Transform A, Transform B)
    {
        waypointA = A;
        waypointB = B;
        destination = waypointA.position;
    }
}
