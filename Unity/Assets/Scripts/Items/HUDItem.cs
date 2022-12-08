using UnityEngine;

public class HUDItem
{
    public GameObject Prefab;
    public ItemProperties ItemProperties;
    public Sprite ItemSprite;

    public HUDItem(GameObject prefab, ItemProperties itemProperties, Sprite itemSprite)
    {
        Prefab = prefab;
		ItemProperties = itemProperties;
		ItemSprite = itemSprite;
    }
}
