using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircleCollider : MonoBehaviour {

    // Use this for initialization
    private CircleCollider2D col;
	void Start () {
        col = GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, col.radius*transform.localScale.x);
    }
}
