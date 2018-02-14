using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParticleSystem : MonoBehaviour {

    // Use this for initialization
    private LaserTurret turret;
	void Start () {
        turret = GetComponentInParent<LaserTurret>();
	}
	
	// Update is called once per frame
	void Update () {
        if (turret.firing())
            transform.position = turret.getLaserEnd();
        else
            transform.position = new Vector3(-5000, -5000);
	}
}
