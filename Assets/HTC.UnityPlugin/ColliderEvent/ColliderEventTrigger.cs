//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.ColliderEvent
{
    public enum ColliderEventTriggerType
    {
        HoverEnter,
        HoverExit,
        PressDown,
        PressUp,
        PressEnter,
        PressExit,
        Click,
        DragStart,
        Drag,
        DragEnd,
        Drop,
        AxisChanged,
    }

    public class ColliderEventTrigger : MonoBehaviour
        , IColliderEventHoverEnterHandler
        , IColliderEventHoverExitHandler
        , IColliderEventPressDownHandler
        , IColliderEventPressUpHandler
        , IColliderEventPressEnterHandler
        , IColliderEventPressExitHandler
        , IColliderEventClickHandler
        , IColliderEventDragStartHandler
        , IColliderEventDragUpdateHandler
        , IColliderEventDragEndHandler
        , IColliderEventDropHandler
        , IColliderEventAxisChangedHandler
    {
        [Serializable]
        public class Entry
        {
            public ColliderEventTriggerType eventID = ColliderEventTriggerType.HoverEnter;
            public EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
        }

        [SerializeField]
        private List<Entry> m_Delegates;

        public List<Entry> triggers
        {
            get { return m_Delegates ?? (m_Delegates = new List<Entry>()); }
            set { m_Delegates = value; }
        }

        private void Execute(ColliderEventTriggerType id, BaseEventData eventData)
        {
            for (int i = 0, imax = triggers.Count; i < imax; ++i)
            {
                var ent = triggers[i];
                if (ent.eventID == id && ent.callback != null)
                {
                    ent.callback.Invoke(eventData);
                }
            }
        }

        public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
        {
            Execute(ColliderEventTriggerType.HoverEnter, eventData);
        }

        public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
        {
            Execute(ColliderEventTriggerType.HoverExit, eventData);
        }

        public void OnColliderEventPressDown(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.PressDown, eventData);
        }

        public void OnColliderEventPressUp(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.PressUp, eventData);
        }

        public void OnColliderEventPressEnter(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.PressEnter, eventData);
        }

        public void OnColliderEventPressExit(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.PressExit, eventData);
        }

        public void OnColliderEventClick(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.Click, eventData);
        }

        public void OnColliderEventDragStart(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.DragStart, eventData);
        }

        public void OnColliderEventDragUpdate(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.Drag, eventData);
        }

        public void OnColliderEventDragEnd(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.DragEnd, eventData);
        }

        public void OnColliderEventDrop(ColliderButtonEventData eventData)
        {
            Execute(ColliderEventTriggerType.Drop, eventData);
        }

        public void OnColliderEventAxisChanged(ColliderAxisEventData eventData)
        {
            Execute(ColliderEventTriggerType.AxisChanged, eventData);
        }
    }
}