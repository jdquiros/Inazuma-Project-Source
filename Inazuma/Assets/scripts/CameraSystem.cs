using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

	private GameObject player;
	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax; //these variables wil clamp camear so it doesnt go off screen

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// LateUpdate is better than update for camera systems?
	void LateUpdate () {
		float x = Mathf.Clamp (player.transform.position.x, xMin, xMax);
		float y = Mathf.Clamp (player.transform.position.y, yMin, yMax);
		gameObject.transform.position = new Vector3 (x, y, gameObject.transform.position.z);
	}
}
	
