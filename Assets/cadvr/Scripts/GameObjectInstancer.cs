using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.GrabAttachMechanics;
using VRTK.Highlighters;
using VRTK.SecondaryControllerGrabActions;

public class GameObjectInstancer : MonoBehaviour {

    [SerializeField]
    private GameObjectList objectSource;
    [SerializeField]
    private Material highlighterMaterial;

    private List<GameObject> instances;

    void Awake()
    {
        objectSource.OnSelected += ObjectSelectedHandler;
    }

    private void ObjectSelectedHandler(GameObject selected)
    {
        GameObject instance = Instantiate<GameObject>(selected);
        VRTK_InteractableObject interactible = instance.AddComponent<VRTK_InteractableObject>();
        VRTK_ChildOfControllerGrabAttach primary = instance.AddComponent<VRTK_ChildOfControllerGrabAttach>();
        interactible.grabAttachMechanicScript = primary;
        primary.precisionGrab = true;
        VRTK_PrecisionScaleAction secondary = instance.AddComponent<VRTK_PrecisionScaleAction>();
        interactible.secondaryGrabActionScript = secondary;
        VRTK_MaterialColorSwapHighlighter highlighter = selected.AddComponent<VRTK_MaterialColorSwapHighlighter>();
        highlighter.customMaterial = highlighterMaterial;

        instance.transform.localScale = Vector3.one * 0.01f;
        instance.transform.position = transform.position;
        instance.transform.rotation = transform.rotation;

    }
}
