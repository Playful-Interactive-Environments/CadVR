using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameObjectListButton : MonoBehaviour {

    [SerializeField]
    private Image progressImage;
    [SerializeField]
    private Text label;
    [SerializeField]
    Button button;

    private float progress = 0.0f;
    private Coroutine uiLoadLerp = null;

    private void Awake()
    {
        progressImage.fillAmount = 0.0f;
        progressImage.type = Image.Type.Filled;
        progressImage.fillMethod = Image.FillMethod.Radial360; 
    }

    public void SetProgress(float progress) {
        if (progress <= 1.0f && progress >= 0.0f)
        {
            this.progress = progress;
        }

        if (uiLoadLerp != null)
        {
            StopCoroutine(uiLoadLerp);
        }

        StartCoroutine(UiLoadLerp());
    }

    private IEnumerator UiLoadLerp()
    {
        do
        {
            progressImage.fillAmount = Mathf.MoveTowards(progressImage.fillAmount, progress, 1 * Time.deltaTime);
            yield return null;
        } while (!Mathf.Approximately(progressImage.fillAmount, progress));
        progressImage.fillAmount = progress;
    }

    public void setText(string text)
    {
        label.text = text;
    }

    public void SetEnabled(bool enabled)
    {
        button.interactable = enabled;
        progressImage.enabled = !enabled;
    }

    public void AddListener(UnityAction  listener)
    {
        button.onClick.AddListener(listener);
    }

}
