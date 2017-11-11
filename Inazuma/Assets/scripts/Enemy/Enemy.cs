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

    public void damageEnemy(int dmg)
    {
        setHealth(health - dmg);
    }

    void setHealth(int health)
    {
        if (health <= 0) {
            kill();
        }
    }



	void OnTriggerEnter2D (Collider2D collision) {
		if (collision.gameObject.tag == "Player")
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.damagePlayer(damage);
            playerController.knockBackPlayer(transform.position);
        }
	}
}
