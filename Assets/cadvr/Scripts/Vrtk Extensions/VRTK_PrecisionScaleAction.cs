namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;
    /// <summary>
    /// This secondary grab action allows vor precise rotation, scale and translation via two controllers.
    /// This secondary action is only compatible with the VRTK_ChildOfControllerGrabAttach script because joints take full controller over
    /// the transform of the grabbed object
    /// </summary>
    public class VRTK_PrecisionScaleAction : VRTK_BaseGrabAction
    {
        //protected new bool isSwappable = true;

        private Vector3 initialScale;
        private Quaternion initialRotation;
        private Vector3 AMt1;
        private Vector3 ABt1;
        private float ABt1Magnitude;
        private Vector3 At1;


        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary controller.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary controller.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary controller.</param>
        /// <param name="primaryGrabPoint">The point on the object where the primary controller initially grabbed the object.</param>
        /// <param name="secondaryGrabPoint">The point on the object where the secondary controller initially grabbed the object.</param>
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            // generate own grabpoints because the one passed by vrtk is parented under the grabbed object.
            // these ones move with the grabbing objects

            GameObject primaryGo = new GameObject("primary grab point");
            primaryGo.transform.position = primaryGrabPoint.position;
            primaryGo.transform.SetParent(currentPrimaryGrabbingObject.transform, true);
            Transform primaryGlobalGrabPoint = primaryGo.transform;

            GameObject secondaryGo = new GameObject("secondary grab point");
            secondaryGo.transform.position = secondaryGrabPoint.position;
            secondaryGo.transform.SetParent(currentSecondaryGrabbingObject.transform, true);
            Transform secondaryGlobalGrabPoint = secondaryGo.transform;

            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGlobalGrabPoint, secondaryGlobalGrabPoint);
            InitTransformation();
        }

        private void InitTransformation()
        {
            initialScale = grabbedObject.transform.localScale;
            initialRotation = grabbedObject.transform.rotation;

            Vector3 At1 = primaryInitialGrabPoint.position;

            AMt1 = grabbedObject.transform.position - At1;
            ABt1 = secondaryInitialGrabPoint.position - At1;
            ABt1Magnitude = ABt1.magnitude;
        }

        public override void ResetAction()
        {
            Destroy(secondaryInitialGrabPoint.gameObject);
            Destroy(primaryInitialGrabPoint.gameObject);
            base.ResetAction();
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public override void ProcessUpdate()
        {
            if (initialised)
            {
                Vector3 At2 = primaryInitialGrabPoint.position;
                Vector3 Bt2 = secondaryInitialGrabPoint.position;
                Vector3 ABt2 = Bt2 - At2;
                Debug.DrawLine(At2, Bt2);

                // scale
                float scaleFactor = ABt2.magnitude / ABt1Magnitude;
                grabbedObject.transform.localScale = initialScale * scaleFactor;
                // rotation
                Quaternion rotationDifference = Quaternion.FromToRotation(ABt1, ABt2);
                grabbedObject.transform.rotation = rotationDifference * initialRotation;
                // tranformation
                grabbedObject.transform.position = At2 + ((rotationDifference * AMt1) * scaleFactor);
            }
        }
    }

}