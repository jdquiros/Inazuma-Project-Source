using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIScroller : MonoBehaviour
{

    public bool revealed;
    public string revealFrom;
    public string hideTo;
    public float speedFactor;
    private Vector3 hiddenPositionBefore;
    private Vector3 hiddenPositionAfter;
    private Vector3 revealedPosition;
    public bool revealOnLoad;
    public bool hideOnLoad;
    bool reversed;
    bool moving;
    void Start()
    {

        if (GetComponent<Image>() != null && !GetComponent<Image>().enabled)
        {
            GetComponent<Image>().enabled = true;
        }
        if (speedFactor == 0)
        {
            speedFactor = 1;
        }

        speedFactor = speedFactor / 100;
        if (speedFactor > 1)
        {
            speedFactor = 1;
        }
        revealedPosition = transform.localPosition;
        hiddenPositionBefore = getHiddenPositionBefore();
        hiddenPositionAfter = getHiddenPositionAfter();
        if (!revealed)
        {
            if (revealFrom == "Up")
            {
                transform.position += Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;
            }
            else if (revealFrom == "Down")
            {
                transform.position += Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;
            }
            else if (revealFrom == "Left")
            {
                transform.position += Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;
            }
            else if (revealFrom == "Right")
            {
                transform.position += Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;
            }
            else
            {
                print("UNKNOWN WALL ANCHOR");
            }
        }
        reversed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (revealed && moving)
        {
            moveToLocation(revealedPosition);
        }
        else if (!revealed && moving)
        {
            if (!reversed)
            {
                moveToLocation(hiddenPositionAfter);
            }
            else
            {
                moveToLocation(hiddenPositionBefore);
            }
        }

    }
    public void moveToLocation(Vector3 target)
    {
        transform.localPosition += (target - transform.localPosition) * (speedFactor);
        if (Vector3.Distance(transform.localPosition, target) < 1)
        {
            moving = false;
            transform.localPosition = target;
        }
    }
    public void revealUI(int direction)
    {
        //1 is normal, -1 is reversed direction
        revealed = true;
        moving = true;
        if (direction == 1)
        {
            transform.localPosition = hiddenPositionBefore;
            reversed = false;
        }
        else
        {
            transform.localPosition = hiddenPositionAfter;
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
            transform.localPosition = hiddenPositionBefore;
        }
        moving = false;
    }
    public void hideImmediately()
    {
        moving = false;
        transform.localPosition = hiddenPositionBefore;
    }


    public Vector3 getHiddenPositionBefore()
    {
        if (revealFrom == "Up")
        {

            return transform.localPosition + Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;

        }
        else if (revealFrom == "Down")
        {
            return transform.localPosition + Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;
        }
        else if (revealFrom == "Left")
        {
            return transform.localPosition + Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;

        }
        else if (revealFrom == "Right")
        {
            return transform.localPosition + Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;

        }
        else
        {
            print("UNKNOWN WALL ANCHOR");
            return transform.localPosition;
        }
    }
    public Vector3 getHiddenPositionAfter()
    {
        if (hideTo == "Up")
        {

            return transform.localPosition + Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;

        }
        else if (hideTo == "Down")
        {
            return transform.localPosition + Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;
        }
        else if (hideTo == "Left")
        {
            return transform.localPosition + Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;

        }
        else if (hideTo == "Right")
        {
            return transform.localPosition + Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;

        }
        else
        {
            print("UNKNOWN WALL ANCHOR");
            return transform.localPosition;
        }
    }
}