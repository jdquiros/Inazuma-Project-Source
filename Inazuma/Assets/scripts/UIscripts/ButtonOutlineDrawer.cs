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
    private TrailRenderer trailRenderer;
    private bool isMoving;
    private Vector3 originalPosition;
    public bool playOnAwake = false;
	void Awake () {
        trailRenderer = GetComponent<TrailRenderer>();
        xSign = (xDir == Direction.Positive) ? 1 : -1;
        ySign = (yDir == Direction.Positive) ? 1 : -1;
        xStop = transform.localPosition.x + xDistance * xSign;
        yStop = transform.localPosition.y + yDistance * ySign;
        originalPosition = transform.localPosition;
        if (playOnAwake)
            drawLine();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isMoving)
        {
            if (Mathf.Abs(transform.localPosition.y - yStop) < speed * Time.deltaTime)
            {
                if (atHeight && Mathf.Abs(transform.localPosition.x - xStop) < speed * Time.deltaTime)
                {
                    transform.localPosition = new Vector3(xStop, transform.localPosition.y, 0);
                }
                else
                {
                    transform.localPosition += Vector3.right * Time.deltaTime * speed * xSign;
                }
                transform.localPosition = new Vector3(transform.localPosition.x, yStop, 0);
                atHeight = true;
            }
            else
            {
                transform.localPosition += Vector3.up * Time.deltaTime * speed * ySign;
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
        trailRenderer.Clear();
    }
}
