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
        
        rb = transform.parent.parent.GetComponent<Rigidbody>();
        screw = transform.parent.parent;
        threadedHole = null;
    }
	
	// Update is called once per frame
	void Update () {
        HoldScrew();
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
        rb.constraints -= RigidbodyConstraints.FreezePositionY;
        rb.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void HoldScrew()
    {
        if(threadedHole != null)
        {
            screw.position = threadedHole.position;
        }
    }
}
