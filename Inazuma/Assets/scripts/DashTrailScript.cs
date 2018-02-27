using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailScript : MonoBehaviour {

    // Use this for initialization
    private PlayerController playerController;
    private float destroyDelay;
    public float rotationSpeed;
    public float direction = 1;
    private bool detached = false;
    void Start () {
        playerController = transform.parent.GetComponent<PlayerController>();
        destroyDelay = GetComponentInChildren<TrailRenderer>().time;
    }

    // Update is called once per frame
    void LateUpdate () {
        if (detached)
        {
            if (transform.parent != null)
            {
                transform.position = transform.parent.position;
                transform.parent = null;
                Destroy(gameObject, destroyDelay);
            }
        }
        else
        {
            if (transform.parent != null)
            {
                transform.localPosition = Vector3.zero;
                if(playerController.moving())
                    transform.Rotate(new Vector3(0, 0, rotationSpeed * direction) * Time.deltaTime);
            }
        }
    }
    void onDetach()
    {
        detached = true;
    }
}
