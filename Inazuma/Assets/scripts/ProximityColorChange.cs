using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityColorChange : MonoBehaviour {

    // Use this for initialization
    public float activationDistance;
    public Color startColor = Color.white;
    public Color endColor = Color.white;
    public float duration = 0;
    private bool playerInRange = false;
    private float colorChangeTimer = 0;
    private TextMesh textMesh;
    private Transform playerTransform;
	void Start () {
        if (duration <= 0)
            duration = 0.001f;      //duration can't be zero, because div by zero errors

        textMesh = GetComponent<TextMesh>();
        textMesh.color = startColor;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position,playerTransform.position) < activationDistance)
        {
            if(colorChangeTimer < duration)
            {
                colorChangeTimer += Time.deltaTime;
            }
        } else
        {

            if(colorChangeTimer > 0)
            {
                colorChangeTimer -= Time.deltaTime;
            }
        }
        textMesh.color = Color.Lerp(startColor, endColor, colorChangeTimer / duration);

    }
}
