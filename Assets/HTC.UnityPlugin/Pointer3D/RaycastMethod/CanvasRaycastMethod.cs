//========= Copyright 2016, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HTC.UnityPlugin.Pointer3D
{
    [AddComponentMenu("HTC/Pointer3D/Canvas Raycast Method")]
    [DisallowMultipleComponent]
    public class CanvasRaycastMethod : BaseRaycastMethod
    {
        private static readonly IndexedSet<ICanvasRaycastTarget> canvases = new IndexedSet<ICanvasRaycastTarget>();

        public static bool AddTarget(ICanvasRaycastTarget obj) { return obj == null ? false : canvases.AddUnique(obj); }

        public static bool RemoveTarget(ICanvasRaycastTarget obj) { return obj == null ? false : canvases.Remove(obj); }

        public override void Raycast(BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults)
        {
            var tempCanvases = ListPool<ICanvasRaycastTarget>.Get();
            tempCanvases.AddRange(canvases);
            for (int i = tempCanvases.Count - 1; i >= 0; --i)
            {
                var target = tempCanvases[i];
                if (target == null || !target.enabled) { continue; }
                Raycast(target.canvas, target.ignoreReversedGraphics, module, position, eventCamera, raycastResults);
            }
            ListPool<ICanvasRaycastTarget>.Release(tempCanvases);
        }

        public static void Raycast(Canvas canvas, bool ignoreReversedGraphics, BaseRaycaster module, Vector2 position, Camera eventCamera, List<RaycastResult> raycastResults)
        {
            if (canvas == null) { return; }

            var ray = eventCamera.ScreenPointToRay(position);
            var distance = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget) { continue; }

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, position, eventCamera)) { continue; }

                if (ignoreReversedGraphics && Vector3.Dot(eventCamera.transform.forward, graphic.transform.forward) <= 0f) { continue; }

                if (!graphic.Raycast(position, eventCamera)) { continue; }

                float dist;
                new Plane(graphic.transform.forward, graphic.transform.position).Raycast(ray, out dist);
                if (dist > distance) { continue; }

                raycastResults.Add(new RaycastResult
                {
                    gameObject = graphic.gameObject,
                    module = module,
                    distance = dist,
                    worldPosition = ray.GetPoint(dist),
                    worldNormal = graphic.transform.forward,
                    screenPosition = position,
                    index = raycastResults.Count,
                    depth = graphic.depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder
                });
            }
        }
    }
}