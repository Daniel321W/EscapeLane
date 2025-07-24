using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public Sprite[] mapSprites;

    void Start()
    {
        string selectedMap = PlayerPrefs.GetString("SelectedMap", "level1");

        foreach (var sprite in mapSprites)
        {
            if (sprite.name == selectedMap)
            {
                backgroundRenderer.sprite = sprite;
                break;
            }
        }
    }
}
