using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEye : MonoBehaviour {

    // Use this for initialization
    public Vector2 randomLookDuration;
    private float randomLookTimer = 0;
    public float lookSpeed;
    private float desiredAngle = 0;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        if (randomLookTimer > 0)
        {
            randomLookTimer -= Time.deltaTime;
        }
        else
        {
            desiredAngle = Random.Range(0, 360);
            randomLookTimer = Random.Range(randomLookDuration.x, randomLookDuration.y);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, desiredAngle)), lookSpeed * Time.deltaTime);
    }
}
