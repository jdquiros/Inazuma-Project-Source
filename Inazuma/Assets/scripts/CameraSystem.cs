using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

	private Transform player;
	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax; //these variables wil clamp camear so it doesnt go off screen
    private Vector3 targetPos;
    public float speedFactor = 0;
    private Vector3 distanceMoved;
    private bool inputReceived = false;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        if(!GameState.compareState(GameState.State.MainMenu))
            transform.position = new Vector3(Checkpoint.GetCurrentCheckpointPos().x, Checkpoint.GetCurrentCheckpointPos().y, gameObject.transform.position.z);
    }
	
	// LateUpdate is better than update for camera systems?
	void LateUpdate () {
        if (!GameState.compareState(GameState.State.MainMenu))
        {
            if (inputReceived)
            {
                float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
                float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
                targetPos = new Vector3(x, y, gameObject.transform.position.z);
                moveToPoint(targetPos);
            } else
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.25f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.25f)
                {
                    inputReceived = true;
                }

            }

        }
        
	}
    private void moveToPoint(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) < speedFactor * Time.deltaTime)
        {
            distanceMoved = (target - transform.position);
            transform.position = target;
        }
        else
        {
            distanceMoved = (target - transform.position) * speedFactor * Time.deltaTime;
            transform.position += distanceMoved;
        }
        
    }
    public Vector3 getDistanceMoved()
    {
        return distanceMoved;
    }
}
	
