using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEnemySpawner : EnemySpawner {

    public override void spawnEnemy()
    {
        GameObject explosion = (GameObject)Instantiate(Resources.Load("RadialExplosionRed"),transform.position,transform.rotation);
        GameObject newEnemy = Instantiate(spawnPrefab,transform.position,transform.rotation).gameObject;

        newEnemy.GetComponent<Enemy>().spawnedBy = this;

    }
}
