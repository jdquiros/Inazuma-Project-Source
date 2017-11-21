using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    private Vector3 moveDirection;
    public float speed;
    public int damage = 1;
	void Start () {
        if (moveDirection == Vector3.zero)
            moveDirection = Vector3.right;
        transform.GetComponent<Rigidbody2D>().velocity = moveDirection*speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void setVelocity(Vector3 newDir,float newSpeed)
    {
        moveDirection = newDir.normalized;
        speed = newSpeed;
        transform.GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().attemptDamagePlayer(damage);
            collision.gameObject.GetComponent<PlayerController>().knockBackPlayer(transform.position);
            kill();          
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("groundLayer"))
        {
            kill();
        }

    }
   
    public void kill()
    {
        GameObject explosion = (GameObject)Instantiate(Resources.Load("RadialExplosionRed"));
        explosion.transform.position = transform.position;
        Destroy(gameObject);
    }
}
