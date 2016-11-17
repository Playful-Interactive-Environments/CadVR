//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    public class Pointer3DRaycaster : BaseFallbackCamRaycaster
    {
        [NonSerialized]
        private Pointer3DEventData hoverEventData;
        [NonSerialized]
        private ReadOnlyCollection<Pointer3DEventData> buttonEventDataListReadOnly;

        [NonSerialized]
        protected readonly List<Pointer3DEventData> buttonEventDataList = new List<Pointer3DEventData>();

        public Pointer3DEventData HoverEventData
        {
            get
            {
                if (hoverEventData == null)
                {
                    hoverEventData = new Pointer3DEventData(EventSystem.current);
                }
                return hoverEventData;
            }
        }

        public ReadOnlyCollection<Pointer3DEventData> ButtonEventDataList
        {
            get
            {
                if (buttonEventDataListReadOnly == null)
                {
                    buttonEventDataListReadOnly = buttonEventDataList.AsReadOnly();
                }
                return buttonEventDataListReadOnly;
            }
        }

        public virtual Vector2 GetScrollDelta() { return Vector2.zero; }

        protected override void Awake()
        {
            base.Awake();
            Pointer3DInputModule.AddRaycaster(this);
        }

        // override OnEnable & OnDisable on purpose so that this BaseRaycaster won't be registered into RaycasterManager
        protected override void OnEnable()
        {
            //base.OnEnable();
        }

        protected override void OnDisable()
        {
            //base.OnDisable();
        }

        protected override void OnDestroy()
        {
            Pointer3DInputModule.RemoveRaycasters(this);
            base.OnDestroy();
        }
    }
}