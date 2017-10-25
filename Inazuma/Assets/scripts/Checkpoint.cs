using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public bool enabled = false;

	public static GameObject[] checkpoints;

	// Use this for initialization
	void Start () {
		if (checkpoints == null) {
			checkpoints = GameObject.FindGameObjectsWithTag ("Checkpoint");	
		}
	}
		
	private void enableCheckPoint () {
		foreach (GameObject checkpoint in checkpoints) {
			checkpoint.GetComponent<Checkpoint> ().enabled = false;
			//checkpoint.GetComponent<Checkpoint>().SetBool ("Active", false);  //for animator? we might use this later if we have animated checkpoint objects
		}
		enabled = true;
	}

	void onTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			enableCheckPoint ();
		}
	}

	public static Vector2 GetCurrentCheckpointPos() {
		Vector2 result = new Vector2 (0, 5);           //inital spawn position

		if (checkpoints != null) {
			foreach (GameObject checkpoint in checkpoints) {
				if (checkpoint.GetComponent<Checkpoint> ().enabled) {
					result = checkpoint.transform.position;
					break;
				}
			}
		}
		return result;
	}
}
