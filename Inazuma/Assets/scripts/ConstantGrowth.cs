using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantGrowth : MonoBehaviour {

    // Use this for initialization
    public float growthRate;
    private Vector3 growthVector;
	void Start () {
        growthVector = new Vector3(growthRate, growthRate, 0);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += growthVector*Time.deltaTime;
	}
}
