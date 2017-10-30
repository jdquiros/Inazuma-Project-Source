using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	public bool active = false;
	public static GameObject[] checkpoints;
    LevelState stateData;
	private bool wasEnabled = false;  //to prevent previously acquired checkpoints from being activated

    // Use this for initialization
    private void Awake()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        stateData = GetComponent<LevelState>();
        stateData.initialLoad();        //forces stateData to load its data, regardless of if its Awake() will be run in the future
        active = (stateData.getLoadState() == 1);  //1 is active, 0 is inactive
       
    }
    void Start () 
	{
        
	}

	public void enableCheckpoint () 
	{
        disableAllCheckpoints();
		enabled = true;
		print("Checkpoint enabled");
        stateData.setLoadState(1);
	}
    public void disableCheckpoint(GameObject checkpoint)
    {
        checkpoint.GetComponent<Checkpoint>().active = false;
        checkpoint.GetComponent<LevelState>().setLoadState(0);
    }
    private void disableAllCheckpoints()
    {
        foreach (GameObject checkpoint in checkpoints)
        {
            disableCheckpoint(checkpoint);
            //checkpoint.GetComponent<Checkpoint>().SetBool ("Active", false);  //for animator? we might use this later if we have animated checkpoint objects
        }
    }
	void OnTriggerEnter2D(Collider2D collision) 
	{
		if (collision.gameObject.tag == "Player" && !active) {
			enableCheckpoint ();
		}

	}

	public static Vector3 GetCurrentCheckpointPos() 
	{
		Vector3 result = new Vector3();       
        
		if (checkpoints != null) {
			foreach (GameObject checkpoint in checkpoints) {
				if (checkpoint.GetComponent<Checkpoint> ().active) {
					result = checkpoint.transform.position;
					break;
				}
			}
		}
		return result;
	}
}
