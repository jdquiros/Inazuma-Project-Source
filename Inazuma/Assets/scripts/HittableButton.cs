using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableButton : MonoBehaviour {

    // Use this for initialization
    public enum Type
    {
        OnlyOnce, FlipFlop, WhileHit
    }
    public int ACTIVE_STATE = 1;
    public int INACTIVE_STATE = 0;
    public Type buttonType;
    private LevelState state;
    public LayerMask hitLayer;
    public string[] hitTags;
    private List<GameObject> objectsOnButton;
	void Start () {
        state = GetComponent<LevelState>();
        objectsOnButton = new List<GameObject>();
	}

    // Update is called once per frame
    void Update()
    {
        if (buttonType == Type.WhileHit)
        {
            if (objectsOnButton.Count <= 0)
            {
                state.setState(INACTIVE_STATE);
            }
            else
            {
                state.setState(ACTIVE_STATE);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (validHit(collision.gameObject))
        {
            switch (buttonType)
            {
                case Type.OnlyOnce:
                    //set active
                    state.setState(ACTIVE_STATE);
                    break;
                case Type.FlipFlop:
                    //switch state
                    state.setState((state.getState() == ACTIVE_STATE) ? INACTIVE_STATE : ACTIVE_STATE);
                    break;
                case Type.WhileHit:
                    if (!objectsOnButton.Contains(collision.gameObject))
                    {
                        objectsOnButton.Add(collision.gameObject);
                    }
                    break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (validHit(collision.gameObject))
        {
            switch (buttonType)
            {
                case Type.OnlyOnce:
                    //pass
                    break;
                case Type.FlipFlop:
                    //pass
                    break;
                case Type.WhileHit:
                    objectsOnButton.Remove(collision.gameObject);
                    break;
            }
        }
    }
    private bool validHit(GameObject g)
    {
        //returns true if the object is on the right layer or has a valid tag
        if (hitLayer == (hitLayer | (1 << g.layer)))
            return true;
        for(int i = 0; i < hitTags.Length; i++)
        {
            if (g.CompareTag(hitTags[i]))
                return true;
        }
        return false;
    }
   

}
