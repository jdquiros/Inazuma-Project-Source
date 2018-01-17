using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOutlineDrawer : MonoBehaviour {

	// Use this for initialization
    public enum Direction
    {
        Positive,
        Negative
    }
    public Direction xDir;
    public Direction yDir;
    public float xDistance;
    public float yDistance;
    public float speed;

    private int xSign;
    private int ySign;
    private float xStop;
    private float yStop;

    private bool atHeight;
    private LineRenderer lineRenderer;
    int pointIndex = 0;
    private bool isMoving;
    private Vector3 originalPosition;
    private Vector3 pointB, pointA;
    public bool playOnAwake = false;
    private bool cornerReached = false;
	void Awake () {
        lineRenderer = GetComponent<LineRenderer>();
        xSign = (xDir == Direction.Positive) ? 1 : -1;
        ySign = (yDir == Direction.Positive) ? 1 : -1;
        xStop = xDistance * xSign;
        yStop = yDistance * ySign;
        originalPosition = new Vector3(-xSign,0);
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(0, originalPosition);
        lineRenderer.SetPosition(1, originalPosition);
        lineRenderer.SetPosition(2, originalPosition);
        if (playOnAwake)
            drawLine();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (isMoving)
        {
            if (Mathf.Abs(pointA.y - yStop) < speed * Time.deltaTime)
            {
                if (!atHeight)
                {
                    atHeight = true;
                    pointA = new Vector3(pointA.x, yStop, 0);
                    lineRenderer.SetPosition(1, pointA);
                    pointB = pointA;

                }
                else
                {
                    if (atHeight && Mathf.Abs(pointB.x - xStop) < speed * Time.deltaTime)
                    {
                        pointB = new Vector3(xStop, transform.localPosition.y, 0);
                    }
                    else
                    {
                        pointB += Vector3.right * Time.deltaTime * speed * xSign;
                    }
                }
                lineRenderer.SetPosition(2,new Vector3(pointB.x, yStop, 0));
            }
            else
            {
                
                pointA += Vector3.up * Time.deltaTime * speed * ySign;
                lineRenderer.SetPosition(1, pointA);
            }
        }
    }
    public void drawLine()
    {
        isMoving = true;
    }
    public void resetLine()
    {
        isMoving = false;
        atHeight = false;
        transform.localPosition = originalPosition;
        cornerReached = false;
        pointA = originalPosition;
        pointB = originalPosition;
        lineRenderer.SetPosition(1, pointA);
        lineRenderer.SetPosition(2, pointB);
    }
}
