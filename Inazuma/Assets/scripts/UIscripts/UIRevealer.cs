using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIRevealer : MonoBehaviour
{

    // Use this for initialization
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public bool revealed;
    public Direction anchorDirection;
    public float speedFactor;
    private Vector3 hiddenPosition;
    private Vector3 revealedPosition;
    private bool moving;
    public bool revealOnLoad;
    public bool hideOnLoad;

    private bool debug = true;
    void Start()
    {


        /* use speedfactor of 100 for instant movement
         * This doesn't correlate to pixels or time or anything because I'm
         * just done with this script. 
         * 
         * Major resizing of the game window WILL break this 
         * */

        if (GetComponent<Image>() != null && !GetComponent<Image>().enabled)
        {
            //You can disable the Image component of any UI element with a UIRevealer
            //for easier viewing of UI underneath and it will re-enable on scene start
            GetComponent<Image>().enabled = true;
        }
        revealedPosition = transform.localPosition;  //saves current location for when the UI is revealed later
        if (!revealed)
        {
            //If default state is "not revealed" move to hidden position
            switch (anchorDirection)
            {
                case (Direction.Up):
                    transform.position += Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;
                    break;

                case (Direction.Down):
                    transform.position += Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;
                    break;

                case (Direction.Left):
                    transform.position += Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;
                    break;
                case (Direction.Right):
                    transform.position += Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;
                    break;
            }

        }

        hiddenPosition = getHiddenPosition();
        moving = false;
        if (revealOnLoad)  //UI element spawns offscreen and begins revealing instantly
        {
            revealUI();
        }
        else if (hideOnLoad)  //UI element spawns onscreen and begins hiding instantly
        {
            hideUI();
        }
    }

    void Update()
    {


        if (revealed && moving)
        {
            moveToLocation(revealedPosition);
        }
        else if (!revealed && moving)
        {
            moveToLocation(hiddenPosition);
        }
        if (Input.GetKeyDown("space") && debug) //For testing purposes only.  Manual activation of all uirevealers
        {
            if (revealed)
            {
                hideUI();
            }
            else
            {
                revealUI();
            }

        }
    }
    public void moveToLocation(Vector3 target)
    {
        //moves towards location, snaps to location once close enough
        transform.localPosition += (target - transform.localPosition) * (speedFactor / 100f) * (1 - Time.deltaTime);
        if (Vector3.Distance(transform.localPosition, target) < 1)
        {
            moving = false;
            transform.localPosition = target;
        }
        if (speedFactor == 100f)
        {
            moving = false;
            transform.localPosition = target;
        }
    }
    public void revealUI()
    {

        revealed = true;
        moving = true;

    }
    public void hideUI()
    {

        revealed = false;
        moving = true;

    }
    public void hideImmediately()
    {
        transform.localPosition = hiddenPosition;
        revealed = false;
        moving = false;
    }
    private Vector3 getHiddenPosition()
    {
        //returns the point that is offscreen that the UI element will move to when set to "not revealed"
        if (revealed)
        {
            switch (anchorDirection)
            {
                case (Direction.Up):
                    return transform.localPosition + Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;

                case (Direction.Down):
                    return transform.localPosition + Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;

                case (Direction.Left):
                    return transform.localPosition + Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;
                case (Direction.Right):
                    return transform.localPosition + Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;
            }
            return transform.localPosition;
        }
        else
        {
            return transform.localPosition;
        }

    }
    private Vector3 getRevealedPosition()
    {
        //returns the point that is onscreen that the UI element will move to when set to "revealed"
        if (!revealed)
        {
            switch (anchorDirection)
            {
                case (Direction.Up):
                    return transform.localPosition - Vector3.up * (1 - gameObject.GetComponent<RectTransform>().anchorMin.y) * Screen.height;
                case (Direction.Down):
                    return transform.localPosition - Vector3.down * gameObject.GetComponent<RectTransform>().anchorMax.y * Screen.height;
                case (Direction.Left):
                    return transform.localPosition - Vector3.left * gameObject.GetComponent<RectTransform>().anchorMax.x * Screen.width;
                case (Direction.Right):
                    return transform.localPosition - Vector3.right * (1 - gameObject.GetComponent<RectTransform>().anchorMin.x) * Screen.width;


            }
            return transform.localPosition;
        }
        else
        {
            return transform.localPosition;
        }
    }




}