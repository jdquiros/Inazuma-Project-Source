using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIndicator : MonoBehaviour {

    // Use this for initialization
    private SpriteRenderer spriteRenderer;
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GameState.controlLayout == 0)
            spriteRenderer.enabled = false;
        else if(!GameState.compareState(GameState.State.MainMenu))
            spriteRenderer.enabled = true;
	}
}
