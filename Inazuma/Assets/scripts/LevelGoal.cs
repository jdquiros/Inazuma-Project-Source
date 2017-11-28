using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour {

    // Use this for initialization
    private bool isLevelWon = false;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isLevelWon = true;
            GameState.setState(GameState.State.LevelWon);
        }
    }
    public bool levelWon()
    {
        return isLevelWon;
    }
}
