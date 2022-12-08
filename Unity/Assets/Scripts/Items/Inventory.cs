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
        var xOffsetToNextItem = 60;
        var yOffsetToNextItem = 50;

        var startY = AssociatedCanvas.renderingDisplaySize.y - 30;

		// 2 Items per Row
		for (var i = 0; i < m_Items.Count; ++i)
        {
			var curX = 20 + (i % 2)*xOffsetToNextItem;
			var curY = startY - (i/2)*yOffsetToNextItem;

            var curHUDItem = Instantiate(ItemInHUDPrefab, new Vector2(curX, curY), Quaternion.identity, ItemsInHUDParent);
            var curItemTextWeight = curHUDItem.transform.Find("Text_Weight").GetComponent<TextMeshProUGUI>();
            var curItemTextValue = curHUDItem.transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();

            var curItem = m_Items[i];
            curItemTextWeight.text = curItem.ItemProperties.weight.ToString();
			curItemTextValue.text = curItem.ItemProperties.value.ToString();

            var itemImage = curHUDItem.transform.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = curItem.ItemSprite;

            var curHUDItemHUDController = curHUDItem.GetComponent<ItemInHUDController>();
            curHUDItemHUDController.HUDItem = curItem;
		}
    }

    private void ClearInventory()
    {
        for (var i = ItemsInHUDParent.childCount-1; i >= 0; --i)
        {
			Destroy(ItemsInHUDParent.GetChild(i).gameObject);
		}
    }
}
