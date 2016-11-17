using HTC.UnityPlugin.Vive;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Teleportable : MonoBehaviour, IPointerClickHandler
{
    public Transform target;
    public Transform pivot;
    public float fadeDuration = 0.3f;

    private Coroutine teleportCoroutine;

    public void OnPointerClick(PointerEventData eventData)
    {
        var viveEventData = eventData as VivePointerEventData;
        if (viveEventData != null && viveEventData.viveButton == ControllerButton.Pad)
        {
            var headVector = Vector3.ProjectOnPlane(pivot.position - target.position, target.up);
            var targetPos = viveEventData.pointerCurrentRaycast.worldPosition - headVector;

            if (fadeDuration <= 0)
            {
                target.position = targetPos;
            }
            else
            {
                if (teleportCoroutine == null)
                {
                    teleportCoroutine = StartCoroutine(StartTeleport(targetPos, fadeDuration));
                }
            }
        }
    }

    public IEnumerator StartTeleport(Vector3 position, float duration)
    {
        var halfDuration = Mathf.Max(0f, duration * 0.5f);

        if (!Mathf.Approximately(halfDuration, 0f))
        {
            SteamVR_Fade.Start(new Color(0f, 0f, 0f, 1f), halfDuration);
            yield return new WaitForSeconds(halfDuration);
            target.position = position;
            SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0f), halfDuration);
            yield return new WaitForSeconds(halfDuration);
        }
        else
        {
            target.position = position;
        }

        teleportCoroutine = null;
    }
}
