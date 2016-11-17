//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Pointer3D;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Vive
{
    // Custom PointerEventData implement for Vive controller.
    public class VivePointerEventData : Pointer3DEventData
    {
        public readonly HandRole hand;
        public readonly ControllerButton viveButton;

        public VivePointerEventData(EventSystem eventSystem, HandRole hand, ControllerButton viveButton, InputButton mouseButton) : base(eventSystem)
        {
            this.hand = hand;
            this.viveButton = viveButton;
            button = mouseButton;
        }

        public override bool GetPress() { return ViveInput.GetPress(hand, viveButton); }

        public override bool GetPressDown() { return ViveInput.GetPressDown(hand, viveButton); }

        public override bool GetPressUp() { return ViveInput.GetPressUp(hand, viveButton); }
    }
}