using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ButtonOutlineController : MonoBehaviour {

    // Use this for initialization
    private ButtonOutlineDrawer[] outlineList;
    private int screenWidth;
    private Camera mainCam;
    private float xOffset;
    public float offSetScale;
    public float sizeScale;
	void Awake () {

        outlineList = GetComponentsInChildren<ButtonOutlineDrawer>();
        string[] res = UnityEditor.UnityStats.screenRes.Split('x');
        screenWidth = int.Parse(res[0]);
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        xOffset = transform.position.x-mainCam.transform.position.x;
        transform.position = new Vector3(mainCam.transform.position.x + xOffset * offSetScale * screenWidth, transform.position.y, 0);
        transform.localScale = new Vector3(screenWidth * sizeScale, transform.localScale.y, transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void resetOutline()
    {
        for(int i = 0; i < outlineList.Length; i++)
        {
            outlineList[i].resetLine();
        }
    }
    public void drawOutline()
    {
        for(int i= 0; i < outlineList.Length; i++)
        {
            outlineList[i].drawLine();
        }
    }
}
