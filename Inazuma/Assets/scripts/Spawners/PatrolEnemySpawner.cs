using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemySpawner : EnemySpawner {


    // Use this for initialization
    public Transform waypointA, waypointB;
    
	
	// Update is called once per frame
	
    public override void spawnEnemy()
    {
        GameObject explosion = (GameObject)Instantiate(Resources.Load("RadialExplosionRed"));
        explosion.transform.position = transform.position;
        GameObject newEnemy = (GameObject)Instantiate(Resources.Load("Patrolling Enemy"));
        newEnemy.transform.position = transform.position;
        newEnemy.GetComponent<Enemy>().spawnedBy = this;
        newEnemy.GetComponent<Patrolling>().setWaypoints(waypointA, waypointB);

    }
}
