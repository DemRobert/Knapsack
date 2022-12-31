using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItemSelectEventHandler : MonoBehaviour
{
	public GameObject AddNewItem;

	private void Start()
	{
		if (GameManager.Instance.GameMode == GameModes.PRACTICE)
		{
			Destroy(AddNewItem);
		}
	}

	public void OnAddNewItemButtonPressed()
    {
		ItemSpawner.Instance.OnAddNewItemButtonPressed();
	}

	public void OnAddInventoryItemButtonPressed()
	{
		ItemSpawner.Instance.OnAddInventoryItemButtonPressed();
	}

	public void OnCancelButtonPressed()
	{
		Destroy(gameObject);
	}
}
