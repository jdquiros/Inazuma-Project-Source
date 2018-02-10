using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    // Use this for initialization
    public enum Type
    {
        Any, All
    }
    public Type activationType;
    public LevelState[] linkedStates;
    public int ACTIVE = 1;
    public int INACTIVE = 0;
    private BoxCollider2D boxCollider;
    public LightningLine[] lineList;
    private LevelState stateData;
    private int stateLastFrame;
    private int state;
	void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        stateData = GetComponent<LevelState>();
        stateLastFrame = stateData.getState();
        state = stateData.getState();
    }
    private void Update()
    {
        switch (activationType)
        {
            case Type.All:
                if (linkedStates.Length > 0)
                {
                    state = LevelState.allActive(linkedStates, ACTIVE) ? ACTIVE : INACTIVE;
                }
                break;
            case Type.Any:
                if (linkedStates.Length > 0)
                {
                    state = LevelState.anyActive(linkedStates, ACTIVE) ? ACTIVE : INACTIVE;
                }
                break;
        }
        
        if(state == ACTIVE && stateLastFrame == INACTIVE)       //when barrier changes from inactive to active
        {
            turnOnBarrier();    
        }
        else if(state == INACTIVE && stateLastFrame == ACTIVE)  //when barrier changes from active to inactive
        {
            turnOffBarrier();
        }
        stateLastFrame = state;
    }
    public void turnOffBarrier()
    {
        boxCollider.enabled = false;
        for(int i = 0; i < lineList.Length; i++)
        {
            lineList[i].setIsDrawing(false);
        }
        stateData.setState(INACTIVE);
    }
    public void turnOnBarrier()
    {
        boxCollider.enabled = true;
        for (int i = 0; i < lineList.Length; i++)
        {
            lineList[i].setIsDrawing(true);
        }
        stateData.setState(ACTIVE);

    }

}
