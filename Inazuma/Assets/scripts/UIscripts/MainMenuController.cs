﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    public enum MenuState
    {
        MainMenu, LevelSelect, Credits, None
    }
    public static MenuState menuState;
    private Canvas mainMenuCanvas;
    public Canvas levelSelectCanvas;
    public Canvas creditsCanvas;

    public bool debug = false;
    public EventSystem eventSystem;
    public Button playButton;
    public Button levelSelectButton;
    public Button creditsButton;
    public Button quitButton;

    public Button LSPlayButton;
    public Button LSBackButton;

    public Button creditsBackButton;

    public GraphicColorLerp title;
    private AudioSource source;

    public UIScroller[] levelList;
    public string[] levelNames;
    private int levelIndex = 0;

    public float creditsInitialDelay;
    public float creditsRepeatDelay;
    public UIRevealer[] nameList;
    public UIRevealer[] roleList;
    private IEnumerator creditsFunc;
    public float afterPressDelay;
    private bool allowInputs = true;
    private void Awake()
    {
        mainMenuCanvas = GetComponent<Canvas>();
        source = GetComponent<AudioSource>();
        menuState = MenuState.MainMenu;
        levelSelectCanvas.enabled = false;
        creditsCanvas.enabled = false;
        creditsFunc = revealCredits(creditsInitialDelay, creditsRepeatDelay);
    }
    void Start () {
        if (GameState.compareState(GameState.State.InGame))
        {
            skipToGame();
        } else if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.MainMenu;
            mainMenuCanvas.enabled = true;
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
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (allowInputs)
            {
                incrementLevel();
                StartCoroutine(preventInputAfterPress(afterPressDelay));
            }
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (allowInputs)
            {
                decrementLevel();
                StartCoroutine(preventInputAfterPress(afterPressDelay));
            }
        }
    }
    private void incrementLevel()
    {
        if(levelIndex < levelList.Length - 1)
        {
            levelList[levelIndex].hideUI(1);
            levelList[levelIndex+1].revealUI(1);
            ++levelIndex;
            source.PlayOneShot(source.clip);
        }
    }
    private void decrementLevel()
    {
        if(levelIndex > 0)
        {
            levelList[levelIndex].hideUI(-1);
            levelList[levelIndex - 1].revealUI(-1);
            --levelIndex;
            source.PlayOneShot(source.clip);

        }
    }
    private IEnumerator preventInputAfterPress(float duration)
    {
        allowInputs = false;
        yield return new WaitForSeconds(duration);
        allowInputs = true;
    }
    private IEnumerator revealCredits(float initialDelay, float repeatDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        for(int i = 0; i < nameList.Length; ++i)
        {
            nameList[i].revealUI();
            roleList[i].revealUI();
            yield return new WaitForSeconds(repeatDelay);
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
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.LevelSelect;
            levelSelectCanvas.enabled = true;
            mainMenuCanvas.enabled = false;
            eventSystem.SetSelectedGameObject(LSPlayButton.gameObject);
        }
    }
    public void creditsButtonPress()
    {
        menuState = MenuState.Credits;
        mainMenuCanvas.enabled = false;
        creditsCanvas.enabled = true;
        eventSystem.SetSelectedGameObject(creditsBackButton.gameObject);
        for(int i = 0; i < nameList.Length; ++i)
        {
            nameList[i].hideImmediately();
            roleList[i].hideImmediately();
        }
        StopCoroutine(creditsFunc);
        creditsFunc = revealCredits(creditsInitialDelay, creditsRepeatDelay);
        StartCoroutine(creditsFunc);
       
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
        if (GameState.compareState(GameState.State.MainMenu))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
            || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A))
            {
                source.PlayOneShot(clip);
            }
        }
    }
    //private IEnumerator playThenDropVolume(AudioClip clip, float initialDelay)
    //{
    //    source.volume
    //    source.PlayOneShot(clip);

    //}
    public void LSPlayButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            GameState.setState(GameState.State.InGame);
            menuState = MenuState.None;
            eventSystem.SetSelectedGameObject(null);
            SceneManager.LoadScene(levelNames[levelIndex]);
        }
    }
    public void LSBackButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.MainMenu;
            levelSelectCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            eventSystem.SetSelectedGameObject(playButton.gameObject);
        }
    }
    public void creditsBackButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.MainMenu;
            creditsCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            eventSystem.SetSelectedGameObject(playButton.gameObject);
        }
    }
}
