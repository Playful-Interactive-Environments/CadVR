using System;
using UnityEngine;

public class AddTriggerBox : BaseGameObjectProcessor {

    [SerializeField]
    private bool isKinematic = true;
    [SerializeField]
    private bool useGravitiy = true;


    public override void Process(GameObject asset)
    {
        Bounds bounds = GetBounds(asset);
        BoxCollider box = asset.GetComponent<BoxCollider>();
        if (!box)
        {
            box = asset.AddComponent<BoxCollider>();
        }

        box.center = bounds.center;
        box.size = bounds.size;

        Rigidbody body = asset.GetComponent<Rigidbody>();
        if (!body)
        {
            body = asset.AddComponent<Rigidbody>();
        }

        body.isKinematic = isKinematic;
        body.useGravity = useGravitiy;
    }


    private Bounds GetBounds(GameObject t)
    {
        Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
        Bounds bounds;

        bounds = new Bounds(t.transform.position, Vector3.zero);

        foreach(Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }
}
