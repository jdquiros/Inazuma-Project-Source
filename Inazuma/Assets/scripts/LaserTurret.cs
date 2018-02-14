using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour {

    // Use this for initialization
    public float angularSpeedFactor;
    public float minSpeed;
    public float activationRange;
    public Transform pivotObject;
    private Transform player;
    private Vector2 vecToPlayer;
    private LineRenderer lineRenderer;
    public float laserOriginOffset;
    private Vector2 laserEnd;
    private LayerMask hitLayer;
    public float idleAngle = 180;
    private bool isFiring;
    public bool drawGizmos;
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lineRenderer = GetComponent<LineRenderer>();
        vecToPlayer = player.position - transform.position;
        hitLayer |= (1 << LayerMask.NameToLayer("groundLayer"));
        hitLayer |= (1 << LayerMask.NameToLayer("oneWayLayer"));
        hitLayer |= (1 << LayerMask.NameToLayer("Default"));
        laserEnd = (Vector2)transform.position;
        isFiring = false;
    }

    // Update is called once per frame
    void Update () {
        vecToPlayer = player.position - transform.position;
        if(vecToPlayer.magnitude < activationRange && hasLineOfSight())
        {
           
            rotateTurretToPlayer();
            fireTurret();
            drawLaser();
            isFiring = true;
            
        } else
        {
            lineRenderer.positionCount = 0;
            returnToIdle();
            isFiring = false;
        }
	}
    private void rotateTurretToPlayer()
    {
        
        float angleBetween = Vector2.Angle(vecToPlayer,new Vector2(Mathf.Cos(pivotObject.eulerAngles.z* Mathf.Deg2Rad), Mathf.Sin(pivotObject.eulerAngles.z* Mathf.Deg2Rad)));
        pivotObject.rotation = Quaternion.Slerp(pivotObject.rotation, Quaternion.Euler(new Vector3(0,0,Mathf.Atan2(vecToPlayer.y,vecToPlayer.x)*Mathf.Rad2Deg)) ,(minSpeed+angularSpeedFactor*(angleBetween/360))*Time.deltaTime);
    }
    private void returnToIdle()
    {
        pivotObject.rotation = Quaternion.Slerp(pivotObject.rotation, Quaternion.Euler(new Vector3(0, 0, idleAngle)), (minSpeed+angularSpeedFactor)  * Time.deltaTime);

    }
    private bool hasLineOfSight()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, vecToPlayer, 1000, hitLayer);
        if (hit.collider != null)
            return hit.transform.CompareTag("Player");
        else
            return false;
    }
    private void fireTurret()
    {
        RaycastHit2D[] hits;

        hits = Physics2D.RaycastAll(transform.position,new Vector3(Mathf.Cos(pivotObject.eulerAngles.z*Mathf.Deg2Rad), Mathf.Sin(pivotObject.eulerAngles.z*Mathf.Deg2Rad)), 1000,hitLayer);
        if(hits.Length > 0)
        {
            float minDist = 1;
            int firstHitIndex = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].fraction > 0.03f && hits[i].fraction < minDist && (hits[i].transform.gameObject.layer != LayerMask.NameToLayer("triggerLayer") || hits[i].transform.CompareTag("Player")))
                {
                    minDist = hits[i].fraction;
                    firstHitIndex = i;
                }
            }
            laserEnd = hits[firstHitIndex].point;
            GameObject hitObject = hits[firstHitIndex].transform.gameObject;
            if (hitObject.CompareTag("Player"))
            {
                hitObject.GetComponent<PlayerController>().forceDamagePlayer(1000000);
            }
        } else
        {
            laserEnd = transform.position + new Vector3(Mathf.Cos(pivotObject.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(pivotObject.eulerAngles.z * Mathf.Deg2Rad)) * 1000;
        }
    }
    private void drawLaser()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0,(Vector2)(transform.position + laserOriginOffset * new Vector3(Mathf.Cos(pivotObject.eulerAngles.z* Mathf.Deg2Rad), Mathf.Sin(pivotObject.eulerAngles.z* Mathf.Deg2Rad))));
        lineRenderer.SetPosition(1,laserEnd);
    }
    public Vector2 getLaserEnd()
    {
        return laserEnd;
    }
    public bool firing()
    {
        return isFiring;
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, activationRange);
        }
    }
}
