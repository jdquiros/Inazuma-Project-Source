using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOscillation : MonoBehaviour {

    // Use this for initialization
    public Vector2 widthRange;
    public float periodSpeed;
    private LineRenderer lineRenderer;

	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        lineRenderer.startWidth = Mathf.Lerp(widthRange.x,widthRange.y,Mathf.Abs(Mathf.Cos(Time.time*periodSpeed)));
        lineRenderer.endWidth = Mathf.Lerp(widthRange.x, widthRange.y, Mathf.Abs(Mathf.Cos(Time.time * periodSpeed)));

    }
}
