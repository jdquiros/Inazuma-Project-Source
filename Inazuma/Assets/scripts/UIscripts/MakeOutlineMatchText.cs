using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MakeOutlineMatchText : MonoBehaviour {

    // Use this for initialization
    private Outline outline;
    private Color initialColor;
    private Text text;
    private Color transparent;
    
	void Start () {
        outline = GetComponent<Outline>();
        initialColor = outline.effectColor;
        text = GetComponent<Text>();
        transparent = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
	}
	
	// Update is called once per frame
	void Update () {
        outline.effectColor = Color.Lerp(transparent, initialColor, text.color.a);
	}
}
