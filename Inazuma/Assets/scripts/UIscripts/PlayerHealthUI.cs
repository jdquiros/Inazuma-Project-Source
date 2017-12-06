﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour {

    // Use this for initialization
    private PlayerController player;
    public Image[] heartList;
    public Text healthText;
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < heartList.Length; ++i)
        {
            if(player.health >= (i+1) && !GameState.compareState(GameState.State.MainMenu))
            {
                heartList[i].enabled = true;
            } else
            {
                heartList[i].enabled = false;
            }
        }
        if (!GameState.compareState(GameState.State.MainMenu)){
            healthText.enabled = true;
        } else
        {
            healthText.enabled = false;
        }

    }
}
