using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEnemy : MonoBehaviour {

    // Use this for initialization
    public float rotationSpeed;
    private GroundPatrolling enemy;
	void Start () {
        enemy = GetComponentInParent<GroundPatrolling>();
	}
	
	// Update is called once per frame
	void Update () {
        if (enemy.xVelocity > 0 && !enemy.waiting())
        {
            transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        } else if(enemy.xVelocity < 0 && !enemy.waiting())
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
    }
}
