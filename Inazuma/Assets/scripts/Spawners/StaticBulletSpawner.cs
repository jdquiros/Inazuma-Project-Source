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

	private Vector3 angles;

	void Start () {
        InvokeRepeating("fireBullet", 0, 60 / fireRate);

		angles = transform.eulerAngles;    //finds the angle to shoot based on the Turret's rotation
		if (angles.z == 0.0f) {
			angles = Vector3.right;
		} else if (angles.z == 90.0f) {
			angles = Vector3.up;
		} else if (angles.z == 180.0f) {
			angles = Vector3.left;
		} else {
			angles = Vector3.down;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void fireBullet()
    {
		GameObject bullet = Instantiate(bulletType, transform.position, transform.rotation).gameObject;

		bullet.GetComponent<Bullet> ().setVelocity (angles, 10.0f); //sets shooting angle. 10 is default speed
	}
}
