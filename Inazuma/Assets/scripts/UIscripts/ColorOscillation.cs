using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorOscillation : MonoBehaviour
{

    // Use this for initialization
    public enum PlayTrigger
    {
        Awake,
        Enable,
        Start,
        None
    }
    public PlayTrigger playOn = PlayTrigger.Awake;
    public Graphic graphicRenderer;
    public Color startColor = Color.white;
    public Color endColor = Color.white;
    private const int POSITIVE = 1;
    private const int NEGATIVE = -1;
    private int direction;
    public float transitionTime;
    public float initialDelay = 0;
    public float timer;
    public bool stopped;

    void Awake()
    {
        graphicRenderer = GetComponent<Graphic>();
        stopped = true;
        direction = POSITIVE;
        if (playOn == PlayTrigger.Awake)
        {
            startColorChange();
        }
    }
    void OnEnable()
    {
        if (playOn == PlayTrigger.Enable)
            startColorChange();
    }
    void Start()
    {
        if (playOn == PlayTrigger.Start)
            startColorChange();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {
            if (initialDelay > 0)
            {
                initialDelay -= Time.deltaTime;
            }
            else
            {
                if (timer < 0)
                    direction = POSITIVE;
                if (timer > transitionTime)
                    direction = NEGATIVE;
                timer += Time.deltaTime * direction;

                graphicRenderer.color = Color.Lerp(startColor, endColor, timer / transitionTime);
            }
        }
    }
    public void startColorChange()
    {
        stopped = false;
    }
    public void stopColorChange()
    {
        stopped = true;
        timer = 0;
        graphicRenderer.color = startColor;
    }
}