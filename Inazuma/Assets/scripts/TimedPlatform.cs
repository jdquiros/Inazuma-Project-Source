using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPlatform : MonoBehaviour {

    // Use this for initialization
    private GameObject player;
    private BoxCollider2D col;
    private ColorLerp colorLerp;
    public float timeUntilFade;
    public float fadeDuration;
    public float respawnDelay;
    public float respawnDuration;
    private bool isActive;
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        col = GetComponent<BoxCollider2D>();
        colorLerp = GetComponent<ColorLerp>();
	}
	
	// Update is called once per frame
    private IEnumerator runCycle()
    {
        isActive = true;
        yield return new WaitForSeconds(timeUntilFade-fadeDuration);
        colorLerp.duration = fadeDuration;
        colorLerp.startColorChange();
        yield return new WaitForSeconds(fadeDuration);
        col.enabled = false;
        yield return new WaitForSeconds(respawnDelay - respawnDuration);
        colorLerp.duration = respawnDuration;
        colorLerp.setColors(colorLerp.endColor, colorLerp.startColor);
        colorLerp.startColorChange();
        yield return new WaitForSeconds(respawnDuration);
        colorLerp.setColors(colorLerp.endColor, colorLerp.startColor);
        col.enabled = true;
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
        }
    }
}
