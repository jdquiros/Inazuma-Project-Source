using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GraphicColorLerp : MonoBehaviour
{

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
    public float initialDelay = 0;
    private Graphic graphicRenderer;
    private IEnumerator colorCoroutine;
    void Awake()
    {
        graphicRenderer = GetComponent<Graphic>();
        colorCoroutine = colorChange();
       
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

   
    private IEnumerator colorChange()
    {
        graphicRenderer.color = startColor;
        while (initialDelay > 0)
        {
            initialDelay -= Time.deltaTime;
            yield return null;
        }
        float elapsedTime = 0;
        if (duration == 0)
        {
            graphicRenderer.color = endColor;
        }
        while (elapsedTime < duration)
        {
            graphicRenderer.color = Color.Lerp(startColor,
                endColor, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        graphicRenderer.color = endColor;
    }
    public void startColorChange()
    {
        StopCoroutine(colorCoroutine);
        colorCoroutine = colorChange();
        StartCoroutine(colorCoroutine);
    }
    public void setColors(Color newStartColor, Color newEndColor)
    {
        startColor = newStartColor;
        endColor = newEndColor;
    }
}