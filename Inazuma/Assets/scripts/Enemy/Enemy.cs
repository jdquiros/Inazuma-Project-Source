using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 1;
    public int damage;

	private bool alive = true;
    private PlayerController playerController;
    private bool invulnerable;
    public bool harmPlayer = true;
    public EnemySpawner spawnedBy;          //is allowed to be null. that means it was not spawned by a Spawner
    void Start()
    {
    }
    
    void kill() {
        if (spawnedBy != null)              //if this enemy was spawned by a spawner, notify it that this enemy is dead
            spawnedBy.removeEnemy();
        spawnExplosion();
		health = 0;
		alive = false;
		Destroy (gameObject);

	}

    public void damageEnemy(int dmg, Vector2 playerToEnemyVector)
    {
        
        if (!invulnerable)
        {
            if(health - dmg <= 0)
            {
                spawnSplatter(playerToEnemyVector);
            }
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
    public void spawnSplatter(Vector2 splatterDirection)
    {
        GameObject splatter = (GameObject)Instantiate(Resources.Load("DirectedSplatter"));
        splatter.transform.position = transform.position;
        splatter.transform.rotation = Quaternion.LookRotation(Vector3.forward,splatterDirection);
    }
    public void spawnExplosion()
    {
        GameObject explosion = (GameObject)Instantiate(Resources.Load("RadialExplosionCyan"));
        explosion.transform.position = transform.position;
    }
	void OnTriggerEnter2D (Collider2D collision) {
		if (collision.gameObject.tag == "Player" && harmPlayer)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.attemptDamagePlayer(damage);
            playerController.knockBackPlayer(transform.position);
        }
	}
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && harmPlayer)
        {
            playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.attemptDamagePlayer(damage);
            playerController.knockBackPlayer(transform.position);
        }
    }
}
