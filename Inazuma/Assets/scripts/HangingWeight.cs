using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingWeight : HittableObject {

    // Use this for initialization
    public Rigidbody2D rb;
    public float moveVelocity;
    public Transform prefabEffect;
    private TrailRenderer tr;
	void Start () {
        tr = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame

    public override void hitByPlayer(Vector3 playerPos)
    {
        if(playerPos.x < transform.position.x)
        {
            rb.velocity = new Vector2(-moveVelocity, rb.velocity.y);
        } else
        {
            rb.velocity = new Vector2(moveVelocity, rb.velocity.y);
        }
        GameObject obj = Instantiate(prefabEffect, transform.position, Quaternion.Euler(0,0,0)).gameObject;
        obj.transform.parent = null;
        tr.Clear();
    }
}
