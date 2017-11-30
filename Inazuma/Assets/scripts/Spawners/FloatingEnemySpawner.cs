using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEnemySpawner : EnemySpawner {

    public override void spawnEnemy()
    {
        GameObject explosion = (GameObject)Instantiate(Resources.Load("RadialExplosionRed"));
        explosion.transform.position = transform.position;
        GameObject newEnemy = (GameObject)Instantiate(Resources.Load("Floating Enemy"));
        newEnemy.transform.position = transform.position;

        newEnemy.GetComponent<Enemy>().spawnedBy = this;

    }
}
