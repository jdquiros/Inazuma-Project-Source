using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HittableObject : MonoBehaviour {

	// Use this for initialization

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public abstract void hitByPlayer(Vector3 playerPos);
}
