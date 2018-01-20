using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour {

    private Vector3 enemyPos;
    public float heightRange = .3f;
    public float periodSpeed = 3f;
    public bool randomOffset;
    private float periodOffset;
	// Use this for initialization
	void Start () {
        
        enemyPos = transform.position;
        if (randomOffset)
            periodOffset = Random.value * Mathf.PI * 2;

	}

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = enemyPos + new Vector3(0, Mathf.Sin(periodSpeed * Time.time + periodOffset) * heightRange, 0);
    }


}
