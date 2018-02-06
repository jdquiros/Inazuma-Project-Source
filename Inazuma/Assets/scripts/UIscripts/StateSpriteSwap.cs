using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelState))]
public class StateSpriteSwap : MonoBehaviour {

    // Use this for initialization
    public int ACTIVE_STATE = 1;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private SpriteRenderer spriteRenderer;
    private LevelState state;
	void Start () {
        state = GetComponent<LevelState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = (state.getState() == ACTIVE_STATE) ? activeSprite : inactiveSprite;

    }

    // Update is called once per frame
    void Update () {
		if(state.getState() == ACTIVE_STATE)
        {
            spriteRenderer.sprite = activeSprite;
        }
        else
        {
            spriteRenderer.sprite = inactiveSprite;
        }
	}
}
