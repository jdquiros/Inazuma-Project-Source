using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {

    // Use this for initialization
    public bool paused = false;
    public KeyCode pauseButton = KeyCode.P;
    private Canvas myCanvas;
	void Start () {
        myCanvas = GetComponent<Canvas>();
        myCanvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(GameState.gameState == GameState.State.InGame)
        {
            if(!paused && Input.GetKeyDown(pauseButton))
            {
                pause();
            } else if(paused && Input.GetKeyDown(pauseButton))
            {
                unPause();
            }
        }
	}
    private void pause()
    {
        paused = true;
        Time.timeScale = 0;
        myCanvas.enabled = true;
    }
    private void unPause()
    {
        paused = false;
        Time.timeScale = 1;
        myCanvas.enabled = false;
    }
}
