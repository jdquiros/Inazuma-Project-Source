using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 1;
    public int damage;

	private bool alive = true;
    private PlayerController playerController;
    void Start()
    {
        spawnHealthBar();
    }
    private void spawnHealthBar()
    {
        GameObject healthBar = (GameObject)Instantiate(Resources.Load("HealthBar"));
        healthBar.transform.SetParent(GameObject.FindGameObjectWithTag("HealthBarParent").transform);

        healthBar.GetComponentInChildren<HealthBar>().setTarget(transform);
    }
    void kill() {
		health = 0;
		alive = false;
		Destroy (gameObject);
	}

    public void damageEnemy(int dmg)
    {
        setHealth(health - dmg);
    }

    void setHealth(int newHealth)
    {
        health = newHealth;
        if (health <= 0) {
            kill();
        }
    }

	void OnTriggerEnter2D (Collider2D collision) {
        print("hit");
		if (collision.gameObject.tag == "Player")
        {
            print("collision");
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.damagePlayer(damage);
            playerController.knockBackPlayer(transform.position);
        }
	}
}
