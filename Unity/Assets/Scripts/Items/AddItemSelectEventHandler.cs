using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItemSelectEventHandler : MonoBehaviour
{
    public void OnAddNewItemButtonPressed()
    {
		ItemSpawner.Instance.OnAddNewItemButtonPressed();
	}

	public void OnAddInventoryItemButtonPressed()
	{
		ItemSpawner.Instance.OnAddInventoryItemButtonPressed();
	}
}
