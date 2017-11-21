using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    private Vector3 moveDirection;

	void Start () {
        if (moveDirection == Vector3.zero)
            moveDirection = Vector3.right;
        transform.GetComponent<Rigidbody2D>().velocity = moveDirection;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
