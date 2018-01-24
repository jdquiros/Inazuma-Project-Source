using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectController : MonoBehaviour {

    // Use this for initialization
    private enum Direction
    {
        Left, Right
    }
    public GameObject playButton;
    public MatchFontSizes LSFontMatcher;
    public string[] levelNames;
    private int levelIndex;
    public ReversibleGraphicColorLerp[] bgList;
    private EventSystem eventSystem;
    private SceneController sceneController;
    public GameObject[] scrollers;
    private UIScroller[] scrollList;
    private ReversibleGraphicColorLerp[] colorLerps;
    public returnToPosition[] arrows;
    public float inputBlockingDuration = 0;
    public float axisDeadZone = 0.1f;
    private bool xAxisMaxed = false;
    private bool allowInput = true;  

    void Awake () {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
        levelIndex = 0;
        scrollList = new UIScroller[scrollers.Length];
        colorLerps = new ReversibleGraphicColorLerp[scrollers.Length];
        for(int i = 0; i < scrollers.Length; i++)
        {
            scrollList[i] = scrollers[i].GetComponent<UIScroller>();
            colorLerps[i] = scrollers[i].GetComponent<ReversibleGraphicColorLerp>();
        }
    }

    // Update is called once per frame
    void Update () {
		if(MainMenuController.menuState == MainMenuController.MenuState.LevelSelect)
        {
            updateLevelSelect();
        }
	}
    private void updateLevelSelect()
    {
        if (eventSystem.currentSelectedGameObject == playButton && allowInput)
        {
            if (Input.GetAxis("Horizontal") < -axisDeadZone)
            {
                scrollLevel(Direction.Left);
                for (int i = 0; i < bgList.Length; i++)
                {
                    if (i == levelIndex)
                    {
                        bgList[i].startColorChange(1);
                    }
                    else
                    {
                        bgList[i].startColorChange(-1);
                    }
                }

            }
            else if (Input.GetAxis("Horizontal") > axisDeadZone)
            {
                scrollLevel(Direction.Right);
                for (int i = 0; i < bgList.Length; i++)
                {
                    if (i == levelIndex)
                    {
                        bgList[i].startColorChange(1);
                    }
                    else
                    {
                        bgList[i].startColorChange(-1);
                    }
                }
            }
    
        }
        if(levelIndex <= 0)
        {
            arrows[0].gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            arrows[0].gameObject.GetComponent<Image>().enabled = true;
        }
        if (levelIndex >= scrollers.Length-1)
        {
            arrows[1].gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            arrows[1].gameObject.GetComponent<Image>().enabled = true;
        }
    }
    private void scrollLevel(Direction dir)
    {
        switch (dir)
        {
            case (Direction.Left):
                if (levelIndex >= 1)
                {
                    scrollList[levelIndex].hideUI(-1);
                    colorLerps[levelIndex].startColorChange(-1);
                    scrollList[levelIndex - 1].revealUI(-1);
                    colorLerps[levelIndex-1].startColorChange(1);
                    arrows[0].moveRectTransform(Vector2.left * .3f);
                    levelIndex--;
                    if (allowInput)
                        StartCoroutine(preventInput(inputBlockingDuration));
                }
                break;
            case (Direction.Right):
                if(levelIndex < scrollers.Length - 1)
                {
                    scrollList[levelIndex].hideUI(1);
                    colorLerps[levelIndex].startColorChange(-1);
                    scrollList[levelIndex+1].revealUI(1);
                    colorLerps[levelIndex+1].startColorChange(1);
                    arrows[1].moveRectTransform(Vector2.right * .3f);
                    if (allowInput)
                        StartCoroutine(preventInput(inputBlockingDuration));
                    levelIndex++;
                }
                break;
        }
        
    }
    public void playButtonPress()
    {
        if (GameState.compareState(GameState.State.MainMenu))
        {
            GameState.setState(GameState.State.InGame);
            MainMenuController.menuState = MainMenuController.MenuState.None;
            eventSystem.SetSelectedGameObject(null);
            StartCoroutine(sceneController.transitionThenLoad(true,levelNames[levelIndex]));
        }
    }

    
    public void resetBackgrounds()
    {
        for(int i = 0; i < bgList.Length; i++)
        {
            if(i == 0)
            {
                bgList[i].setColorTimer(1);
             
                bgList[i].endColorChange();
            } else
            {
                bgList[i].setColorTimer(0);
                bgList[i].resetColor();
                bgList[i].endColorChange();
            }
        }
    }
    private bool maxedXAxisThisFrame(Direction dir)
    {
        //returns true when the stick hits the edge, for 1 frame

        switch (dir)
        {
            case (Direction.Right):
                return Input.GetAxis("Horizontal") >= (1 - axisDeadZone) && !xAxisMaxed;
            case (Direction.Left):
                return Input.GetAxis("Horizontal") <= (-1 + axisDeadZone) && !xAxisMaxed;
        }
        print("maxedXAxisThisFrame: Invalid Direction->" + dir);
        return false;
    }
    public void loadLevelSelect()
    {
        eventSystem.SetSelectedGameObject(playButton);
        colorLerps[0].setColorTimer(1);
        colorLerps[0].endColorChange();
        bgList[0].setColorTimer(1);
        LSFontMatcher.resizeText();
        resetBackgrounds();
        for(int i = 0; i < scrollList.Length; i++)
        {
            if(i == 0)
            {
                scrollList[i].revealImmediately();
            } else
            {
                scrollList[i].hideImmediately();
            }
        }
        levelIndex = 0;
    }
    private IEnumerator preventInput(float duration)
    {
        allowInput = false;
        yield return new WaitForSeconds(duration);
        allowInput = true;
    }
}
