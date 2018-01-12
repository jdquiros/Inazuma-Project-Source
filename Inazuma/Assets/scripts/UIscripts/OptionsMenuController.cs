using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionsMenuController : MonoBehaviour {

    // Use this for initialization
    public GameObject ctrlLayoutSelectButton;
    public GameObject keyLayoutSelectButton;
    private Text ctrlLayoutSelectText;
    private Text keyLayoutSelectText;
    public string[] ctrlLayoutText;
    public string[] keyLayoutText;
    void Start() {
        ctrlLayoutSelectText = ctrlLayoutSelectButton.GetComponentInChildren<Text>();
        keyLayoutSelectText = keyLayoutSelectButton.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update() {

    }
    public void ctrlLayoutSelectPress()
    {
        GameState.controlLayout = (GameState.controlLayout + 1) % ctrlLayoutText.Length;
        ctrlLayoutSelectText.text = ctrlLayoutText[GameState.controlLayout];
    }
    public void keyLayoutSelectPress()
    {
        GameState.keyboardLayout = (GameState.keyboardLayout + 1) % keyLayoutText.Length;
        keyLayoutSelectText.text = keyLayoutText[GameState.keyboardLayout];
    }
    
}
