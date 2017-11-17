using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour {

    public Vector3 enemyPos;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = enemyPos + new Vector3(0, Mathf.Sin(3 * Time.time) / 2, 0);
    }


}
