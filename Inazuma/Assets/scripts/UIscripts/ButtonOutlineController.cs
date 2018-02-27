using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ButtonOutlineController : MonoBehaviour {

    // Use this for initialization
    private ButtonOutlineDrawer[] outlineList;
    private int screenWidth;
    private int screenHeight;
    private Camera mainCam;
    public float screenSpaceX;
    public float screenSpaceY;
    public float sizeScaleX;
    public float sizeScaleY;
	void Awake () {

        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        outlineList = GetComponentsInChildren<ButtonOutlineDrawer>();
        screenWidth = mainCam.pixelWidth;
        screenHeight = mainCam.pixelHeight;
        Vector3 leftBorder = mainCam.ScreenToWorldPoint(new Vector3(0, screenHeight / 2));
        Vector3 rightBorder = mainCam.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight / 2));
        Vector3 bottomBorder = mainCam.ScreenToWorldPoint(new Vector3(0, 0));
        Vector3 topBorder = mainCam.ScreenToWorldPoint(new Vector3(0, screenHeight));
        float worldScreenWidth = rightBorder.x - leftBorder.x;
        float worldScreenHeight = topBorder.y - bottomBorder.y;
        transform.position = new Vector3(leftBorder.x + Mathf.Lerp(0, worldScreenWidth, screenSpaceX),bottomBorder.y + Mathf.Lerp(0,worldScreenHeight,screenSpaceY));
        transform.localScale = new Vector3(sizeScaleX * worldScreenWidth, sizeScaleY * worldScreenHeight);
        
	}
	
	// Update is called once per frame
	void Update () {

    }
    public void resetOutline()
    {
        if (outlineList == null)
            return;
        for(int i = 0; i < outlineList.Length; i++)
        {
            outlineList[i].resetLine();
        }
    }
    public void drawOutline()
    {
        if (outlineList == null)
            return;
        for (int i= 0; i < outlineList.Length; i++)
        {
            outlineList[i].drawLine();
        }
    }
  
    
}
