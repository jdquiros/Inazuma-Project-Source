using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class LevelWonMenuController : MonoBehaviour {

    // Use this for initialization
    private Canvas myCanvas;
    private LevelGoal goal;
    private AudioSource source;
    public GraphicColorLerp[] colorLerps;
    private EventSystem eventSystem;
    public GameObject continueButton;
    private SceneController sceneController;
    public bool allowInput = true;
	void Start () {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        source = GetComponent<AudioSource>();
        myCanvas = GetComponent<Canvas>();
        goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<LevelGoal>();
        myCanvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!myCanvas.enabled && goal.levelWon())
        {
            myCanvas.enabled = true;
            foreach(GraphicColorLerp g in colorLerps)
            {
                g.startColorChange();
            }
            eventSystem.SetSelectedGameObject(continueButton);
            
        } else if (myCanvas.enabled)
        {
            updateLevelWonUI();
        }
	}
    private void updateLevelWonUI()
    {

    }
    public void playSoundOnNavigation(AudioClip clip)
    {
        //needs to be updated for controller support
        if (allowInput)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
            {
                source.PlayOneShot(clip);
            }
        }

    }
    public void continueButtonPress()
    {
        if (allowInput)
        {
            GameState.setState(GameState.State.InGame);
            GameState.setLevel(++GameState.levelNumber);
            allowInput = false;
            StartCoroutine(sceneController.transitionThenLoad(true,"level_" + GameState.levelNumber));
        }
    }
    public void mainMenuButtonPress()
    {
        if (allowInput)
        {
            GameState.setState(GameState.State.MainMenu);
            GameState.setLevel(1);
            LevelData.resetAll();
            allowInput = false;
            StartCoroutine(sceneController.transitionThenLoad(true,"menu_and_level_1"));
        }
    }
    
}
