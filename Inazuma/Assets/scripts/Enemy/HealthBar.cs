using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    // Use this for initialization
    private Transform enemyTransform;
    private Enemy enemy;
    private int enemyMaxHealth;
    private float rightAnchorX;
    private SpriteRenderer outsideSprite;
    private SpriteRenderer insideSprite;
    private float enemyHealthPercent;
    private bool visible;
	void Start () {
        rightAnchorX = transform.localScale.x;
        outsideSprite = GetComponentInParent<SpriteRenderer>();
        insideSprite = GetComponentInChildren<SpriteRenderer>();
        visible = false;
        enemyHealthPercent = 1f;
        enemyTransform = transform.parent.parent;
        enemy = enemyTransform.gameObject.GetComponent<Enemy>();
        enemyMaxHealth = enemy.health;
	}

   

    private void Update()
    {
        enemyHealthPercent = enemy.health * 1.0f / enemyMaxHealth;
        transform.localScale = new Vector3(Mathf.Lerp(0, rightAnchorX, enemyHealthPercent), transform.localScale.y, 1);
        if(enemyHealthPercent <= .99f)
        {
            visible = true;
        } else
        {
            visible = false;
        }
        if (visible)
        {
            outsideSprite.enabled = true;
            insideSprite.enabled = true;
        } else
        {
            outsideSprite.enabled = false;
            insideSprite.enabled = false;
        }
        

    }

}
