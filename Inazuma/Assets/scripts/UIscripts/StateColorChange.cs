using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelState))]
public class StateColorChange : MonoBehaviour {

    public int ACTIVE_STATE = 1;
    public Color activeColor;
    public Color inactiveColor;
    private SpriteRenderer spriteRenderer;
    private LevelState state;
    void Start()
    {
        state = GetComponent<LevelState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = (state.getState() == ACTIVE_STATE) ? activeColor : inactiveColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (state.getState() == ACTIVE_STATE)
        {
            spriteRenderer.color = activeColor;
        }
        else
        {
            spriteRenderer.color = inactiveColor;
        }
    }
}
