using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTrailRenderers : MonoBehaviour {

    // Use this for initialization
    private PlayerController player;
    private TrailRenderer trailRenderer;
	void Start () {
        player = transform.parent.parent.gameObject.GetComponent<PlayerController>();
        trailRenderer = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(player.movementState == PlayerController.MovementState.Dash || player.movementState == PlayerController.MovementState.Lunge)
        {
            trailRenderer.startColor = Color.yellow;
            trailRenderer.endColor = new Color(1, 1, 0, 0);
        }
        else
        {
            trailRenderer.startColor = new Color(1, 1, 0, 0);
            trailRenderer.endColor = new Color(1, 1, 0, 0);
        }
	}
}
