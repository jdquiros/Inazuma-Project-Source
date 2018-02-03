using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    // Use this for initialization
    public Transform pingPrefab;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            spawnPing();
            Destroy(gameObject);
        }
    }
    private void spawnPing()
    {
        GameObject ping = Instantiate(pingPrefab,transform.position,transform.rotation).gameObject;
    }
}
