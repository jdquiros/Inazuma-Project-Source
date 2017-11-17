using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIScroller : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public Direction anchorDirection;
    public enum SpeedType
    {
        linear,
        multiplicative
    }
    public SpeedType speedType;
    public bool revealed;
    public float speed;
    public bool revealOnLoad;
    public bool hideOnLoad;
    bool reversed;
    bool moving;
    private Vector2 moveVector;
    private RectTransform myTransform;
    private float[] hiddenAnchorsBefore;
    private float[] hiddenAnchorsAfter;
    private float[] revealedAnchors;
    public float snapTolerance = 0.05f;


    /*
     *   --------[2,3]
     *   |         |
     *   |         |
     * [0,1]--------
     */


    void Start()
    {

        if (GetComponent<Image>() != null && !GetComponent<Image>().enabled)
        {
            GetComponent<Image>().enabled = true;
        }
        myTransform = GetComponent<RectTransform>();
        hiddenAnchorsBefore = new float[4];
        hiddenAnchorsAfter = new float[4];
        revealedAnchors = new float[4];
        
        revealedAnchors[0] = myTransform.anchorMin.x;  //saves current location for when the UI is revealed later
        revealedAnchors[1] = myTransform.anchorMin.y;
        revealedAnchors[2] = myTransform.anchorMax.x;
        revealedAnchors[3] = myTransform.anchorMax.y;

        switch (anchorDirection)
        {
            case (Direction.Up):
                hiddenAnchorsBefore[0] = revealedAnchors[0];
                hiddenAnchorsBefore[1] = revealedAnchors[1] + (1 - revealedAnchors[1]);
                hiddenAnchorsBefore[2] = revealedAnchors[2];
                hiddenAnchorsBefore[3] = revealedAnchors[3] + (1 - revealedAnchors[1]);
                hiddenAnchorsAfter[0] = revealedAnchors[0];
                hiddenAnchorsAfter[1] = revealedAnchors[1] - (revealedAnchors[3]);
                hiddenAnchorsAfter[2] = revealedAnchors[2];
                hiddenAnchorsAfter[3] = revealedAnchors[3] - (revealedAnchors[3]);
                moveVector = new Vector2(0, 1);
                break;
            case (Direction.Down):
                hiddenAnchorsBefore[0] = revealedAnchors[0];
                hiddenAnchorsBefore[1] = revealedAnchors[1] - (revealedAnchors[3]);
                hiddenAnchorsBefore[2] = revealedAnchors[2];
                hiddenAnchorsBefore[3] = revealedAnchors[3] - (revealedAnchors[3]);
                hiddenAnchorsAfter[0] = revealedAnchors[0];
                hiddenAnchorsAfter[1] = revealedAnchors[1] + (1 - revealedAnchors[1]);
                hiddenAnchorsAfter[2] = revealedAnchors[2];
                hiddenAnchorsAfter[3] = revealedAnchors[3] + (1 - revealedAnchors[1]);
                moveVector = new Vector2(0, -1);
                break;
            case (Direction.Right):
                hiddenAnchorsBefore[0] = revealedAnchors[0] + (1 - revealedAnchors[0]);
                hiddenAnchorsBefore[1] = revealedAnchors[1];
                hiddenAnchorsBefore[2] = revealedAnchors[2] + (1 - revealedAnchors[0]);
                hiddenAnchorsBefore[3] = revealedAnchors[3];
                hiddenAnchorsAfter[0] = revealedAnchors[0] - (revealedAnchors[2]);
                hiddenAnchorsAfter[1] = revealedAnchors[1];
                hiddenAnchorsAfter[2] = revealedAnchors[2] - (revealedAnchors[2]);
                hiddenAnchorsAfter[3] = revealedAnchors[3];
                moveVector = new Vector2(1, 0);
                break;
            case (Direction.Left):
                hiddenAnchorsBefore[0] = revealedAnchors[0] - (revealedAnchors[2]);
                hiddenAnchorsBefore[1] = revealedAnchors[1];
                hiddenAnchorsBefore[2] = revealedAnchors[2] - (revealedAnchors[2]);
                hiddenAnchorsBefore[3] = revealedAnchors[3];
                hiddenAnchorsAfter[0] = revealedAnchors[0] + (1 - revealedAnchors[0]);
                hiddenAnchorsAfter[1] = revealedAnchors[1];
                hiddenAnchorsAfter[2] = revealedAnchors[2] + (1 - revealedAnchors[0]);
                hiddenAnchorsAfter[3] = revealedAnchors[3];
                moveVector = new Vector2(-1, 0);
                break;
        }
        if (!revealed)
        {
            setRectTransform(hiddenAnchorsBefore);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (revealed && moving)
        {
            moveToLocation(revealedAnchors,moveVector);
        }
        else if (!revealed && moving)
        {
            if (!reversed)
            {
                moveToLocation(hiddenAnchorsAfter,moveVector);
            }
            else
            {
                moveToLocation(hiddenAnchorsBefore,moveVector);
            }
        }

    }
    public void moveToLocation(float[] targetAnchors, Vector3 move)
    {
        //moves towards location, snaps to location once close enough
        float[] newAnchors = new float[4];
        newAnchors[0] = myTransform.anchorMin.x;
        newAnchors[1] = myTransform.anchorMin.y;
        newAnchors[2] = myTransform.anchorMax.x;
        newAnchors[3] = myTransform.anchorMax.y;
        if (!reversed)
            move *= -1;
        switch (speedType)
        {
            case (SpeedType.linear):
                newAnchors[0] += move.x * speed * Time.deltaTime;
                newAnchors[1] += move.y * speed * Time.deltaTime;
                newAnchors[2] += move.x * speed * Time.deltaTime;
                newAnchors[3] += move.y * speed * Time.deltaTime;
                if (Vector2.Distance(new Vector2(newAnchors[0], newAnchors[1]), new Vector2(targetAnchors[0], targetAnchors[1])) < speed * Time.deltaTime)
                {
                    setRectTransform(targetAnchors);
                    moving = false;
                }
                else
                {
                    setRectTransform(newAnchors);
                }
                break;
            case (SpeedType.multiplicative):
                newAnchors[0] += Mathf.Abs(moveVector.x) * (targetAnchors[0] - newAnchors[0]) * speed * Time.deltaTime;
                newAnchors[1] += Mathf.Abs(moveVector.y) * (targetAnchors[1] - newAnchors[1]) * speed * Time.deltaTime;
                newAnchors[2] += Mathf.Abs(moveVector.x) * (targetAnchors[2] - newAnchors[2]) * speed * Time.deltaTime;
                newAnchors[3] += Mathf.Abs(moveVector.y) * (targetAnchors[3] - newAnchors[3]) * speed * Time.deltaTime;
                if (Vector2.Distance(new Vector2(newAnchors[0], newAnchors[1]), new Vector2(targetAnchors[0], targetAnchors[1])) < snapTolerance)
                {
                    setRectTransform(targetAnchors);
                    moving = false;
                }
                else
                {
                    setRectTransform(newAnchors);
                }
                break;
        }
    }
    public void revealUI(int direction)
    {
        //1 is normal, -1 is reversed direction
        revealed = true;
        moving = true;
        if (direction == 1)
        {
            setRectTransform(hiddenAnchorsBefore);
            reversed = false;
        }
        else
        {
            setRectTransform(hiddenAnchorsAfter);
            reversed = true;
        }
    }
    public void hideUI(int direction)
    {

        revealed = false;
        moving = true;
        if (direction == 1)
        {
            reversed = false;
        }
        else if (direction == -1)
        {
            reversed = true;
        }
    }
    public bool isRevealed()
    {
        return revealed;
    }
    public bool isMoving()
    {
        return moving;
    }
    public void setMoving(bool x)
    {
        moving = x;
    }
    public void resetToOriginalPosition()
    {
        if (!revealed)
        {
            setRectTransform(hiddenAnchorsBefore);
        }
        moving = false;
    }
    public void hideImmediately()
    {
        moving = false;
        setRectTransform(hiddenAnchorsBefore);
    }
    private void setRectTransform(float[] anchors)
    {
        myTransform.anchorMin = new Vector2(anchors[0], anchors[1]);
        myTransform.anchorMax = new Vector2(anchors[2], anchors[3]);
    }

    
}