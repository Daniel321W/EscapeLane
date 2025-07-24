using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScaleSpriteToScreen : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null || Camera.main == null) return;

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        float screenWorldHeight = Camera.main.orthographicSize * 2f;
        float screenWorldWidth = screenWorldHeight * Screen.width / (float)Screen.height;

        // Dopasuj tak, żeby cały sprite był widoczny (bez rozciągania)
        float scaleFactor = Mathf.Min(screenWorldWidth / spriteWidth, screenWorldHeight / spriteHeight);

        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}
