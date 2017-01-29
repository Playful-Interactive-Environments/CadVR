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
        private Vector3 initialScale;
        private Quaternion initialRotation;
        private Vector3 AMt1;
        private Vector3 ABt1;
        private float ABt1Magnitude;
        private Vector3 Mt1;
        private Vector3 At1;
        private Vector3 Bt1;


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
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            initialScale = grabbedObject.transform.localScale;
            initialRotation = grabbedObject.transform.rotation;
            // the pivot of the grabbed object
            Mt1 = grabbedObject.transform.position;
            // the initial grab position of first controller
            At1 = primaryInitialGrabPoint.position;
            Bt1 = secondaryInitialGrabPoint.position;

            AMt1 = Mt1 - At1;
            ABt1 = Bt1 - At1;
            ABt1Magnitude = ABt1.magnitude;
    }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public override void ProcessUpdate()
        {
            Vector3 ABt2 = secondaryInitialGrabPoint.position - primaryInitialGrabPoint.position;

            // scale
            float scaleFactor = ABt2.magnitude / ABt1Magnitude;
            Vector3 newScale = initialScale * scaleFactor;
            grabbedObject.transform.localScale = newScale;
            // rotation
            Quaternion rotationDifference = Quaternion.FromToRotation(ABt1, ABt2);
            grabbedObject.transform.rotation = initialRotation * rotationDifference;
            // tranformation
            grabbedObject.transform.position = At1 + ((rotationDifference * AMt1) * scaleFactor);
        }
    }
}