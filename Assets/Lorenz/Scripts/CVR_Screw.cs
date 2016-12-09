using UnityEngine;
using System.Collections;
using System;

public class CVR_Screw : MonoBehaviour {

    bool engaged;
    Rigidbody rb;
    Transform screw;
    Transform threadedHole;

	// Use this for initialization
	void Start () {

        InitRigidBody();
        screw = transform.parent.parent;
        threadedHole = null;
    }
	
	// Update is called once per frame
	void Update () {
        //HoldScrew();
	}

    private void InitRigidBody()
    {
        rb = transform.parent.parent.GetComponent<Rigidbody>();
        //if (rb == null)
        //{
        //    rb = gameObject.AddComponent<Rigidbody>();
        //}
        rb.isKinematic = false;
        //rb.useGravity = false;
        rb.angularDrag = 10; // otherwise knob will continue to move too far on its own
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CVR_ThreadHole>() != null && threadedHole == null)
        {
            print("collision");
            threadedHole = other.transform;

            EngageScrew();
        }
    }

    private void EngageScrew()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //rb.constraints -= RigidbodyConstraints.FreezeRotationY;
        //rb.constraints -= RigidbodyConstraints.FreezePositionY;
        rb.transform.rotation = Quaternion.Euler(0, 0, 0);
        HoldScrew();
    }

    private void HoldScrew()
    {
        if(threadedHole != null)
        {
            screw.position = threadedHole.position;
        }
    }
}
