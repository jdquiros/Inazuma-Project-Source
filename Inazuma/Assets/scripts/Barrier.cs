using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    // Use this for initialization
    private const int ACTIVE = 1;
    private const int INACTIVE = 0;
    private BoxCollider2D boxCollider;
    public LightningLine[] lineList;
    private LevelState stateData;
    private bool active;
	void Awake () {
        boxCollider = GetComponent<BoxCollider2D>();
        stateData = GetComponent<LevelState>();
        stateData.initialLoad();        //forces stateData to load its data, regardless of if its Awake() will be run in the future
        active = (stateData.getState() == 1);
        if (active)
        {
            turnOnBarrier();
        } else
        {
            turnOffBarrier();
        }
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
