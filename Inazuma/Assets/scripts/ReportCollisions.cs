using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportCollisions : MonoBehaviour {

	// Use this for initialization
    private GameObject parent;
    public bool onEnter;
    public bool onStay;
    public bool onExit;
    private BoxCollider2D col;
	void Awake () {
        parent = transform.parent.gameObject;
        col = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame

    private void OnTriggerEnter2D(Collider2D other)
    {
        //runs function in target gameObject and all of its components
        if(onEnter)
            parent.SendMessage("childTriggerEnter2D", other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (onStay)
            parent.SendMessage("childTriggerStay2D", other);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (onExit)
            parent.SendMessage("childTriggerExit2D", other);
    }
}
