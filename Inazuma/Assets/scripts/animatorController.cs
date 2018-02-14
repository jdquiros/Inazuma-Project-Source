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
		//anim.SetBool ("attacking", playerController.attacking()); //from our old "basic attack" - not in use
		anim.SetBool ("attacking", playerController.lungeAttacking ());
		anim.SetBool ("moving", playerController.moving ());
		anim.SetBool ("spawning", playerController.isSpawning ());
		anim.SetBool ("grounded", playerController.isGrounded ());
		anim.SetInteger ("movementState", (int)playerController.movementState);
		anim.SetBool ("dead", playerController.isDead ());


// 		movementState: Paralyzed, Free, Dash, Lunge, OnLadder, Hover, Grappled, Clinging
//		int equivalent:    0    ,   1 ,  2  ,  3   ,     4   ,  5   ,    6    ,    7


		//"lunging" = movementState 3;

		//int getfacingdir: 4 == left, 0 == right
		if (playerController.getFacingDirection () == 0) {
			transform.localScale = new Vector3 (theScale.x, transform.localScale.y, transform.localScale.z);
		} else {
			transform.localScale = new Vector3(-theScale.x, transform.localScale.y, transform.localScale.z);
		}



		// aimDirection:	Right, UpRight, Up, UpLeft, Left, DownLeft, Down, DownRight
		// int equivalent:    0  ,    1   ,  2,    3  ,  4  ,    5    ,   6 ,   7   


	
		if ((int)playerController.movementState == 6) { //if grappled
			switch (playerController.getAimDirection()) {
			case 1: //UpRight
				transform.localEulerAngles = new Vector3(0, 0, 45);
				print ("upright");
				break;
			case 2: //Up
				transform.localEulerAngles = new Vector3(0, 0, -90);				
				break;
			case 3: //UpLeft
				transform.localEulerAngles = new Vector3(0, 0, -45);				
				break;
			case 5: //DownLeft
				transform.localEulerAngles = new Vector3(0, 0, 45);				
				break;
			case 6: //Down
				transform.localEulerAngles = new Vector3(0, 0, -90);				
				break;
			case 7: //DownRight
				transform.localEulerAngles = new Vector3(0, 0, -45);				
				break;
			}
		}
		if ((int)playerController.movementState == 7) { //if clinging
			transform.localEulerAngles = new Vector3 (0, 0, 0);
		}
	}
}
