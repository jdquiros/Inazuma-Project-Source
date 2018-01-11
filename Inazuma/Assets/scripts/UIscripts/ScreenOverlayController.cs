using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOverlayController : MonoBehaviour {

    // Use this for initialization

    private GraphicColorLerp colorLerp;
    public bool playOnAwake = false;
    private Color startColor;
    private Color endColor;
    private void Awake()
    {
        colorLerp = GetComponent<GraphicColorLerp>();
        if (GameState.playTransition)       //This is the main reason why this is a script.  It will look to the GameState to tell if it should play on awake
            playOnAwake = true;
    }
    void Start () {
        GameState.playTransition = false;
        if (playOnAwake)
        {
            colorLerp.startColorChange();
        }
        startColor = colorLerp.startColor;
        endColor = colorLerp.endColor;
	}
	
	// Update is called once per frame
    public void screenAppear()
    {
        colorLerp.setColors(endColor, startColor);
        colorLerp.startColorChange();
    }
    public void screenDisappear()
    {
        colorLerp.setColors(startColor, endColor);
        colorLerp.startColorChange();
    }
}
