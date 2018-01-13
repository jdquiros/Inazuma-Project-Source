using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatform : MonoBehaviour {

    // Use this for initialization
    private GameObject player;
    private BoxCollider2D col;
    private ColorLerp colorLerp;
    private SpriteColorOscillation oscillator;
    private SpriteRenderer spriteRenderer;
    public float timeUntilFade;
    public float criticalFlashingDuration;
    public float criticalFlashSpeedMultiplier = 1;
    public float fadeDuration;
    public float respawnDelay;
    public float respawnDuration;
    private bool isActive;
    private bool playerInside;
    private Color colorA,colorB;
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        col = GetComponent<BoxCollider2D>();
        colorLerp = GetComponent<ColorLerp>();
        oscillator = GetComponent<SpriteColorOscillation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorA = colorLerp.startColor;
        colorB = colorLerp.endColor;
	}
	
	// Update is called once per frame
    private IEnumerator runCycle()
    {
        isActive = true;
        oscillator.startColorChange();
        yield return new WaitForSeconds(timeUntilFade-fadeDuration-criticalFlashingDuration);
        oscillator.transitionTime /= criticalFlashSpeedMultiplier;
        yield return new WaitForSeconds(criticalFlashingDuration);
        oscillator.transitionTime *= criticalFlashSpeedMultiplier;
        colorLerp.duration = fadeDuration*1.2f;
        colorLerp.setColors(spriteRenderer.color, new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b,0));
        oscillator.stopColorChange();
        colorLerp.startColorChange();
        yield return new WaitForSeconds(fadeDuration);
        colorLerp.setColors(colorA, colorB);
        col.enabled = false;
        yield return new WaitForSeconds(respawnDelay - respawnDuration);
        colorLerp.duration = respawnDuration;
        colorLerp.setColors(colorB, colorA);
        while (playerInside)
        {
            yield return new WaitForEndOfFrame();
        }
        col.enabled = true;
        colorLerp.startColorChange();
        yield return new WaitForSeconds(respawnDuration);
        colorLerp.setColors(colorA, colorB);

        isActive = false;
    }
    public bool touchable()
    {
        return col.enabled;
    }
    void childTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = true;
            if (!isActive)
                StartCoroutine(runCycle());
        }
    }
    void childTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    void childTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
