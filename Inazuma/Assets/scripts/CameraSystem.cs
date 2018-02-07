using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

    private Vector3 unityPosition;
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
    public Vector2 xBounds;
    private Camera cam;
	// Use this for initialization
	void Start () {
        cam = Camera.main;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        if (!GameState.compareState(GameState.State.MainMenu))
        {
            transform.position = new Vector3(Checkpoint.GetCurrentCheckpointPos().x, Checkpoint.GetCurrentCheckpointPos().y, gameObject.transform.position.z);
        }
        unityPosition = transform.position;


    }

    // LateUpdate is better than update for camera systems?
    void LateUpdate () {
        float left = cam.ViewportToWorldPoint(new Vector3(xBounds.x,0)).x;
        float right = cam.ViewportToWorldPoint(new Vector3(xBounds.y, 0)).x;
        
        unityPosition += player.GetComponent<PlayerController>().getVelocity().x * Vector3.right * Time.deltaTime;
       
        
        if (!GameState.compareState(GameState.State.MainMenu))
        {
            
            float speed = speedFactor;
            /*
            if(player.position.x < left)
            {
                speed = Mathf.Abs(player.GetComponent<PlayerController>().getVelocity().x);
            } else if(player.position.x > right)
            {
                speed = Mathf.Abs(player.GetComponent<PlayerController>().getVelocity().x);
            } 
            */
            if (inputReceived)
            {
                /*
                playerDirection = playerController.getFacingDirection() == 0 ? 1 : -1;
                switch (playerDirection)
                {
                    case 1:

                        break;
                    case -1:

                        break;
                }
                float x = Mathf.Clamp(player.transform.position.x+
                    playerDirection*extensionDistance, xMin, xMax);
                float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
				targetPos = new Vector3(x, y, gameObject.transform.position.z);
				moveToPoint (targetPos,speed);
                */
                playerDirection = playerController.getFacingDirection() == 0 ? 1 : -1;
                float extend = Mathf.Clamp(Mathf.Abs(player.GetComponent<PlayerController>().getVelocity().x),0,extensionDistance);
                float desireX;
                desireX = (playerDirection == 1) ? xBounds.x : xBounds.y;
                desireX += playerDirection * extend;
                moveObjectToPoint(player.position, new Vector3(desireX, .5f),speed);
                unityPosition = new Vector3(unityPosition.x, player.position.y, unityPosition.z);       //temporary
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
        
        transform.position = new Vector3(RoundToNearestPixel(unityPosition.x,cam),RoundToNearestPixel(unityPosition.y,cam),transform.position.z);
	}

	private float RoundToNearestPixel(float unityUnits, Camera viewingCamera)
	{
		float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
		valueInPixels = Mathf.Round(valueInPixels);
		float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
		return adjustedUnityUnits;
	}

    private void moveToPoint(Vector3 target, float speed)
    {
        if (Vector2.Distance(unityPosition, target) < speed * Time.deltaTime)
        {
            distanceMoved = (target - unityPosition);
            unityPosition = target; 
        }
        else
        {
            distanceMoved = (target - unityPosition).normalized * speed * Time.deltaTime;
			distanceMoved = new Vector3 (RoundToNearestPixel (distanceMoved.x, Camera.main), RoundToNearestPixel (distanceMoved.y, Camera.main), this.transform.position.z);
			Vector3 newPos = unityPosition + distanceMoved;
			unityPosition = newPos;
        }
        
    }
    private void moveObjectToPoint(Vector3 targetObj, Vector3 viewPoint, float speed)
    {

        Vector3 targetPoint = cam.ViewportToWorldPoint(viewPoint);
        if (Vector2.Distance(targetPoint, targetObj) < speed * Time.deltaTime)
        {
            distanceMoved = (Vector2)(targetObj - targetPoint);
            unityPosition += distanceMoved;
        }
        else
        {
            distanceMoved = ((Vector2)(targetObj - targetPoint)).normalized * speed * Time.deltaTime;
            unityPosition += distanceMoved;
        }
    }
    public Vector3 getDistanceMoved()
    {
        return distanceMoved;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        //screen center
        Gizmos.DrawWireSphere(transform.position, 1);
        Vector3 leftBounds, rightBounds;
        leftBounds = Camera.main.ViewportToWorldPoint(new Vector3(xBounds.x, .5f, 0));
        rightBounds = Camera.main.ViewportToWorldPoint(new Vector3(xBounds.y, .5f, 0));
        Gizmos.DrawWireSphere(rightBounds, .5f);
        Gizmos.DrawWireSphere(leftBounds,.5f);
        Gizmos.DrawWireSphere(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)),.5f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(player.position + Vector3.down * 1000, player.position + Vector3.up * 1000);
        playerDirection = playerController.getFacingDirection() == 0 ? 1 : -1;
        float extend = Mathf.Clamp(Mathf.Abs(player.GetComponent<PlayerController>().getVelocity().x), 0, extensionDistance);
        float desireX;
        desireX = (playerDirection == 1) ? xBounds.x : xBounds.y;
        desireX += playerDirection * extend;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(cam.ViewportToWorldPoint(new Vector3(desireX,.5f)),.5f);
    }
}
	
