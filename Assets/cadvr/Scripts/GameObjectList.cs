using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
[AddComponentMenu("Cad Vr/Ui/ObjectList")]
public class GameObjectList : MonoBehaviour {

    public delegate void OnSelectedDelegate(GameObject selectedGameObject);
    public OnSelectedDelegate OnSelected;

    [SerializeField]
    private AutoLoadAllAvailableBundles autoLoader;
    [SerializeField]
    private Button buttonPrefab;
    private List<GameObject> goList = new List<GameObject>();
    private Button placeholderButton;

    private bool isPlaceholderButtonUsed = false;

	private void Awake () {
        autoLoader.OnGameObjectLoaded += GameObjectLoadedHandler;

        placeholderButton = Instantiate<GameObject>(buttonPrefab.gameObject).GetComponent<Button>();
        placeholderButton.interactable = false;
        Text text = placeholderButton.GetComponentInChildren<Text>();
        if (text)
        {
            text.text = "No Objects yet";
        }

        placeholderButton.transform.SetParent(transform, false);
        placeholderButton.transform.SetAsLastSibling();
        isPlaceholderButtonUsed = true;
    }

    private void GameObjectLoadedHandler(GameObject go)
    {
        if (isPlaceholderButtonUsed)
        {
            placeholderButton.gameObject.SetActive(false);
            isPlaceholderButtonUsed = false;
        }

        Button newButton = Instantiate<GameObject>(buttonPrefab.gameObject).GetComponent<Button>();
        newButton.interactable = true;
        Text text = newButton.GetComponentInChildren<Text>();
        if (text)
        {
            text.text = go.name;
        }

        newButton.transform.SetParent(transform, false);
        newButton.transform.SetAsLastSibling();

        // bind game object to the button via a closure
        newButton.onClick.AddListener(() => {
            if (OnSelected != null)
            {
                OnSelected(go);
            }
        });
    }
}
