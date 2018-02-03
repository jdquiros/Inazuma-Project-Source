using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeOscillation : MonoBehaviour {

    // Use this for initialization
    public float speed;
    public float amplitude;
    public float periodOffset;
    private Vector3 originalScale;
    private float scale;
	void Start () {
        originalScale = transform.localScale;
        scale = 0;
	}
	
	// Update is called once per frame
	void Update () {
        scale = amplitude * Mathf.Cos(Time.time * speed + periodOffset);
        transform.localScale = originalScale + new Vector3(scale,scale,0); 
	}
}
