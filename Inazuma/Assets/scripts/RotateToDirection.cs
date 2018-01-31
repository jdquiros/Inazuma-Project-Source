using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToDirection : MonoBehaviour {

    // Use this for initialization
    private PlayerController controller;
    public float rotationSpeed;
    private float desiredAngle;
    private TrailRenderer tr;
	void Start () {
        controller = GetComponentInParent<PlayerController>();
        tr = GetComponentInChildren<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GameState.compareState(GameState.State.InGame))
        {
            desiredAngle = controller.getAimDirection() * 45f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, desiredAngle)), rotationSpeed * Time.deltaTime);

            if (Mathf.Abs(desiredAngle - transform.rotation.eulerAngles.z) < 5f)
            {
                tr.Clear();
                tr.enabled = false;
            }
            else
            {
                tr.enabled = true;
            }
        }
	}
}
