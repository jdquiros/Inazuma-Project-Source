using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTurret : MonoBehaviour {

    // Use this for initialization
    public float activationDistance;
    public Transform bulletType;
    public float fireDelay;
    private float fireTimer;
    public float bulletVelocity;
    public Vector2 randomLookDuration;
    private float randomLookTimer = 0;
    public float lookSpeed;
    private float desiredAngle = 0;
    private Transform playerTransform;
    public Transform eyeObject;
    public SpriteRenderer pupilSprite;
    public Color pupilStartColor;
    public Color pupilEndColor;
    private bool firing;
    void Start () {
        desiredAngle = 0;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(eyeObject.position, playerTransform.position) < activationDistance)
        {
            if (!firing)
                StartCoroutine(turretFire());
        }
        else
        {
            if (fireTimer > 0)
                fireTimer -= Time.deltaTime*2;
            else
                fireTimer = 0;
            randomEyeMovement();
        }
        eyeObject.rotation = Quaternion.Lerp(eyeObject.rotation, Quaternion.Euler(new Vector3(0, 0, desiredAngle)), lookSpeed * Time.deltaTime);
        pupilSprite.color = Color.Lerp(pupilStartColor, pupilEndColor, fireTimer / fireDelay);
    }
    private void randomEyeMovement()
    {
        if (randomLookTimer > 0)
        {
            randomLookTimer -= Time.deltaTime;
        }
        else
        {
            desiredAngle = Random.Range(0, 360);
            randomLookTimer = Random.Range(randomLookDuration.x, randomLookDuration.y);
        }
    }
    private void lookAtTarget(Vector3 v)
    {
        desiredAngle = Mathf.Atan2(v.y - eyeObject.position.y, v.x - eyeObject.position.x) * Mathf.Rad2Deg;
    }
    private IEnumerator turretFire()
    {
        firing = true;
        lookAtTarget(playerTransform.position);

        while (true)
        {
            if (fireTimer < fireDelay / 2)
            {
                fireTimer += Time.deltaTime;
                lookAtTarget(playerTransform.position);
            }
            else if (fireTimer < fireDelay)
                fireTimer += Time.deltaTime;
            else
                break;
            yield return null;
        }

        fireTimer = 0;
        fireBullet();
        firing = false;

    }
 
    private void fireBullet()
    {
        GameObject bullet = Instantiate(bulletType,eyeObject.transform.position,eyeObject.transform.rotation,null).gameObject;
        bullet.GetComponent<Bullet>().setVelocity(new Vector3(Mathf.Cos(desiredAngle*Mathf.Deg2Rad),Mathf.Sin(desiredAngle*Mathf.Deg2Rad)), bulletVelocity);
    }
}
