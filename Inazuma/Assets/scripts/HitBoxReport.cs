using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxReport : MonoBehaviour {

    // Use this for initialization
    public GameObject reportTarget;     //default is parent gameobject
    private PolygonCollider2D col;
    private float enableTimer = 0f;

    private SpriteRenderer spriteRenderer; //for debug;
    private void Awake()
    {
        if(reportTarget == null)
        {
            if (transform.parent == null)
                print("No parent for default HitBoxReport reportTarget");
            reportTarget = transform.parent.gameObject;     
        }
        col = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start () {
        col.enabled = false;               //leave disabled until told to be enabled
	}
	
	// Update is called once per frame
	void Update () {
        if (col.enabled)
        {
            if(enableTimer > 0)
            {
                enableTimer -= Time.deltaTime;
                spriteRenderer.color = new Color(1, 0, 0, 0.25f);
            } else
            {
                enableTimer = 0;
                col.enabled = false;
                spriteRenderer.color = Color.clear;
                moveHitBox(new Vector3(-500, -500), 0);
            }
        }
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        //runs function in target gameObject and all of its components
        reportTarget.SendMessage("onHitBoxCollision", other);
    }
    public void enableHitBox(float seconds)
    {
        //enable collider for x seconds
        col.enabled = true;
        enableTimer = seconds;
    }
    public void moveHitBox(Vector3 position, float angle)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
    }
}
