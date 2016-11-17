//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTC.UnityPlugin.Pointer3D
{
    [AddComponentMenu("HTC/Pointer3D/Physics2D Raycast Method")]
    public class Physics2DRaycastMethod : PhysicsRaycastMethod
    {
        private static readonly RaycastHit2D[] hits = new RaycastHit2D[64];

        public override void Raycast(BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults)
        {
            var ray = eventCamera.ScreenPointToRay(position);
            var distance = eventCamera.farClipPlane - eventCamera.nearClipPlane;
            var hitCount = Physics2D.GetRayIntersectionNonAlloc(ray, hits, distance, RaycastMask);

            for (int i = 0; i < hitCount; ++i)
            {
                var sr = hits[i].collider.gameObject.GetComponent<SpriteRenderer>();

                raycastResults.Add(new RaycastResult
                {
                    gameObject = hits[i].collider.gameObject,
                    module = module,
                    distance = Vector3.Distance(eventCamera.transform.position, hits[i].transform.position),
                    worldPosition = hits[i].point,
                    worldNormal = hits[i].normal,
                    screenPosition = position,
                    index = raycastResults.Count,
                    sortingLayer = sr != null ? sr.sortingLayerID : 0,
                    sortingOrder = sr != null ? sr.sortingOrder : 0
                });
            }
        }
    }
}