using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchFontSizes : MonoBehaviour {

    // Use this for initialization
    public Text[] textList;

	void Start () {
        resizeText();
	}
	private IEnumerator waitForFrame()
    {
        yield return null;
        applySizes();

    }
    private void applySizes()
    {
        int minSize = 1000;
        foreach(var item in textList)
        {
            minSize = Mathf.Min(minSize, item.cachedTextGenerator.fontSizeUsedForBestFit);
        }
        foreach(var item in textList)
        {
            item.resizeTextMaxSize = minSize;
        }
    }
    public void resizeText()
    {
        foreach (var item in textList)
        {
            item.resizeTextMaxSize = 1000;
        }
        StartCoroutine(waitForFrame());
    }
	// Update is called once per frame
	void Update () {
		
	}
}
