using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopBackground : MonoBehaviour {

    // Use this for initialization
    public float xParallaxFactor;
    public float yParallaxFactor;
    private float spriteWidth;
    public float Z;
    private CameraSystem parentCamera;
	void Start () {
        spriteWidth = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.x * transform.lossyScale.x;
        transform.localPosition = new Vector3(0,0,Z);
        parentCamera = GetComponentInParent<CameraSystem>();
    }
	
	// Update is called once per frame
	void Update () {
		if(transform.localPosition.x < -spriteWidth)
        {
            transform.localPosition += Vector3.right * spriteWidth;
        } else if(transform.localPosition.x > spriteWidth)
        {
            transform.localPosition += Vector3.left * spriteWidth;
        }
        transform.localPosition += new Vector3(-parentCamera.getDistanceMoved().x * xParallaxFactor,0,0);
        transform.localPosition += new Vector3(0, -parentCamera.getDistanceMoved().y * yParallaxFactor, 0);
    }
}
