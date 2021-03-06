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
    public float ySpeedFactor;
    public float lowSpeedFactor;
    public float lowSpeedDistance;
    private Vector3 distanceMoved;
    private bool inputReceived = false;
    private PlayerController playerController;
    public float extensionDuration;
    private float extensionTimer;
    public float extensionDistance;        
    private int playerDirection;
    public Vector2 xBounds;
    private Camera cam;
    private int direction;
    private int directionLastFrame;
    private PlayerInputHandler pInput;
	// Use this for initialization
	void Start () {
        cam = Camera.main;
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        pInput = player.gameObject.GetComponent<PlayerInputHandler>();
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

        if(Time.timeScale > 0.05f)
            unityPosition += (Vector3)playerController.distanceMoved();
        direction = playerController.getFacingDirection() == 4 ? -1 : 1;
        
        if (!GameState.compareState(GameState.State.MainMenu))
        {
            
            float speed = speedFactor;
            
            if (inputReceived)
            {
                
                extensionTimer = Mathf.Clamp(extensionTimer, 0, extensionDuration);
                float extend = Mathf.Lerp(0,extensionDistance,extensionTimer/extensionDuration);
                float desireX;
                desireX = (direction == 1) ? xBounds.x : xBounds.y;
                desireX += direction * extend;
                if (((direction == 1 && playerController.getVelocity().x > 0) 
                    || (direction == -1 && playerController.getVelocity().x < 0)))
                {
                    
                    extensionTimer += Time.deltaTime;
                }
                else
                {
                    extensionTimer -= Time.deltaTime;
                }
                moveObjectToPoint(player.position.x, desireX,speed);
                doYMovement(player.position.y, ySpeedFactor);
                //unityPosition = new Vector3(unityPosition.x, player.position.y, unityPosition.z);       //temporary
            } else
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.25f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.25f
                    || pInput.jumpButton(PlayerInputHandler.Action.Down)
                    || pInput.dashButton(PlayerInputHandler.Action.Down))
                {
                    //Only follow player after some input has been done
                    inputReceived = true;
                }

            }

        }
        if(direction != directionLastFrame)
        {
            extensionTimer = 0;
        }
        directionLastFrame = direction;
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
        if (Vector2.Distance(unityPosition, target) <= speed * Time.deltaTime)
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
    private void moveObjectToPoint(float targetObj, float viewPoint, float speed)
    {

        float targetPoint = cam.ViewportToWorldPoint(new Vector3(viewPoint,0)).x+(unityPosition-transform.position).x;
        float distance = Mathf.Abs(targetPoint- targetObj);
        if (distance < lowSpeedFactor*speed * Time.deltaTime)
        {
            distanceMoved = new Vector3((targetObj - targetPoint),distanceMoved.y);
            unityPosition += distanceMoved;
        }
        else
        {

            if (distance < lowSpeedDistance)
            {
                distanceMoved = Vector3.right*Mathf.Sign((targetObj - targetPoint)) * Mathf.Lerp(lowSpeedFactor * speed, speed, distance / lowSpeedDistance) * Time.deltaTime;
            } else
            {
                distanceMoved = Vector3.right*Mathf.Sign((targetObj - targetPoint)) * speed * Time.deltaTime;

            }
            unityPosition += distanceMoved;
        }
    }
    private void doYMovement(float targetObj, float speed)
    {
        float targetPoint = cam.ViewportToWorldPoint(new Vector3(0,0.5f)).y + (unityPosition - transform.position).y;
        float distance = Mathf.Abs(targetPoint - targetObj);
        if (distance < speed * Time.deltaTime)
        {
            distanceMoved = new Vector3(distanceMoved.x,(targetObj-targetPoint));
            unityPosition += distanceMoved;
        }
        else
        {
            distanceMoved = Vector3.up * Mathf.Sign((targetObj - targetPoint)) * speed * Time.deltaTime;
            unityPosition += distanceMoved;
        }
    }
    public Vector3 getDistanceMoved()
    {
        return distanceMoved;
    }
    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

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
        float extend = Mathf.Lerp(0, extensionDistance, extensionTimer / extensionDuration);
        float desireX;
        desireX = (playerDirection == 1) ? xBounds.x : xBounds.y;
        desireX += playerDirection * extend;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(cam.ViewportToWorldPoint(new Vector3(desireX,.5f)),.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, lowSpeedDistance);
        
    }
}
	
