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
    private GameObjectListButton buttonPrefab;

    private Dictionary<string, GameObjectListButton> buttons = new Dictionary<string, GameObjectListButton>();
    private GameObjectListButton placeholderButton;

    private bool isPlaceholderButtonUsed = false;

	private void Awake () {
        autoLoader.OnGameObjectLoaded += GameObjectLoadedHandler;

        autoLoader.OnPreAssetLoad += AssetPreLoadedHandler;


        autoLoader.OnAssetLoadProgress += AssetProgressHandler;

        placeholderButton = Instantiate<GameObject>(buttonPrefab.gameObject).GetComponent<GameObjectListButton>();
        placeholderButton.SetEnabled(false);
        placeholderButton.setText("No Objects yet");
        placeholderButton.transform.SetParent(transform, false);
        placeholderButton.transform.SetAsLastSibling();
        isPlaceholderButtonUsed = true;
    }

    private void AssetPreLoadedHandler(string assetBundleName, string assetName)
    {
        GameObjectListButton newButton = Instantiate<GameObject>(buttonPrefab.gameObject).GetComponent<GameObjectListButton>();
        newButton.SetEnabled(false);
        newButton.setText("Loading...");
        newButton.transform.SetParent(transform, false);
        newButton.transform.SetAsLastSibling();
        Debug.Log("Adding asset " + assetBundleName + assetName);
        buttons.Add(assetBundleName + assetName,  newButton);
    }

    private void AssetProgressHandler(string assetBundleName, string assetName, float progress)
    {
        GameObjectListButton button;
        if (buttons.TryGetValue(assetBundleName + assetName, out button)) {
            button.SetProgress(progress);
        }

        Debug.Log("Progress: " + progress + "  Bundle: " + assetBundleName + "  Asset: " + assetName);
    }

    private void GameObjectLoadedHandler(string assetBundleName, string assetName, GameObject go)
    {
        if (isPlaceholderButtonUsed)
        {
            placeholderButton.gameObject.SetActive(false);
            isPlaceholderButtonUsed = false;
        }

        GameObjectListButton button;
        if (buttons.TryGetValue(assetBundleName + assetName, out button))
        {
            button.setText(go.name);
            button.SetEnabled(true);
            // bind game object to the button via a closure
            button.AddListener(() =>
            {
                if (OnSelected != null)
                {
                    OnSelected(go);
                }
            });
        } else
        {
            Debug.LogError("Loaded game object has not entry in the button list. This should not happen: " + assetBundleName + assetName);
        }
    }
}
