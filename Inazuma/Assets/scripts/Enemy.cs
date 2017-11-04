using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 1;

	private bool alive = true;
	private Vector3 enemyPos;
    private PlayerController playerController;

    void Start () {
		enemyPos = transform.localPosition;
	}

	void Update () {
		transform.localPosition = enemyPos + new Vector3 (0, Mathf.Sin (3*Time.time)/2, 0);
	}
		
	void kill() {
		health = 0;
		alive = false;
		Destroy (gameObject);
	}

	void OnTriggerEnter2D (Collider2D collision) {
		if (collision.gameObject.layer == LayerMask.NameToLayer ("attackLayer")) {
			kill ();
		} else if (collision.gameObject.tag == "Player")
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.damagePlayer(1);
        }
	}
}
