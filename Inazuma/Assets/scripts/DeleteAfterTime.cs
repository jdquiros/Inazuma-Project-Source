using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour {

    // Use this for initialization
    public float destroyDelay;
	void Start () {
        Destroy(gameObject, destroyDelay);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
