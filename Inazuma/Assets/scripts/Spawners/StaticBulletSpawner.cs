using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBulletSpawner : MonoBehaviour {

    //this script always makes the bullet fire to the right.  reangle the gameobject to change its direction
    //this script constantly fires.  Another script is necessary for conditional firing


    // Use this for initialization
    public float fireRate;              //shots per minute
    public Transform bulletType;        //drag prefab here
    public float xOffset;

	void Start () {
        InvokeRepeating("fireBullet", 0, 60 / fireRate);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void fireBullet()
    {
        GameObject bullet = Instantiate(bulletType).gameObject;
        bullet.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, 0);
    }
}
