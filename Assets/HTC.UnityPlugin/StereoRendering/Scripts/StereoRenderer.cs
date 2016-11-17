//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using UnityEngine;
using System;

namespace HTC.UnityPlugin.StereoRendering
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class StereoRenderer : MonoBehaviour
    {
        #region variables

        //--------------------------------------------------------------------------------
        // getting/setting stereo anchor pose

        public Transform canvasOrigin;
        [SerializeField]
        private Vector3 m_canvasOriginWorldPosition = new Vector3(0f, 0f, 0f);
        [SerializeField]
        private Vector3 m_canvasOriginWorldRotation = new Vector3(0f, 0f, 0f);

        public Vector3 canvasOriginPos
        {
            get
            {
                if (canvasOrigin == null) { return m_canvasOriginWorldPosition; }
                return canvasOrigin.position;
            }
            set
            {
                m_canvasOriginWorldPosition = value;
            }
        }

        public Vector3 canvasOriginEuler
        {
            get
            {
                if (canvasOrigin == null) { return m_canvasOriginWorldRotation; }
                return canvasOrigin.eulerAngles;
            }
            set
            {
                m_canvasOriginWorldRotation = value;
            }
        }

        public Quaternion canvasOriginRot
        {
            get { return Quaternion.Euler(canvasOriginEuler); }
            set { canvasOriginEuler = value.eulerAngles; }
        }

        public Vector3 localCanvasOriginPos
        {
            get { return transform.InverseTransformPoint(canvasOriginPos); }
            set { canvasOriginPos = transform.InverseTransformPoint(value); }
        }

        public Vector3 localCanvasOriginEuler
        {
            get { return (Quaternion.Inverse(transform.rotation) * Quaternion.Euler(canvasOriginEuler)).eulerAngles; }
            set { canvasOriginEuler = (transform.rotation * Quaternion.Euler(value)).eulerAngles; }
        }

        public Quaternion localCanvasOriginRot
        {
            get { return Quaternion.Inverse(transform.rotation) * canvasOriginRot; }
            set { canvasOriginRot = transform.rotation * value; }
        }

        //--------------------------------------------------------------------------------
        // getting/setting stereo anchor pose

        public Transform anchorTransform;
        [SerializeField]
        private Vector3 m_anchorWorldPosition = new Vector3(0f, 0f, 0f);
        [SerializeField]
        private Vector3 m_anchorWorldRotation = new Vector3(0f, 0f, 0f);

        public Vector3 anchorPos
        {
            get
            {
                if (anchorTransform == null) { return m_anchorWorldPosition; }
                return anchorTransform.position;
            }
            set
            {
                m_anchorWorldPosition = value;
            }
        }

        public Vector3 anchorEuler
        {
            get
            {
                if (anchorTransform == null) { return m_anchorWorldRotation; }
                return anchorTransform.eulerAngles;
            }
            set
            {
                m_anchorWorldRotation = value;
            }
        }

        public Quaternion anchorRot
        {
            get { return Quaternion.Euler(anchorEuler); }
            set { anchorEuler = value.eulerAngles; }
        }

        //--------------------------------------------------------------------------------
        // other variables

        // flags
        private bool canvasVisible = false;
        public bool shouldRender = true;
        public bool isMirror = false;

        // the camera rig that represents HMD
        private GameObject mainCameraParent;
        private Camera mainCameraEye;

        // camera rig for stereo rendering, which is on the object this component attached to
        public GameObject stereoCameraHead = null;
        public Camera stereoCameraEye = null;

        // render texture for stereo rendering
        private RenderTexture leftEyeTexture = null;
        private RenderTexture rightEyeTexture = null;

        // the materials for displaying render result
        private Material stereoMaterial;

        // for mirror transform
        private Vector3 mirrorMatrixScale = new Vector3(-1f, 1f, 1f);

        // list of objects that should be ignored when rendering
        [SerializeField]
        private GameObject[] ignoreWhenRender;

        // for callbacks
        private Action preRenderListeners;
        private Action postRenderListeners;

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
        // initialization and destruction

        private void Start()
        {
            if (Application.isPlaying)
            {
                // disable stereo camera -- shuold be controlled by this script
                stereoCameraEye.enabled = false;

                // get render textures with 4x MSAA
                leftEyeTexture = new RenderTexture((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneHeight, 24);
                leftEyeTexture.antiAliasing = 4;

#if UNITY_5_4_OR_NEWER
                rightEyeTexture = new RenderTexture((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneHeight, 24);
                rightEyeTexture.antiAliasing = 4;
#endif

                // find stereo material and swap shader based on Unity version
                Renderer renderer = GetComponent<Renderer>();
                Material[] materialList = renderer.materials;
                for (int i = 0; i < materialList.Length; i++)
                {
                    Material mt = materialList[i];
                    if (mt.shader.name == "Custom/StereoRenderShader" || mt.shader.name == "Custom/StereoRenderShader_5_3")
                    {
                        stereoMaterial = mt;

#if UNITY_5_4_OR_NEWER
                        renderer.materials[i].shader = Shader.Find("Custom/StereoRenderShader");
#else
                        renderer.materials[i].shader = Shader.Find("Custom/StereoRenderShader_5_3");
#endif

                        break;
                    }
                }

                // get main camera and registor to StereoRenderManager
                StereoRenderManager.Instance.AddToManager(this);
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            // initialize canvas origin
            canvasOrigin = transform;
            canvasOriginPos = transform.position;
            canvasOriginEuler = transform.eulerAngles;

            // initialize anchor
            anchorPos = new Vector3(0f, 0f, 0f);
            anchorEuler = new Vector3(0f, 0f, 0f);

            // initialize stereo camera rig
            stereoCameraHead = new GameObject("Stereo Camera Head [" + gameObject.name + "]");
            stereoCameraHead.transform.parent = transform;

            GameObject stereoCameraEyeObj = new GameObject("Stereo Camera Eye [" + gameObject.name + "]");
            stereoCameraEyeObj.transform.parent = stereoCameraHead.transform;
            stereoCameraEye = stereoCameraEyeObj.AddComponent<Camera>();
            stereoCameraEye.enabled = false;

            // if only one material, substitute it with stereo render material
            Renderer renderer = GetComponent<Renderer>();
            Material[] materials = renderer.sharedMaterials;
            if(materials.Length == 1)
            {
                renderer.sharedMaterial = (Material)Resources.Load("StereoRenderMaterial", typeof(Material));
            }

            // find stereo material and swap shader based on Unity version
            Material[] materialList = renderer.materials;
            for (int i = 0; i < materialList.Length; i++)
            {
                Material mt = materialList[i];
                if (mt.shader.name == "Custom/StereoRenderShader" || mt.shader.name == "Custom/StereoRenderShader_5_3")
                {
                    stereoMaterial = mt;

#if UNITY_5_4_OR_NEWER
                        renderer.materials[i].shader = Shader.Find("Custom/StereoRenderShader");
#else
                    renderer.materials[i].shader = Shader.Find("Custom/StereoRenderShader_5_3");
#endif

                    break;
                }
            }
        }
#endif

        public void InitMainCamera(GameObject parent, Camera cam)
        {
            mainCameraParent = parent;
            mainCameraEye = cam;
        }

        private void OnDestroy()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(stereoCameraHead);
            }
            else if (Application.isPlaying)
            {
                StereoRenderManager.Instance.RemoveFromManager(this);
            }
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                StereoRenderManager.Instance.RemoveFromManager(this);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        // visibility and rendering

        private void OnWillRenderObject()
        {
            if (Camera.current == mainCameraEye)
            {
                canvasVisible = true;
            }
        }

        public void Render()
        {
            // move stereo camera around based on HMD pose
            MoveStereoCameraBasedOnHmdPose();

            // invoke pre-render events
            if (preRenderListeners != null)
                preRenderListeners.Invoke();

            if(canvasVisible)
            {
                // disable specified objects
                for (int i = 0; i < ignoreWhenRender.Length; i++)
                    ignoreWhenRender[i].SetActive(false);

                // invert backface culling when rendering a mirror
                if (isMirror)
                    GL.invertCulling = true;

#if UNITY_5_4_OR_NEWER
                RenderWithTwoTextures();
#else
                RenderWithOneTexture();
#endif

                // reset backface culling
                if (isMirror)
                    GL.invertCulling = false;

                // re-activate specified objects
                for (int i = 0; i < ignoreWhenRender.Length; i++)
                    ignoreWhenRender[i].SetActive(true);

                // finish this render pass, reset visibility
                canvasVisible = false;
            }

            // invoke post-render events
            if (postRenderListeners != null)
                postRenderListeners.Invoke();
        }

        public void MoveStereoCameraBasedOnHmdPose()
        {
            // compute the relationship between main cam and portal entry
#if UNITY_5_4_OR_NEWER
            Vector3 posCanvasToMainCam = mainCameraEye.transform.position - canvasOriginPos;
#else
            Vector3 posCanvasToMainCam = mainCameraParent.transform.position - canvasOriginPos;
#endif 

            // compute the rotation between the portal entry and the portal exit
            Quaternion rotCanvasToAnchor = anchorRot * Quaternion.Inverse(canvasOriginRot);

            // move remote camera position
            Vector3 posAnchorToStereoCam = rotCanvasToAnchor * posCanvasToMainCam;
            stereoCameraHead.transform.position = anchorPos + posAnchorToStereoCam;

            // rotate remote camera
#if UNITY_5_4_OR_NEWER
            stereoCameraHead.transform.rotation = rotCanvasToAnchor * mainCameraEye.transform.rotation;
#else
            stereoCameraHead.transform.rotation = rotCanvasToAnchor * mainCameraParent.transform.rotation;
#endif

            // reflection transform
            if (isMirror)
            {
                Vector3 stereoHeadPos = stereoCameraHead.transform.localPosition;
                stereoHeadPos.x *= -1;
                Vector3 stereoHeadRot = -stereoCameraHead.transform.localEulerAngles;
                stereoHeadRot.x *= -1;
                stereoCameraHead.transform.localPosition = stereoHeadPos;
                stereoCameraHead.transform.localEulerAngles = stereoHeadRot;
            }
        }

        private void RenderWithTwoTextures()
        {
            // compute rotation for eye offset
            Quaternion rotCanvasToAnchor = anchorRot * Quaternion.Inverse(canvasOriginRot);

            // left eye rendering
            Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
            eyeOffset.z = 0.0f;

            if (isMirror)
            {
                stereoCameraEye.transform.localPosition = stereoCameraEye.transform.localPosition + stereoCameraHead.transform.TransformVector(eyeOffset);
                stereoCameraEye.projectionMatrix = HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix(Valve.VR.EVREye.Eye_Left, stereoCameraEye.nearClipPlane, stereoCameraEye.farClipPlane, Valve.VR.EGraphicsAPIConvention.API_DirectX)) * Matrix4x4.Scale(mirrorMatrixScale);
            }
            else
            {
                stereoCameraEye.transform.position = stereoCameraHead.transform.position + rotCanvasToAnchor * eyeOffset;
                stereoCameraEye.projectionMatrix = HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix(Valve.VR.EVREye.Eye_Left, stereoCameraEye.nearClipPlane, stereoCameraEye.farClipPlane, Valve.VR.EGraphicsAPIConvention.API_DirectX));
            }

            stereoCameraEye.targetTexture = leftEyeTexture;
            stereoCameraEye.Render();
            stereoMaterial.SetTexture("_LeftEyeTexture", leftEyeTexture);

            // right eye rendering
            eyeOffset = SteamVR.instance.eyes[1].pos;
            eyeOffset.z = 0.0f;

            if (isMirror)
            {
                stereoCameraEye.transform.localPosition = stereoCameraEye.transform.localPosition + stereoCameraHead.transform.TransformVector(eyeOffset);
                stereoCameraEye.projectionMatrix = HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix(Valve.VR.EVREye.Eye_Right, stereoCameraEye.nearClipPlane, stereoCameraEye.farClipPlane, Valve.VR.EGraphicsAPIConvention.API_DirectX)) * Matrix4x4.Scale(mirrorMatrixScale);
            }
            else
            {
                stereoCameraEye.transform.position = stereoCameraEye.transform.parent.transform.position + rotCanvasToAnchor * eyeOffset;
                stereoCameraEye.projectionMatrix = HMDMatrix4x4ToMatrix4x4(SteamVR.instance.hmd.GetProjectionMatrix(Valve.VR.EVREye.Eye_Right, stereoCameraEye.nearClipPlane, stereoCameraEye.farClipPlane, Valve.VR.EGraphicsAPIConvention.API_DirectX));
            }

            stereoCameraEye.targetTexture = rightEyeTexture;
            stereoCameraEye.Render();
            stereoMaterial.SetTexture("_RightEyeTexture", rightEyeTexture);
        }

        private void RenderWithOneTexture()
        {
            // move remote camera eye to the corresponding position of the main camera eye
            // note that the main camera head pose has been syncronized with remote camera head
            Vector3 vectorHeadToEye = mainCameraEye.transform.position - mainCameraParent.transform.position;
            Quaternion rotCanvasToAnchor = anchorRot * Quaternion.Inverse(canvasOriginRot);

            // sync projection matrix
            if (isMirror)
            {
                stereoCameraEye.transform.position = stereoCameraHead.transform.position + vectorHeadToEye;
                stereoCameraEye.projectionMatrix = mainCameraEye.projectionMatrix * Matrix4x4.Scale(mirrorMatrixScale);
            }
            else
            {
                stereoCameraEye.transform.position = stereoCameraHead.transform.position + rotCanvasToAnchor * vectorHeadToEye;
                stereoCameraEye.projectionMatrix = mainCameraEye.projectionMatrix;
            }

            // render current eye
            stereoCameraEye.targetTexture = leftEyeTexture;
            stereoMaterial.SetTexture("_MainTexture", leftEyeTexture);
            stereoCameraEye.Render();
        }

        /////////////////////////////////////////////////////////////////////////////////
        // callbacks and utilities

        public void AddPreRenderListener(Action listener)
        {
            if (listener == null) { return; }
            preRenderListeners += listener;
        }

        public void AddPostRenderListener(Action listener)
        {
            if (listener == null) { return; }
            postRenderListeners += listener;
        }

        public void RemovePreRenderListener(Action listener)
        {
            if (listener == null) { return; }
            preRenderListeners -= listener;
        }

        public void RemovePostRenderListener(Action listener)
        {
            if (listener == null) { return; }
            postRenderListeners -= listener;
        }

        private Matrix4x4 HMDMatrix4x4ToMatrix4x4(Valve.VR.HmdMatrix44_t hmdMatrix)
        {
            Matrix4x4 m = Matrix4x4.identity;

            m[0, 0] = hmdMatrix.m0;
            m[0, 1] = hmdMatrix.m1;
            m[0, 2] = hmdMatrix.m2;
            m[0, 3] = hmdMatrix.m3;

            m[1, 0] = hmdMatrix.m4;
            m[1, 1] = hmdMatrix.m5;
            m[1, 2] = hmdMatrix.m6;
            m[1, 3] = hmdMatrix.m7;

            m[2, 0] = hmdMatrix.m8;
            m[2, 1] = hmdMatrix.m9;
            m[2, 2] = hmdMatrix.m10;
            m[2, 3] = hmdMatrix.m11;

            m[3, 0] = hmdMatrix.m12;
            m[3, 1] = hmdMatrix.m13;
            m[3, 2] = hmdMatrix.m14;
            m[3, 3] = hmdMatrix.m15;

            return m;
        }
    }
}