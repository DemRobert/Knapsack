using UnityEditor;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
    public GameObject Prefab;
    public Sprite ObjectAsSprite;

	public void SetPrefab(GameObject prefab)
	{
		Prefab = prefab;

		//var texture = AssetPreview.GetAssetPreview(prefab);
		//ObjectAsSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2());
	}
}
