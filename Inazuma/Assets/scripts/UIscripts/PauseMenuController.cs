using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class PauseMenuController : MonoBehaviour {

    // Use this for initialization
    public static bool paused;
    public KeyCode pauseButton = KeyCode.P;
    public string mainMenuSceneName = "menu_and_level_1";
    public GameObject resumeButton;
    public EventSystem eventSystem;

    private Canvas myCanvas;
    private AudioSource source;
	void Start () {
        paused = false;
        myCanvas = GetComponent<Canvas>();
        myCanvas.enabled = false;
        source = GetComponent<AudioSource>();
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
        eventSystem.SetSelectedGameObject(resumeButton);
    }
    private void unPause()
    {
        paused = false;
        Time.timeScale = 1;
        myCanvas.enabled = false;
        eventSystem.SetSelectedGameObject(null);
    }
    public void mainMenuButtonPress()
    {
        unPause();
        LevelData.resetAll();

        GameState.setState(GameState.State.MainMenu);
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void resumeButtonPress()
    {
        unPause();
    }
    public void playSoundOnNavigation(AudioClip clip)
    {
       
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
            {
                Time.timeScale = 1;
                source.PlayOneShot(clip);
                if(paused)
                    Time.timeScale = 0;
            }
        
    }
}
