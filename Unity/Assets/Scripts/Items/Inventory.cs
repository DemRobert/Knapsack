using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private readonly List<HUDItem> m_Items = new();
    private int m_TotalWeight = 0;
    private int m_TotalValue = 0;

    public GameObject ItemInHUDPrefab;
    public Canvas AssociatedCanvas;
    public Transform ItemsInHUDParent;

    public bool Maximized = false;
    private float m_MaximizedInventoryScale = 2.0f;
    private Vector2 m_ItemInHUDPrefabImageSize;

	private void Start()
	{
		m_ItemInHUDPrefabImageSize = ItemInHUDPrefab.transform.Find("ItemImage").GetComponent<RectTransform>().rect.size;
        m_ItemInHUDPrefabImageSize *= ItemInHUDPrefab.transform.localScale;
	}

	public List<HUDItem> GetItems()
    {
        return m_Items;
    }

    public int GetTotalWeight()
    {
        return m_TotalWeight;
    }

	public int GetTotalValue()
	{
		return m_TotalValue;
	}

	public void AddItem(HUDItem item)
    {
        m_Items.Add(item);

        var itemProperties = item.ItemProperties;
        m_TotalWeight += itemProperties.weight;
        m_TotalValue += itemProperties.value;

		ClearInventory();
		ShowItemsOnScreen();
	}

	public void RemoveItem(HUDItem item)
	{
        m_Items.Remove(item);

		var itemProperties = item.ItemProperties;
		m_TotalWeight -= itemProperties.weight;
		m_TotalValue -= itemProperties.value;

        ClearInventory();
        ShowItemsOnScreen();
	}

    public void ShowItemsOnScreen()
    {
        var displaySize = AssociatedCanvas.renderingDisplaySize;
        var aspectRatio = displaySize.x / displaySize.y;

		var xOffsetToNextItem = displaySize.x * 0.06f;
        var yOffsetToNextItem = (displaySize.y*0.06f) / (1.0f / aspectRatio);

        var startX = displaySize.x * 0.03f;
        var startY = displaySize.y * 0.95f;

        if (Maximized)
        {
            xOffsetToNextItem *= m_MaximizedInventoryScale;
			yOffsetToNextItem *= m_MaximizedInventoryScale;

            startX += displaySize.x*0.5f - m_ItemInHUDPrefabImageSize.x - xOffsetToNextItem*0.25f;
			startY *= 0.85f;
		}

		// 2 Items per Row
		for (var i = 0; i < m_Items.Count; ++i)
        {
			var curX = startX + (i % 2)*xOffsetToNextItem;
			var curY = startY - (i / 2)*yOffsetToNextItem;

            var curHUDItem = Instantiate(ItemInHUDPrefab, new Vector2(curX, curY), Quaternion.identity, ItemsInHUDParent);
            var curItemTextWeight = curHUDItem.transform.Find("Text_Weight").GetComponent<TextMeshProUGUI>();
            var curItemTextValue = curHUDItem.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

            if (Maximized)
            {
                curHUDItem.transform.localScale *= m_MaximizedInventoryScale;
			}

			var curItem = m_Items[i];
            curItemTextWeight.text = curItem.ItemProperties.weight.ToString();
			curItemTextValue.text = curItem.ItemProperties.value.ToString();

            var itemImage = curHUDItem.transform.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = curItem.ItemSprite;

            var curHUDItemHUDController = curHUDItem.GetComponent<ItemInHUDController>();
            curHUDItemHUDController.HUDItem = curItem;
		}
    }

    public void UpdateInventory()
    {
        ClearInventory();
		ShowItemsOnScreen();
	}

    private void ClearInventory()
    {
        for (var i = ItemsInHUDParent.childCount-1; i >= 0; --i)
        {
			Destroy(ItemsInHUDParent.GetChild(i).gameObject);
		}
    }
}
