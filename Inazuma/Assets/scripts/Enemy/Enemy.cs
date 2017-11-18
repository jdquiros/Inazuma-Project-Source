using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 1;
    public int damage;

	private bool alive = true;
    private PlayerController playerController;
    private bool invulnerable;
    void Start()
    {
    }
    
    void kill() {
		health = 0;
		alive = false;
		Destroy (gameObject);
	}

    public void damageEnemy(int dmg)
    {
        
        if (!invulnerable)
        {
            setHealth(health - dmg);
            StartCoroutine(invulnerableForDuration(.02f));
        }

    }
    private IEnumerator invulnerableForDuration(float duration)
    {
        invulnerable = true;
        yield return new WaitForSeconds(duration);
        invulnerable = false;
    }
    void setHealth(int newHealth)
    {
        health = newHealth;
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
