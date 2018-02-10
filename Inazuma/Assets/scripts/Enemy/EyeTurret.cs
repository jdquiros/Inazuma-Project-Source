using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTurret : MonoBehaviour {

    // Use this for initialization
    public float activationDistance;
    public Transform bulletType;
    public int burstCount;
    public float fireDelay;
    public float burstFireDelay;
    private float fireTimer;
    public float bulletVelocity;
    public float aimDeviation;
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
            if (fireTimer < fireDelay)
            {
                fireTimer += Time.deltaTime;
                lookAtTarget(playerTransform.position);
            }
            else
                break;
            yield return null;
        }

        fireBullet(0);
        for(int i = 0; i < burstCount-1; i++)
        {
            yield return new WaitForSeconds(burstFireDelay);
            fireBullet(aimDeviation);
        }
        fireTimer = 0;

        firing = false;

    }
 
    private void fireBullet(float deviation)
    {
        int rngSign = Random.value > 0.5f ? 1 : -1;
        GameObject bullet = Instantiate(bulletType,eyeObject.transform.position,eyeObject.transform.rotation,null).gameObject;
        float shootAngle = Mathf.Deg2Rad*(desiredAngle+ rngSign * Random.value * deviation);
        bullet.GetComponent<Bullet>().setVelocity(new Vector3(Mathf.Cos(shootAngle),
            Mathf.Sin(shootAngle)), bulletVelocity);
        
       
    }
}
