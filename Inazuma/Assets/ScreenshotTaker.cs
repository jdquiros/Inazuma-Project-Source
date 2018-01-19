using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour {

    // Use this for initialization
    private int screenshotCount = 0;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ScreenCapture.CaptureScreenshot("Screenshot"+screenshotCount,4);
            screenshotCount++;
        }
	}
}
