using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    public struct Pose
    {
        public Vector3 pos;
        public Quaternion rot;

        public Pose(Vector3 p, Quaternion r) { pos = p; rot = r; }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Quaternion.Inverse(rot) * (point - pos);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return pos + (rot * point);
        }
    }

    public float initGrabDistance = 0.5f;

    private Rigidbody rigid;
    private PointerEventData dragEvent;
    private Camera dragCam;
    private Vector3 deltaPos;
    private Quaternion deltaRot;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (dragEvent == null)
        {
            eventData.useDragThreshold = false;

            dragEvent = eventData;
            dragCam = eventData.pointerPressRaycast.module.eventCamera;

            var eventPose = GetEventPose(eventData.position, dragCam);

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    {
                        deltaPos = eventPose.InverseTransformPoint(transform.position);
                        deltaRot = Quaternion.Inverse(eventPose.rot) * transform.rotation;
                        break;
                    }
                case PointerEventData.InputButton.Middle:
                case PointerEventData.InputButton.Right:
                    {
                        var distance = eventData.pointerPressRaycast.distance;
                        var ray = dragCam.ScreenPointToRay(eventData.position);
                        deltaPos = eventPose.InverseTransformPoint((transform.position - ray.GetPoint(distance)) + ray.GetPoint(Mathf.Min(distance, initGrabDistance)));
                        deltaRot = Quaternion.Inverse(eventPose.rot) * transform.rotation;
                        break;
                    }
                default:
                    {
                        dragEvent = null;
                        return;
                    }
            }

            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragEvent == eventData)
        {
            var eventPose = GetEventPose(eventData.position, dragCam);

            transform.position = eventPose.TransformPoint(deltaPos);
            transform.rotation = eventPose.rot * deltaRot;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragEvent == eventData)
        {
            var eventPose = GetEventPose(eventData.position, dragCam);

            var dropPose = new Pose(eventPose.TransformPoint(deltaPos), eventPose.rot * deltaRot);

            rigid.AddForce(rigid.mass * (Vector3.ClampMagnitude(dropPose.pos - transform.position, 0.1f)) / Time.deltaTime, ForceMode.Impulse);
            rigid.AddTorque(rigid.mass * Vector3.Cross(transform.forward, dropPose.rot * Vector3.forward) / Time.deltaTime);

            dragEvent = null;
            dragCam = null;
        }
    }

    public static Pose GetEventPose(Vector2 screenPos, Camera cam)
    {
        var pose = new Pose();
        var ray = cam.ScreenPointToRay(screenPos);
        pose.pos = ray.origin;
        pose.rot = Quaternion.LookRotation(ray.direction, cam.transform.up);
        return pose;
    }
}
