using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour {

    // Use this for initialization
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private SpriteRenderer spriteRenderer;
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        spriteRenderer.sprite = inactiveSprite;
	}
    private void HitByRaycast()
    {
        spriteRenderer.sprite = activeSprite;
    }
}
