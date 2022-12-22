using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPrefab : MonoBehaviour
{
    public GameObject Prefab;
    public Sprite ObjectAsSprite;

	private readonly Dictionary<GameObject, Sprite> m_PrefabSprites = new();

	public void SetPrefab(GameObject prefab)
	{
		Prefab = prefab;

		if (!m_PrefabSprites.ContainsKey(prefab))
		{
			//var texture = AssetPreview.GetMiniThumbnail(prefab);
			//var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2());

			//m_PrefabSprites.Add(prefab, sprite);
		}

		//ObjectAsSprite = m_PrefabSprites[prefab];
	}
}
