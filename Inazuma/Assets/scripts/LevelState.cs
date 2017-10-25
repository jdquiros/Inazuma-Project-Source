using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour {

    // Use this for initialization
    public int levelNumber = 0;
    public string category;           
    public string objName;               //MUST BE UNIQUE
    public int initialState = 0;            //when game is first opened, state should be this
    private int loadState;                   //this state is read from the LevelDataDictionary and tells you what state to spawn an object in
	void Start () {
        LevelData.addItem(levelNumber, category, objName, initialState);
        //loads the dictionary with the inital state if it does not have an intial state yet
        loadState = LevelData.getState(levelNumber, category, objName);
	}
	public int getLoadState()
    {
        return loadState;
    }
    public void saveLoadState(int val)
    {
        //saves current state to be loaded after scene reload
        LevelData.setState(levelNumber, category, objName, val);
    }
	
}
