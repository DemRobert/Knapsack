using UnityEngine;

public class HUDItem
{
    public GameObject Prefab;
    public ItemPropertiesNoUnity ItemProperties;
    public Sprite ItemSprite;

    public HUDItem(GameObject prefab, ItemPropertiesNoUnity itemProperties, Sprite itemSprite)
    {
        Prefab = prefab;
		ItemProperties = itemProperties;
		ItemSprite = itemSprite;
    }
}
