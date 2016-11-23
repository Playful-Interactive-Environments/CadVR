using UnityEngine;
using System.Collections;

public class LerpToTarget : MonoBehaviour {

    public Transform targetTransform;
    [Range(0, 1)]
    public float rotationLerpSpeed;
    [Range(0, 1)]
    public float translationLerpSpeed;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (targetTransform != null)
        {
            if(rotationLerpSpeed < 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, rotationLerpSpeed);
            }else
            {
                transform.rotation = targetTransform.rotation;
            }
            if (translationLerpSpeed < 1)
            {
                transform.position = Vector3.Lerp(transform.position, targetTransform.position, translationLerpSpeed);
            }
            else
            {
                transform.position = targetTransform.transform.position;
            }
        }

	}
}
