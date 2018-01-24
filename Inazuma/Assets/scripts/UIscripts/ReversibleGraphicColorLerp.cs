using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReversibleGraphicColorLerp : MonoBehaviour {

    // Use this for initialization
    public enum PlayTrigger
    {
        Start,
        Enable,
        None
    }
    public PlayTrigger playOn = PlayTrigger.Start;
    public Color startColor = Color.white;
    public Color endColor = Color.white;
    public float duration = 0;
    private Graphic graphicRenderer;
    [Range(0.0f, 1.0f)]
    public float initialTimerState = 0;
    private float colorTimer = 0;
    private int direction;
    private bool activated = false;

    void Awake()
    {
        graphicRenderer = GetComponent<Graphic>();
        colorTimer = duration * initialTimerState;
    }
    void OnEnable()
    {
        if (playOn == PlayTrigger.Enable)
            startColorChange(1);
    }
    void Start()
    {
        if (playOn == PlayTrigger.Start)
            startColorChange(1);
    }
    void Update()
    {
        if (activated)
        {
            colorTimer += Time.deltaTime * direction;

            graphicRenderer.color = Color.Lerp(startColor, endColor, colorTimer/duration);

            if (colorTimer > duration)
                colorTimer = duration;
            else if (colorTimer < 0)
                colorTimer = 0;
        }
    }

    
    public void startColorChange(int dir)
    {
        graphicRenderer.enabled = true;
        direction = dir;
        activated = true;
    }
    public void endColorChange()
    {
        activated = false;
    }
    public void resetColor()
    {
        graphicRenderer.color = startColor;
    }
    public void setColors(Color newStartColor, Color newEndColor)
    {
        startColor = newStartColor;
        endColor = newEndColor;
    }
    public void setColorTimer(float perc)
    {
        //Between 0% and 100%
        colorTimer = duration*perc;
        graphicRenderer.color = Color.Lerp(startColor, endColor, colorTimer / duration);

    }
}
