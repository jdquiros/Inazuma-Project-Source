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
        if (transform.parent != null)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
            if (player.movementState == PlayerController.MovementState.Lunge)
                transform.parent = null;
        }
	}
    public void startSwing(float rotSpeed, Vector2 range, Transform pTransform, Vector3 offset, bool flipY)
    {
        attackAngleRange = range;
        transform.parent = pTransform;
        player = GetComponentInParent<PlayerController>();
        transform.localPosition = offset;
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
        yield return new WaitForSeconds(delay);
        transform.parent = null;
        Destroy(gameObject, tr.time);
    }
}
