using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScaleToScreen : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null) return;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        Vector3 scale = transform.localScale;

        // Skaluj na podstawie wysokości, resztę proporcjonalnie
        float scaleFactor = worldScreenHeight / spriteHeight;
        scale.x = scaleFactor;
        scale.y = scaleFactor;

        transform.localScale = scale;
    }
}
