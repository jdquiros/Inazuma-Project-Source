using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryTurret : MonoBehaviour {

    // Use this for initialization

    //CALCULATIONS ONLY GOOD WHEN THERE IS NO HEIGHT DIFFERENCE
    public Transform projectileType;
    public float fireRate;                   //shots per minute
    private Transform player;
    private float horzRangeToPlayer;
    private float launchAngle;                  //in radians
    public float launchVelocity;
    public float projectileGravity;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("spawnProjectile", 0, 60f / fireRate);
	}
	
	// Update is called once per frame
	void Update () {
        calculateLaunchAngle();
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, launchAngle*Mathf.Rad2Deg));
	}
    private void spawnProjectile()
    {
        GameObject projectile = Instantiate(projectileType).gameObject;
        projectile.transform.position = transform.position;
        projectile.GetComponent<TrailRenderer>().Clear();
        projectile.GetComponent<LobbedProjectile>().gravity = projectileGravity;
        projectile.GetComponent<LobbedProjectile>().setVelocity(transform.right * launchVelocity);
    }
    private void calculateLaunchAngle()
    {
        //formula: angle = (1/2) (inverse SIN(gravity*distance/(launchVelocity^2)))
        if(launchVelocity == 0)
        {
            //prevent division by zero
            print("launch velocity cannot be zero");
            return;
        }
        horzRangeToPlayer = (player.position.x - transform.position.x);
       
        launchAngle = projectileGravity * horzRangeToPlayer;
        launchAngle /= launchVelocity * launchVelocity;
        launchAngle = Mathf.Asin(launchAngle);
        launchAngle /= 2;
        launchAngle = (Mathf.PI / 2) - launchAngle;
        if (float.IsNaN(launchAngle))                   //there is no valid launchAngle.  This could be because player is out of range
            launchAngle = Mathf.PI / 4;
    }
}
