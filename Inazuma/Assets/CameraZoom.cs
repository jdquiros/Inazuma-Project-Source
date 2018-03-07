using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public float zoomSize;   //camera size to zoom to. camera size is default 12
	public float smoothTime; //how long to transition from current size to zoom sizes
	public float delay;      //how long to wait before beginning the transition

	private GameObject cam;
	private float originalSize; //original camera size

	bool triggered;
	bool playOnce;
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
			cam.GetComponent<Camera>().orthographicSize = 
				Mathf.Lerp (cam.GetComponent<Camera>().orthographicSize, zoomSize, Time.deltaTime / smoothTime);
		}
		if (!triggered) {
			cam.GetComponent<Camera>().orthographicSize = 
				Mathf.Lerp (cam.GetComponent<Camera>().orthographicSize, originalSize, Time.deltaTime / smoothTime);
		}
	}
}
