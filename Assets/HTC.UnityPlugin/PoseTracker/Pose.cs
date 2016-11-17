//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using UnityEngine;

namespace HTC.UnityPlugin.PoseTracker
{
    /// <summary>
    /// Describe a pose by position and rotation
    /// </summary>
    public struct Pose
    {
        public Vector3 pos;
        public Quaternion rot;

        public static Pose identity
        {
            get { return new Pose(Vector3.zero, Quaternion.identity); }
        }

        public Pose(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }

        public Pose(Transform t, bool useLocal = false)
        {
            if(t == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
            }
            else if (!useLocal)
            {
                pos = t.position;
                rot = t.rotation;
            }
            else
            {
                pos = t.localPosition;
                rot = t.localRotation;
            }
        }

        public override bool Equals(object o)
        {
            if (o is Pose)
            {
                Pose t = (Pose)o;
                return pos == t.pos && rot == t.rot;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ rot.GetHashCode();
        }

        public static bool operator ==(Pose a, Pose b)
        {
            return a.pos == b.pos && a.rot == b.rot;
        }

        public static bool operator !=(Pose a, Pose b)
        {
            return a.pos != b.pos || a.rot != b.rot;
        }

        public static Pose operator *(Pose a, Pose b)
        {
            return new Pose
            {
                rot = a.rot * b.rot,
                pos = a.pos + a.rot * b.pos
            };
        }

        public void Multiply(Pose a, Pose b)
        {
            rot = a.rot * b.rot;
            pos = a.pos + a.rot * b.pos;
        }

        public void Inverse()
        {
            rot = Quaternion.Inverse(rot);
            pos = -(rot * pos);
        }

        public Pose GetInverse()
        {
            var t = new Pose(pos, rot);
            t.Inverse();
            return t;
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Quaternion.Inverse(rot) * (point - pos);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return pos + (rot * point);
        }

        public static Pose Lerp(Pose a, Pose b, float t)
        {
            return new Pose(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
        }

        public void Lerp(Pose to, float t)
        {
            pos = Vector3.Lerp(pos, to.pos, t);
            rot = Quaternion.Slerp(rot, to.rot, t);
        }

        public static Pose LerpUnclamped(Pose a, Pose b, float t)
        {
            return new Pose(Vector3.LerpUnclamped(a.pos, b.pos, t), Quaternion.SlerpUnclamped(a.rot, b.rot, t));
        }

        public void LerpUnclamped(Pose to, float t)
        {
            pos = Vector3.LerpUnclamped(pos, to.pos, t);
            rot = Quaternion.SlerpUnclamped(rot, to.rot, t);
        }

        public static void SetPose(Transform target, Pose pose, Transform origin = null)
        {
            if (origin != null && origin != target.parent)
            {
                pose = new Pose(origin) * pose;
                pose.pos.Scale(origin.localScale);
                target.position = pose.pos;
                target.rotation = pose.rot;
            }
            else
            {
                target.localPosition = pose.pos;
                target.localRotation = pose.rot;
            }
        }

        public static Pose FromToPose(Pose from, Pose to)
        {
            var invRot = Quaternion.Inverse(from.rot);
            return new Pose(invRot * (to.pos - from.pos), invRot * to.rot);
        }
    }
}