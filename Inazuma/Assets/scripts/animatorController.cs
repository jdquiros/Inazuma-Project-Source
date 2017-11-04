using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatorController : MonoBehaviour {

	private PlayerController playerController;
	private Animator anim;
	private Vector3 theScale;

	private void Awake() {
		playerController = GetComponentInParent<PlayerController> ();
		anim = GetComponent<Animator> ();
	}

	void Start () {
		theScale = transform.localScale;
	}

	void FixedUpdate () {
		float xVel = playerController.getVelocity ().x;
		float yVel = playerController.getVelocity ().y;
		int facing = playerController.getFacingDirection ();

		anim.SetFloat ("xVel", Mathf.Abs (xVel));
		anim.SetFloat ("yVel", yVel);
	
		//if (playerController....attacking())
		//anim.SetBool("attacking", true);
		//...

		//int facing: 4 == left, 0 == right
		if (facing == 0) {
			transform.localScale = new Vector3 (theScale.x, transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(-theScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
