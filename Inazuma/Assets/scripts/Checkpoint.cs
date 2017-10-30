using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public bool enabled = false;
	public static GameObject[] checkpoints;

	private bool wasEnabled = false;  //to prevent previously acquired checkpoints from being activated

	// Use this for initialization
	void Start () 
	{
		if (checkpoints == null) {
			checkpoints = GameObject.FindGameObjectsWithTag ("Checkpoint");	
		}
	}

	private void enableCheckPoint () 
	{
		foreach (GameObject checkpoint in checkpoints) {
			checkpoint.GetComponent<Checkpoint> ().enabled = false;
			//checkpoint.GetComponent<Checkpoint>().SetBool ("Active", false);  //for animator? we might use this later if we have animated checkpoint objects
		}
		enabled = true;
		wasEnabled = true;
		print("Checkpoint enabled");
	}

	void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.gameObject.tag == "Player" && !wasEnabled) {
			enableCheckPoint ();
		}

	}

	public static Vector3 GetCurrentCheckpointPos() 
	{
		Vector3 result = new Vector3();       

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
