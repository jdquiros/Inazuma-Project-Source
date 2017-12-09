using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnToPosition : MonoBehaviour {

    // Use this for initialization
    private float[] originalPosition;
    private float[] currentPosition;
    public float speed;
    private RectTransform myTransform;
    private bool moving;
	void Start () {
        myTransform = GetComponent<RectTransform>();
        originalPosition = new float[4];
        currentPosition = new float[4];
        updateOriginalPosition();
        moving = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (moving)
            moveToLocation(originalPosition);
	}
    public void moveToLocation(float[] targetAnchors)
    {
        //moves towards location, snaps to location once close enough
        float[] newAnchors = new float[4];
        newAnchors[0] = myTransform.anchorMin.x;
        newAnchors[1] = myTransform.anchorMin.y;
        newAnchors[2] = myTransform.anchorMax.x;
        newAnchors[3] = myTransform.anchorMax.y;

        Vector2 moveVector = new Vector2(originalPosition[0] - currentPosition[0], originalPosition[1] - currentPosition[1]).normalized;
        newAnchors[0] += moveVector.x * speed * Time.deltaTime;
        newAnchors[1] += moveVector.y * speed * Time.deltaTime;
        newAnchors[2] += moveVector.x * speed * Time.deltaTime;
        newAnchors[3] += moveVector.y * speed * Time.deltaTime;
        if (Vector2.Distance(new Vector2(newAnchors[0], newAnchors[1]), new Vector2(targetAnchors[0], targetAnchors[1])) < speed * Time.deltaTime)
        {
            setRectTransform(targetAnchors);
            moving = false;
        }
        else
        {
            setRectTransform(newAnchors);
        }
            
    }
    public void moveRectTransform(Vector2 vec)
    {
        moving = true;
        currentPosition[0] = originalPosition[0] + vec.x;
        currentPosition[1] = originalPosition[1] + vec.y;
        currentPosition[2] = originalPosition[2] + vec.x;
        currentPosition[3] = originalPosition[3] + vec.y;
        setRectTransform(currentPosition);
    }
    private void setRectTransform(float[] anchors)
    {
        myTransform.anchorMin = new Vector2(anchors[0], anchors[1]);
        myTransform.anchorMax = new Vector2(anchors[2], anchors[3]);
    }
    public void updateOriginalPosition()
    {
        originalPosition[0] = myTransform.anchorMin.x;  //saves current location for when the UI is revealed later
        originalPosition[1] = myTransform.anchorMin.y;
        originalPosition[2] = myTransform.anchorMax.x;
        originalPosition[3] = myTransform.anchorMax.y;
        currentPosition[0] = myTransform.anchorMin.x;  //saves current location for when the UI is revealed later
        currentPosition[1] = myTransform.anchorMin.y;
        currentPosition[2] = myTransform.anchorMax.x;
        currentPosition[3] = myTransform.anchorMax.y;
    }
}
