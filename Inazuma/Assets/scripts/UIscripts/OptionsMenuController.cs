using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionsMenuController : MonoBehaviour {

    // Use this for initialization
    public GameObject layoutSelectButton;
    private Text layoutSelectText;
    public string[] layoutText;
	void Start () {
        layoutSelectText = layoutSelectButton.GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void layoutSelectPress()
    {
        GameState.controlLayout = (GameState.controlLayout + 1) % layoutText.Length;
        layoutSelectText.text = layoutText[GameState.controlLayout];
    }
    
}
