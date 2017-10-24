using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateInAir : MonoBehaviour {

    // Use this for initialization
    public float rotationSpeed = 0;
    PlayerController playerController;
	void Start () {
        playerController = gameObject.GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!playerController.isGrounded() && !playerController.isDashing)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed));
        } else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
	}
}
