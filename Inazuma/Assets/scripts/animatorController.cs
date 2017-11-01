using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorController : MonoBehaviour {

	PlayerController playerController;
	Animator anim;
	Vector3 theScale;

	private void Awake() {
		playerController = GetComponentInParent<PlayerController> ();
		anim = GetComponent<Animator> ();
	}

	void Start () {
		theScale = transform.localScale;
	}
	
	void FixedUpdate () {
		float speed = playerController.getVelocity ().x;
		int facing = playerController.getFacingDirection ();

		anim.SetFloat ("Speed", Mathf.Abs (speed));
	
		//int facing -> 4 == left, 0 == right
		if (facing == 0) {
			transform.localScale = new Vector3 (theScale.x, transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(-theScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
