using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{

    // Use this for initialization
    
    public bool preserveStateAcrossScenes = true;
    public int levelNumber = 0;
    public int initialState = 0;            //when game is first opened, state should be this
    private int state;                   //this state is read from the LevelDataDictionary and tells you what state to spawn an object in
    void Awake()
    {
        if (preserveStateAcrossScenes)
        {
            LevelData.addItem(levelNumber,  gameObject.name, initialState);
            //loads the dictionary with the inital state if it does not have an intial state yet
            state = LevelData.getState(levelNumber,  gameObject.name);
        } else
        {
            state = initialState;
        }
    }
    public void initialLoad()
    {
        //call initialLoad if you need stateData to be guaranteed to be loaded while inside an Awake()
        if (preserveStateAcrossScenes)
        {
            LevelData.addItem(levelNumber, gameObject.name, initialState);
            state = LevelData.getState(levelNumber, gameObject.name);
        } else
        {
            state = initialState;
        }
        
    }
    public int getState()
    {
        return state;
    }
    public void setState(int val)
    {
        //saves current state to be loaded after scene reload
        if (preserveStateAcrossScenes)
        {
            LevelData.setState(levelNumber, gameObject.name, val);
        }
        state = val;
    }
    public static bool allActive(LevelState[] stateList, int activeStateValue)
    {
        for(int i = 0; i < stateList.Length; i++)
        {
            if(stateList[i].getState() != activeStateValue)
            {
                return false;
            }
        }
        return true;
    }
    public static bool anyActive(LevelState[] stateList, int activeStateValue)
    {
        for (int i = 0; i < stateList.Length; i++)
        {
            if (stateList[i].getState() == activeStateValue)
            {
                return true;
            }
        }
        return false;
    }
}

