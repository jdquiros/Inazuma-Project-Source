using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZigZagParticle : MonoBehaviour {

    // Use this for initialization
    public bool randomizeDirection = false;
    public bool inverted = false;
    public float speed;
    public float moveAngle;
    public float zigZagInterval;
    public float zigZagTurnAngle;
    public float lifeTime;
    private Vector3 moveVector;
    private float currentMoveAngle;

	void Start () {
        if (randomizeDirection)
            moveAngle = Random.value * 360;
        if (!inverted)
        {
            currentMoveAngle = moveAngle - zigZagTurnAngle / 2;
        } else
        {
            currentMoveAngle = moveAngle + zigZagTurnAngle / 2;
            zigZagTurnAngle *= -1;
        }
        moveVector = new Vector3(Mathf.Cos(currentMoveAngle * Mathf.Deg2Rad), Mathf.Sin(currentMoveAngle * Mathf.Deg2Rad), 0);
        StartCoroutine(zigZag());
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate () {
        transform.position += moveVector * speed * Time.deltaTime;
	}
    private IEnumerator zigZag()
    {
        while (true)
        {
            yield return new WaitForSeconds(zigZagInterval);
            currentMoveAngle += zigZagTurnAngle;
            moveVector = new Vector3(Mathf.Cos(currentMoveAngle * Mathf.Deg2Rad), Mathf.Sin(currentMoveAngle * Mathf.Deg2Rad), 0);
            yield return new WaitForSeconds(zigZagInterval);
            currentMoveAngle -= zigZagTurnAngle;
            moveVector = new Vector3(Mathf.Cos(currentMoveAngle * Mathf.Deg2Rad), Mathf.Sin(currentMoveAngle * Mathf.Deg2Rad), 0);
        }
    }
}
