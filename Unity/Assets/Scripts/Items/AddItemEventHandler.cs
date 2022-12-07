using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddItemEventHandler : MonoBehaviour
{
	public void OnAddItemMenuCancelButtonPressed()
	{
		ItemSpawner.Instance.OnAddItemMenuCancelButtonPressed();
	}

	public void OnAddItemMenuAddButtonPressed()
	{
		ItemSpawner.Instance.OnAddItemMenuAddButtonPressed();
	}
}