using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    public enum MenuState
    {
        MainMenu, LevelSelect, Credits, None
    }
    public static MenuState menuState;
    public Canvas levelSelectCanvas;
    public bool debug = false;
    public EventSystem eventSystem;
    public Button playButton;
    public Button levelSelectButton;
    public Button creditsButton;
    public Button quitButton;

    public GraphicColorLerp title;
    private AudioSource source;

    public UIScroller[] levelList;
    private int levelIndex = 0;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        menuState = MenuState.MainMenu;
        levelSelectCanvas.enabled = false;
    }
    void Start () {
        if (GameState.compareState(GameState.State.InGame))
        {
            skipToGame();
        } else if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.MainMenu;
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (menuState)
        {
            case (MenuState.MainMenu):
                updateMainMenu();
                break;
            case (MenuState.LevelSelect):
                updateLevelSelect();
                break;
        }
	}
    private void updateMainMenu()
    {
        if(eventSystem.currentSelectedGameObject == playButton.gameObject)
        { 
            playButton.gameObject.GetComponent<ColorOscillation>().startColorChange();
        } else
        {
            playButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();

        }
        if (eventSystem.currentSelectedGameObject == levelSelectButton.gameObject)
        {
            levelSelectButton.gameObject.GetComponent<ColorOscillation>().startColorChange();
        }
        else
        {
            levelSelectButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();

        }
        if (eventSystem.currentSelectedGameObject == creditsButton.gameObject)
        {
            creditsButton.gameObject.GetComponent<ColorOscillation>().startColorChange();
        }
        else
        {
            creditsButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();

        }
        if (eventSystem.currentSelectedGameObject == quitButton.gameObject)
        {
            quitButton.gameObject.GetComponent<ColorOscillation>().startColorChange();
        }
        else
        {
            quitButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();

        }
    }
    private void updateLevelSelect()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            incrementLevel();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            decrementLevel();
        }
    }
    private void incrementLevel()
    {
        if(levelIndex < levelList.Length - 1)
        {
            levelList[levelIndex].hideUI(1);
            levelList[levelIndex+1].revealUI(1);
            ++levelIndex;
        }
    }
    private void decrementLevel()
    {
        if(levelIndex > 0)
        {
            levelList[levelIndex].hideUI(-1);
            levelList[levelIndex - 1].revealUI(-1);
            --levelIndex;
        }
    }
    public void playButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            GameState.setState(GameState.State.InGame);
            menuState = MenuState.None;
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().setMovementState(1);

            playButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();
            levelSelectButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();
            creditsButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();
            quitButton.gameObject.GetComponent<ColorOscillation>().stopColorChange();

            playButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            levelSelectButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            creditsButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            quitButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            title.startColorChange();

            eventSystem.SetSelectedGameObject(null);
            if (!debug)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    private void skipToGame()
    {
        GameState.setState(GameState.State.InGame);
        menuState = MenuState.None;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().setMovementState(1);
        playButton.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        levelSelectButton.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        creditsButton.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        quitButton.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        title.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
        eventSystem.SetSelectedGameObject(null);

        if (!debug)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }


    }
    public void levelSelectButtonPress()
    {
        menuState = MenuState.LevelSelect;
        levelSelectCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(null);
    }
    public void creditsButtonPress()
    {

    }
    public void quitButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            Application.Quit();
        }
    }
    public void playSoundOnNavigation(AudioClip clip)
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
        {
            source.PlayOneShot(clip);
        }
    }
}
