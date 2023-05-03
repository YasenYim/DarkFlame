using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowCam : MonoBehaviour
{
    enum UpdateWay
    {
        Update,
        FixedUpdate,
    }

    public Transform target;

    [SerializeField]
    UpdateWay updateWay;

    Vector3 offset;

    void Start ()
    {
        offset = transform.position - target.position;
    }
	
	void Update ()
    {
        if (updateWay == UpdateWay.Update)
        {
            Vector3 p = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, p, 0.3f);
        }
	}

    private void FixedUpdate()
    {
        if (updateWay == UpdateWay.FixedUpdate)
        {
            Vector3 p = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, p, 0.3f);
        }
    }
}
