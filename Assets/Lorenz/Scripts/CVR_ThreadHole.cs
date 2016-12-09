using UnityEngine;
using System.Collections;

public class CVR_ThreadHole : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
    }
}
