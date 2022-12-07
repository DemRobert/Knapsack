using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHUDController : MonoBehaviour
{
    public static PlayerHUDController Instance;

    public TextMeshProUGUI KnapsackCapacityText;

    private int m_KnapsackCapacity = 0;
    private int m_OldKnapsackCapacity = 0;

	private bool m_IsEnteringNumber;
	private string m_CurrentlyTypedString = "";

	private void Awake()
	{
        Instance = this;
	}

	private void Start()
	{
		SetKnapsackCapacity(1);
	}

	private void Update()
	{
		var eventSystem = GameManager.Instance.EventSystem;

		if (Input.GetMouseButtonDown(0))
		{
			var pointerEventData = new PointerEventData(eventSystem);
			pointerEventData.position = Input.mousePosition;

			var uiRaycastResults = new List<RaycastResult>();
			eventSystem.RaycastAll(pointerEventData, uiRaycastResults);

			if (uiRaycastResults.Count > 0)
			{
				// We assume only one Result is valid (the 1st one)
				var raycastResult = uiRaycastResults[0];
				var raycastGameObject = raycastResult.gameObject;

				if (raycastGameObject.name.Equals("Image"))
				{
					var imageComponent = raycastGameObject.GetComponent<Image>();
					// The associated SpawnPoint of the Image/Canvas (parent.parent)
					var spawnPoint = raycastGameObject.transform.parent.parent;

					switch (imageComponent.sprite.name)
					{
						case "RemoveItem":
							ItemSpawner.Instance.RemoveItem(spawnPoint);
							imageComponent.sprite = ItemSpawner.Instance.AddItemImage;

							break;

						case "AddItem":
							ItemSpawner.Instance.AddItem(spawnPoint);

							break;

						default:
							Debug.Log("PlayerMovement: UI Raycast; Name of Sprite of ImageComponent is not recognized!");

							break;
					}
				}
				else if (GameManager.Instance.GameMode == GameManager.GameModes.LEARNING &&
					raycastGameObject.name.Equals("Text_KnapsackCapacity"))
				{
					m_OldKnapsackCapacity = m_KnapsackCapacity;
					m_IsEnteringNumber = true;

					SetKnapsackCapacityText("ENTER NUMBER");
				}
			}
		}

		if (m_IsEnteringNumber)
		{
			var input = Input.inputString;
			if (input != null && input.Length != 0)
			{
				var curTyped = input[0];
				// If a Digit is typed
				if (curTyped >= 48 && curTyped <= 57)
				{
					m_CurrentlyTypedString += curTyped;
					SetKnapsackCapacityText(m_CurrentlyTypedString);
				}
				// If the Delete Button was pressed
				else if (Input.GetKey(KeyCode.Backspace))
				{
					if (m_CurrentlyTypedString.Length > 0)
					{
						m_CurrentlyTypedString = m_CurrentlyTypedString.Remove(m_CurrentlyTypedString.Length - 1);
						SetKnapsackCapacityText(m_CurrentlyTypedString);
					}
				}
			}
		}

		if (m_IsEnteringNumber)
		{
			var anyButtonPressed = false;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SetKnapsackCapacity(m_OldKnapsackCapacity);
				anyButtonPressed = true;
			}
			else if (Input.GetKeyDown(KeyCode.Return))
			{
				if (m_CurrentlyTypedString.Length == 0)
				{
					SetKnapsackCapacity(0);
				}
				else
				{
					m_KnapsackCapacity = int.Parse(m_CurrentlyTypedString);
				}

				anyButtonPressed = true;
			}

			if (anyButtonPressed)
			{
				m_IsEnteringNumber = false;
				m_CurrentlyTypedString = "";
			}
		}
	}

	public void SetKnapsackCapacity(int capacity)
    {
        m_KnapsackCapacity = capacity;
		SetKnapsackCapacityText(capacity.ToString());
	}

	private void SetKnapsackCapacityText(string text)
	{
		KnapsackCapacityText.text = "Rucksackkapazität: " + text;
	}


	public int GetKnapsackCapacity()
    {
        return m_KnapsackCapacity;
    }
}
