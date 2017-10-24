using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorLerp : MonoBehaviour
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
    private SpriteRenderer spriteRenderer;
    private IEnumerator colorCoroutine;
    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
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
        spriteRenderer.color = startColor;
        while (initialDelay > 0)
        {
            initialDelay -= Time.deltaTime;
            yield return null;
        }
        float elapsedTime = 0;
        if (duration == 0)
        {
            spriteRenderer.color = endColor;
        }
        while (elapsedTime < duration)
        {
            spriteRenderer.color = Color.Lerp(startColor,
                endColor, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endColor;
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


