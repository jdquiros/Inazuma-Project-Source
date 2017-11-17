using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    // Use this for initialization
    private Transform enemyTransform;
    private Enemy enemy;
    public Vector3 offset;
    private Transform parentTransform;
    private int enemyMaxHealth;
    public float width;
    public float height;
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (enemy != null)
        {
            transform.localScale = new Vector3(Mathf.Clamp(enemy.health * 1f / enemyMaxHealth, 0, 1), 1, 1);
            print(Mathf.Clamp(enemy.health * 1f / enemyMaxHealth, 0, 1));
            parentTransform.position = enemyTransform.position + offset;
       
        } else
        {
            Destroy(transform.parent.gameObject);
        }
        
	}
    public void setTarget(Transform enemyTrans)
    {
        enemyTransform = enemyTrans;
        enemy = enemyTransform.GetComponent<Enemy>();
        enemyMaxHealth = enemy.health;
        parentTransform = transform.parent;
        parentTransform.position = enemyTransform.position + offset;
        parentTransform.localScale = new Vector3(1, 1, 1);

    }
}
