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
		anim.SetFloat ("xVel", Mathf.Abs (playerController.getVelocity().x));
		anim.SetFloat ("yVel", playerController.getVelocity().y);
		anim.SetBool ("attacking", playerController.attacking());
		anim.SetBool ("lunging", playerController.lungeAttacking ());
		anim.SetBool ("moving", playerController.moving ());
		anim.SetBool ("spawning", playerController.isSpawning ());
		anim.SetBool ("grounded", playerController.isGrounded ());
		anim.SetInteger ("movementState", (int)playerController.movementState);


// 		movementState: Paralyzed, Free, Dash, Lunge, OnLadder, Hover
//		int equivalent:    0    ,   1 ,  2  ,  3   ,     4   ,  5

		//int getfacingdir: 4 == left, 0 == right
		if (playerController.getFacingDirection () == 0) {
			transform.localScale = new Vector3 (theScale.x, transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(-theScale.x, transform.localScale.y, transform.localScale.z);
		}
	}
}
