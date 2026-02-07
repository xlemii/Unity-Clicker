using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 50f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private TextMeshProUGUI textMesh;

    private RectTransform rect;
    private Color startColor;
    private float elapsed = 0f;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

        startColor = new Color(1f, 0.4f, 0.7f);
        if (textMesh != null)
            textMesh.color = startColor;
    }

    public void SetText(string text)
    {
        if (textMesh != null)
            textMesh.text = text;
    }

    public void SetColor(Color c)
    {
        if (textMesh != null)
        {
            c = new Color(1f, 0.4f, 0.7f);
            textMesh.color = c;
            startColor = c;
        }
    }

    private void Update()
    {
        if (rect != null)
            rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
        else
            transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, t);

        if (textMesh != null)
            textMesh.color = c;

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}
