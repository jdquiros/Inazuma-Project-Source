using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeScript : MonoBehaviour {

    // Use this for initialization
    public float activationDistance;
    public Vector2 randomLookDuration;
    private float randomLookTimer = 0;
    public float lookSpeed;
    private float desiredAngle = 0;
    private Transform playerTransform;
	void Start () {
        desiredAngle = 0;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(transform.position, playerTransform.position) < activationDistance)
        {

            desiredAngle = Mathf.Atan2(playerTransform.position.y-transform.position.y,playerTransform.position.x-transform.position.x)*Mathf.Rad2Deg;
        } else
        {
            if(randomLookTimer > 0)
            {
                randomLookTimer -= Time.deltaTime;
            } else
            {
                desiredAngle = Random.Range(0, 360);
                randomLookTimer = Random.Range(randomLookDuration.x, randomLookDuration.y);
            }
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, desiredAngle)), lookSpeed * Time.deltaTime);
    }
    
}
