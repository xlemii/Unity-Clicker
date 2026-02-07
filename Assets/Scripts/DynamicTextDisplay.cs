using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DynamicTextDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage; 
    [SerializeField] private TextMeshProUGUI textField;

    [Header("Teksty do wyświetlenia")]
    [TextArea(2, 5)] public string[] possibleTexts;     

    [Header("Ustawienia")]
    public bool showOnStart = false;                
    public float displayTime = 3f;               


    private void Start()
    {
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        if (showOnStart)
            ShowRandomText();
    }

    public void ShowRandomText()
    {
        if (possibleTexts.Length == 0 || backgroundImage == null || textField == null)
            return;

        string chosenText = possibleTexts[Random.Range(0, possibleTexts.Length)];
        textField.text = chosenText;

        backgroundImage.gameObject.SetActive(true);

        CancelInvoke(nameof(HideText));
        Invoke(nameof(HideText), displayTime);
    }

    public void ShowSpecificText(int index)
    {
        if (index < 0 || index >= possibleTexts.Length || backgroundImage == null || textField == null)
            return;

        textField.text = possibleTexts[index];
        backgroundImage.gameObject.SetActive(true);

        CancelInvoke(nameof(HideText));
        Invoke(nameof(HideText), displayTime);
    }

    public void HideText()
    {
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);
    }
}
