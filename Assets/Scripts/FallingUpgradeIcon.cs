using UnityEngine;
using UnityEngine.UI;

public class FallingUpgradeIcon : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 velocity;
    private float gravity;
    private float bounceFactor;
    private float floorY = -400;
    private Image image;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        gravity = Random.Range(-280f, -320f);
        bounceFactor = Random.Range(0.6f, 0.85f);

        velocity = new Vector2(Random.Range(-60f, 60f), Random.Range(-120f, -60f));
    }

    public void SetSprite(Sprite sprite)
    {
        if (image != null)
            image.sprite = sprite;
    }

    private void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        rect.anchoredPosition += velocity * Time.deltaTime;

        if (rect.anchoredPosition.y < floorY)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, floorY);
            velocity.y *= -bounceFactor;

            if (Mathf.Abs(velocity.y) < 30f)
                Destroy(gameObject, 0.5f); 
        }
    }
}
