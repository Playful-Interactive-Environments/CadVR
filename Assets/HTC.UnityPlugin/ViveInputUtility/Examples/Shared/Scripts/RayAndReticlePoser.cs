using HTC.UnityPlugin.Vive;
using UnityEngine;

public class RayAndReticlePoser : MonoBehaviour
{
    public ViveRaycaster raycaster;
    public Transform rayScaler;
    public Transform reticle;

    public GameObject hitTarget;
    public float hitDistance;

    protected virtual void LateUpdate()
    {
        var result = raycaster.FirstRaycastResult();
        if (!result.isValid || result.gameObject == null)
        {
            var scaledDistance = transform.InverseTransformVector(0f, 0f, raycaster.FarDistance).magnitude;
            rayScaler.localScale = new Vector3(1f, 1f, scaledDistance);
            if (reticle.gameObject.activeSelf) { reticle.gameObject.SetActive(false); }
        }
        else
        {
            var scaledDistance = transform.InverseTransformVector(0f, 0f, result.distance).magnitude;
            rayScaler.localScale = new Vector3(1f, 1f, scaledDistance);
            if (!reticle.gameObject.activeSelf) { reticle.gameObject.SetActive(true); }
            reticle.position = result.worldPosition;
            //Debug.Log(result.gameObject.name + "distance=" + result.distance);
        }
        hitTarget = result.gameObject;
        hitDistance = result.distance;
    }
}
