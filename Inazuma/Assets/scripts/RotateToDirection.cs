using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToDirection : MonoBehaviour {

    // Use this for initialization
    private PlayerController controller;
    public float rotationSpeed;
    private float desiredAngle;
	void Start () {
        controller = GetComponentInParent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        desiredAngle = controller.getAimDirection() * 45f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, desiredAngle)),rotationSpeed*Time.deltaTime);
	}
}
