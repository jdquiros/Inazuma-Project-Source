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
    private PlayerController playerController;
    public float extensionDistance;        
    private int playerDirection;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();
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
                playerDirection = playerController.getFacingDirection() == 0 ? 1 : -1;
                
                float x = Mathf.Clamp(player.transform.position.x+
                    playerDirection*extensionDistance, xMin, xMax);
                float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
				targetPos = new Vector3(x, y, gameObject.transform.position.z);
				moveToPoint (targetPos);
            } else
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.25f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.25f
                    || Input.GetButtonDown("ctrlLayout"+GameState.controlLayout+"Jump") 
                    || Input.GetButtonDown("keyLayout"+GameState.keyboardLayout+"Jump"))
                {
                    //Only follow player after some input has been done
                    inputReceived = true;
                }

            }

        }
        
	}

	private float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
		return adjustedUnityUnits;
	}

    private void moveToPoint(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) < speedFactor * Time.deltaTime)
        {
            distanceMoved = (target - transform.position);
			distanceMoved = new Vector3 (RoundToNearestPixel (distanceMoved.x, Camera.main), RoundToNearestPixel (distanceMoved.y, Camera.main), this.transform.position.z);
            transform.position = target; 
        }
        else
        {
            distanceMoved = (target - transform.position) * speedFactor * Time.deltaTime;
			distanceMoved = new Vector3 (RoundToNearestPixel (distanceMoved.x, Camera.main), RoundToNearestPixel (distanceMoved.y, Camera.main), this.transform.position.z);
			Vector3 newPos = transform.position + distanceMoved;
			Vector3 roundPos = new Vector3 (RoundToNearestPixel (newPos.x, Camera.main), RoundToNearestPixel (newPos.y, Camera.main), this.transform.position.z);
			transform.position = roundPos;
        }
        
    }
    public Vector3 getDistanceMoved()
    {
        return distanceMoved;
    }
    
}
	
