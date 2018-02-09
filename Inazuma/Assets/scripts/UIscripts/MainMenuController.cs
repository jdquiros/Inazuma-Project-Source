using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    // Use this for initialization
    public enum MenuState
    {
        MainMenu, LevelSelect, Credits, Options, None
    }
    public static MenuState menuState;
    private Canvas mainMenuCanvas;
    public Canvas levelSelectCanvas;
    public Canvas creditsCanvas;
    public Canvas optionsCanvas;
    public Canvas transitionCanvas;

    public bool debug = false;
    private EventSystem eventSystem;
    private SceneController sceneController;
    public Button playButton;
    public Button levelSelectButton;
    public Button creditsButton;
    public Button quitButton;
    public Button optionsButton;
    public Button optionsDefaultButton;



    public Button creditsBackButton;

    public MatchFontSizes optionsMatchSizes;
    public GraphicColorLerp title;
    public GraphicColorLerp background;
    private AudioSource source;

    public float creditsInitialDelay;
    public float creditsRepeatDelay;
    public UIRevealer[] nameList;
    public UIRevealer[] roleList;
    private IEnumerator creditsFunc;
    public float afterPressDelay;
    private bool allowInputs = true;
    public ButtonOutlineController[] outlineList;
    private float gameStartDelay = 0.2f;
    private void Awake()
    {
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        mainMenuCanvas = GetComponent<Canvas>();
        source = GetComponent<AudioSource>();
        menuState = MenuState.MainMenu;
        levelSelectCanvas.enabled = false;
        creditsCanvas.enabled = false;
        optionsCanvas.enabled = false;
        creditsFunc = revealCredits(creditsInitialDelay, creditsRepeatDelay);
        if (GameState.compareState(GameState.State.InGame) || SceneManager.GetActiveScene().name == "menu_and_level_1")
        {
            skipToGame();
        }
        else if (GameState.compareState(GameState.State.MainMenu))
        {
            if (SceneManager.GetActiveScene().name == "menu_and_level_1")
            {
                menuState = MenuState.MainMenu;
                mainMenuCanvas.enabled = true;
            }
            else
            {
                menuState = MenuState.None;
                mainMenuCanvas.enabled = false;
                skipToGame();
            }
        }
    }
    void Start () {

        
        outlineList[0].resetOutline();
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
        if (gameStartDelay <= 0) { 
            gameStartDelay = 0;
            if (eventSystem.currentSelectedGameObject == playButton.gameObject)
            {
                outlineList[0].drawOutline();
            }
            else
            {
                outlineList[0].resetOutline();

            }
            if (eventSystem.currentSelectedGameObject == levelSelectButton.gameObject)
            {
                outlineList[1].drawOutline();
            }
            else
            {
                outlineList[1].resetOutline();

            }
            if (eventSystem.currentSelectedGameObject == optionsButton.gameObject)
            {
                outlineList[2].drawOutline();
            }
            else
            {
                outlineList[2].resetOutline();

            }
            if (eventSystem.currentSelectedGameObject == creditsButton.gameObject)
            {
                outlineList[3].drawOutline();
            }
            else
            {
                outlineList[3].resetOutline();

            }
            if (eventSystem.currentSelectedGameObject == quitButton.gameObject)
            {
                outlineList[4].drawOutline();
            }
            else
            {
                outlineList[4].resetOutline();

            }
        } else {
            gameStartDelay -= Time.deltaTime;

        }
    }
    //private void updateLevelSelect()
    //{
    //    if (Input.GetAxis("Horizontal") > 0.05f)
    //    {
    //        if (allowInputs)
    //        {
    //            incrementLevel();
    //            StartCoroutine(preventInputAfterPress(afterPressDelay));
    //        }
    //    } else if (Input.GetAxis("Horizontal") < -0.05f)
    //    {
    //        if (allowInputs)
    //        {
    //            decrementLevel();
    //            StartCoroutine(preventInputAfterPress(afterPressDelay));
    //        }
    //    }
    //}
    //private void incrementLevel()
    //{
    //    if(levelIndex < levelList.Length - 1)
    //    {
    //        levelList[levelIndex].hideUI(1);
    //        levelList[levelIndex+1].revealUI(1);
    //        ++levelIndex;
    //        source.PlayOneShot(source.clip);
    //        LSRightArrow.moveRectTransform(new Vector2(-.05f, 0));
    //        if(levelIndex >= levelList.Length - 1)
    //        {
    //            LSRightArrow.gameObject.GetComponent<Image>().enabled = false;
    //        } 
    //        LSLeftArrow.gameObject.GetComponent<Image>().enabled = true;

            
    //    }
    //}
    //private void decrementLevel()
    //{
    //    if(levelIndex > 0)
    //    {
    //        levelList[levelIndex].hideUI(-1);
    //        levelList[levelIndex - 1].revealUI(-1);
    //        --levelIndex;
    //        source.PlayOneShot(source.clip);
    //        LSLeftArrow.moveRectTransform(new Vector2(.05f, 0));
    //        if(levelIndex == 0)
    //        {
    //            LSLeftArrow.gameObject.GetComponent<Image>().enabled = false;
    //        } 
    //        LSRightArrow.gameObject.GetComponent<Image>().enabled = true;

            
    //    }
    //}
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

            playButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            levelSelectButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            creditsButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            quitButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            optionsButton.gameObject.GetComponent<GraphicColorLerp>().startColorChange();
            title.startColorChange();
            background.startColorChange();
            resetAllOutlines();
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
        resetAllOutlines();
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
            resetAllOutlines();
        }
    }
    public void creditsButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.Credits;
            mainMenuCanvas.enabled = false;
            creditsCanvas.enabled = true;
            eventSystem.SetSelectedGameObject(creditsBackButton.gameObject);
            for (int i = 0; i < nameList.Length; ++i)
            {
                nameList[i].hideImmediately();
                roleList[i].hideImmediately();
            }
            StopCoroutine(creditsFunc);
            creditsFunc = revealCredits(creditsInitialDelay, creditsRepeatDelay);
            StartCoroutine(creditsFunc);
            resetAllOutlines();
        }
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
    //public void LSPlayButtonPress()
    //{
    //    if (GameState.compareState(GameState.State.MainMenu))
    //    {
    //        GameState.setState(GameState.State.InGame);
    //        menuState = MenuState.None;
    //        eventSystem.SetSelectedGameObject(null);
    //        StartCoroutine(sceneController.transitionThenLoad(true,levelNames[levelIndex]));
    //    }
    //}
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
    public void optionsButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.Options;
            optionsCanvas.enabled = true;
            mainMenuCanvas.enabled = false;
            eventSystem.SetSelectedGameObject(optionsDefaultButton.gameObject);
            optionsMatchSizes.resizeText();
            resetAllOutlines();
        }
    }
    public void optionsBackButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            menuState = MenuState.MainMenu;
            optionsCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            eventSystem.SetSelectedGameObject(playButton.gameObject);
        }
    }
    
    private void resetAllOutlines()
    {
        for(int i = 0; i < outlineList.Length; i++)
        {
            outlineList[i].resetOutline();
        }
    }
}
