using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbedProjectile : Bullet {

    // Use this for initialization
    public float gravity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetComponent<Rigidbody2D>().velocity += Vector2.down * gravity*Time.deltaTime;
    }
    public void setVelocity(Vector2 vel)
    {
        transform.GetComponent<Rigidbody2D>().velocity = vel;
    }
    
    

    
}
