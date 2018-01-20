using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectController : MonoBehaviour {

    // Use this for initialization
    public GameObject[] levelObjList;
    public string[] levelNames;
    private int levelIndex;
    public ReversibleGraphicColorLerp[] bgList;
    private EventSystem eventSystem;
    private SceneController sceneController;

	void Start () {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();

    }

    // Update is called once per frame
    void Update () {
		
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
    public void onLevelButtonSelect()
    {
        if(Input.GetAxisRaw("Vertical") < -0.8f)
        {
            levelIndex++;
            if (levelIndex >= levelNames.Length)
                levelIndex = levelNames.Length;
        } else if(Input.GetAxisRaw("Vertical") > 0.8f)
        {
            levelIndex--;
            if (levelIndex < 0)
                levelIndex = 0;
        }
        for(int i = 0; i < bgList.Length; i++)
        {
            if(i == levelIndex)
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
