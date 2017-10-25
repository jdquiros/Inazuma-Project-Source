using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempEnemy : MonoBehaviour {

    // Use this for initialization
    int state;
	void Start () {
        state = GetComponent<LevelState>().getLoadState();
        if(state == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
            state = 2;
        } else if (state == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
            state = 1;

        } else
        {
            state = 1;
        }
        GetComponent<LevelState>().setLoadState(state);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
