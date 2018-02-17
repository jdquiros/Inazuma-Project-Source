using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour {

    // Use this for initialization
    public Vector3 dirVector;
    public float speed;
    public bool stopAfterTime;
    public float stopDelay;
    private float stopTimer;
	void Start () {
        setDirVector(dirVector);
        stopTimer = stopDelay;
	}
	
	// Update is called once per frame
	void Update () {
        stopTimer -= Time.deltaTime;
        if(stopTimer > 0)
            transform.position += dirVector * speed * Time.deltaTime;
	}
    public void setDirVector(Vector3 v)
    {
        dirVector = v.normalized;
    }
    
}
