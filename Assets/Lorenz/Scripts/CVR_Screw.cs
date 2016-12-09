using UnityEngine;
using System.Collections;
using System;

public class CVR_Screw : MonoBehaviour {

    bool engaged;
    Rigidbody rb;
    VRTK.VRTK_InteractableObject io;
    public Transform threadedHole;

	// Use this for initialization
	void Start () {

        InitRigidBody();
        InitInteractable();
        threadedHole = null;
        //TestKnob();
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void InitRigidBody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        //rb.useGravity = false;
        rb.angularDrag = 10; // otherwise knob will continue to move too far on its own
    }

    private void InitInteractable()
    {
        io = GetComponent<VRTK.VRTK_InteractableObject>();
        if (io == null)
        {
            io = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        }
        io.isGrabbable = true;
        io.precisionSnap = true;
        io.stayGrabbedOnTeleport = false;
        io.grabOverrideButton = VRTK.VRTK_ControllerEvents.ButtonAlias.Trigger_Touch;
        io.grabAttachMechanic = VRTK.VRTK_InteractableObject.GrabAttachType.Track_Object;
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
        rb.constraints -= RigidbodyConstraints.FreezeRotationZ;
        //rb.constraints -= RigidbodyConstraints.FreezePositionY;
        transform.position = threadedHole.position;
        transform.rotation = threadedHole.rotation;
        transform.parent = threadedHole.transform;
    }
}
