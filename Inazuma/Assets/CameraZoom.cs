using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public float zoomSize;   //camera size to zoom to. camera size is default 12. it doesnt zoom let you zoom lower than ~7 for some reason
	public float zoomY; 	//how far to move up/down +/-
	public float zoomX;     //how far to move right/left +/-
		
	public float smoothTime; //how long to transition from current size to zoom sizes
	public float delay;      //how long to wait before beginning the transition

	private GameObject cam;
	private float originalSize; //original camera size

	bool triggered;
	bool hasTriggered; 


	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("Main Camera");
		triggered = false;
		originalSize = cam.GetComponent<Camera> ().orthographicSize;
		}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player" && !triggered) {
			triggered = true;
			StartCoroutine (TheDelay (delay));
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Player" && triggered) {
			triggered = false;
			StartCoroutine (TheDelay (delay));
		}
	}

	public IEnumerator TheDelay(float seconds) {
		yield return new WaitForSeconds (seconds);
	}



	// Update is called once per frame
	void Update () {
		if (triggered) {
			if (!hasTriggered) {
				cam.transform.position = new Vector3 (cam.transform.position.x + zoomX, cam.transform.position.y + zoomY, cam.transform.position.z);
			hasTriggered = true;
			}
			cam.GetComponent<Camera>().orthographicSize = 
				Mathf.Lerp (cam.GetComponent<Camera>().orthographicSize, zoomSize, Time.deltaTime / smoothTime);
		}
		if (!triggered) {
			if (hasTriggered) {
				cam.transform.position = new Vector3 (cam.transform.position.x - zoomX, cam.transform.position.y - zoomY, cam.transform.position.z);
				hasTriggered = false;
			}
			cam.GetComponent<Camera>().orthographicSize = 
				Mathf.Lerp (cam.GetComponent<Camera>().orthographicSize, originalSize, Time.deltaTime / smoothTime);
		}
	}
}
