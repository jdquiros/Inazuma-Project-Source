using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 1;
    public int damage;

	private bool alive = true;
    private PlayerController playerController;

		
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
            playerController.damagePlayer(damage);
            playerController.knockBackPlayer(transform.position);
        }
	}
}
