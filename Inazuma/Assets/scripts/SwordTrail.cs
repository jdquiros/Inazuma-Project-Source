using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrail : MonoBehaviour {

    // Use this for initialization
    private PlayerController player;
    private TrailRenderer tr;
    public float rotationSpeed;
    public Vector2 attackAngleRange;            //attackAngleRange.x is starting angle, attackAngleRange.y is end angle
    private IEnumerator swingFunc;
    private bool isRotating;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //if(Vector3.Distance(parentTransform.position,transform.position) > 0.1f)
        //      {
        //          tr.enabled = false;
        //          transform.position = parentTransform.position;
        //          tr.enabled = true;
        //      }
        if (isRotating)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
            
        }
	}
    public void startSwing(float rotSpeed, Vector2 range, Vector3 pos, Vector3 offset, bool flipY)
    {
        attackAngleRange = range;
        player = GetComponentInParent<PlayerController>();
        transform.localPosition = pos+offset;
        rotationSpeed = rotSpeed;
        if (!flipY)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, attackAngleRange.x));
        } else if (flipY)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, attackAngleRange.y));
            rotationSpeed *= -1;
        } 
        tr = GetComponentInChildren<TrailRenderer>();
        tr.Clear();
        if(swingFunc != null)
            StopCoroutine(swingFunc);
        swingFunc = unParentAfterDelay( Mathf.Abs((attackAngleRange.y - attackAngleRange.x)/rotationSpeed));
        StartCoroutine(swingFunc);
    }
    private IEnumerator unParentAfterDelay(float delay)
    {
        isRotating = true;
        yield return new WaitForSeconds(delay);
        isRotating = false;
        Destroy(gameObject, tr.time);
    }
}
