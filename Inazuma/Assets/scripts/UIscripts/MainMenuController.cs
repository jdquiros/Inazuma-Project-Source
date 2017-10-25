using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    private enum MenuState
    {
        MainMenu, LevelSelect, Credits, None
    }
    private MenuState menuState = MenuState.MainMenu;
    public bool debug = false;
    public EventSystem eventSystem;
    public Button playButton;
    public Button levelSelectButton;
    public Button creditsButton;
    public Button quitButton;

    public GraphicColorLerp title;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        switch (menuState)
        {
            case (MenuState.MainMenu):
                updateMainMenu();
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
    public void playButtonPress()
    {
        GameState.setState(GameState.State.Gameplay);
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
    public void levelSelectButtonPress()
    {

    }
    public void creditsButtonPress()
    {

    }
    public void quitButtonPress()
    {
        Application.Quit();
    }
}
