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

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").transform;
        float x = Mathf.Clamp(player.transform.position.x, xMin, xMax);
        float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
        transform.position = new Vector3(x, y, gameObject.transform.position.z);
    }
	
	// LateUpdate is better than update for camera systems?
	void LateUpdate () {
		float x = Mathf.Clamp (player.transform.position.x, xMin, xMax);
		float y = Mathf.Clamp (player.transform.position.y, yMin, yMax);
		targetPos = new Vector3 (x, y, gameObject.transform.position.z);
        moveToPoint(targetPos);
	}
    private void moveToPoint(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) < speedFactor * Time.deltaTime)
        {
            transform.position = target;
        }
        else
        {
            transform.position += (target - transform.position) * speedFactor * Time.deltaTime;
        }
        
    }
}
	
