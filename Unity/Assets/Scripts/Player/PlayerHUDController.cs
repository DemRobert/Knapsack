using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerHUDController : MonoBehaviour
{
    public static PlayerHUDController Instance;

    public TextMeshProUGUI KnapsackCapacityText;

	public GameObject PauseMenu;
	private bool m_Paused;

    private int m_KnapsackCapacity = 0;
    private int m_OldKnapsackCapacity = 0;

	private bool m_IsEnteringNumber;
	private string m_CurrentlyTypedString = "";

	private Inventory m_PlayerInventory;

	public LayerMask PhysicsRayCastLayer;
	private bool m_IsCollidingWithItem;
	private bool m_HasRecentlyCollidedWithItem;
	private GameObject m_CurOutlinedObject;

	private DynamicProgAlgoBehaviour m_DynamicProgrAlgorithm = new();
	private GreedyAlgoBehaviour m_GreedyAlgorithm = new();

	public GameObject DynamicProgrSolverScreen;
	public GameObject GreedySolverScreen;

	private void Awake()
	{
        Instance = this;
	}

	private void Start()
	{
		if (MainMenuManager.SelectedGameMode == GameModes.PRACTICE)
		{
			SetKnapsackCapacity(Mathf.RoundToInt(Random.Range(4.0f, 10.0f)));
		}
		else
		{
			SetKnapsackCapacity(5);
		}

		if (MainMenuManager.SelectedAlgorithm == AlgoTypes.Dynamic)
		{
			DynamicProgrSolverScreen.SetActive(true);
		}
		else if (MainMenuManager.SelectedAlgorithm == AlgoTypes.Greedy)
		{
			GreedySolverScreen.SetActive(true);
		}

		m_PlayerInventory = transform.parent.GetComponent<Inventory>();
	}

	public void SetPaused(bool paused)
	{
		m_Paused = paused;
	}

	public bool IsPaused()
	{
		return PauseMenu.activeInHierarchy || m_Paused;
	}

	private void Update()
	{
		if (IsPaused())
		{
			return;
		}

		var eventSystem = GameManager.Instance.EventSystem;

		var isCollidingWithUIStuff = false;
		if (Input.GetMouseButtonDown(0))
		{
			var pointerEventData = new PointerEventData(eventSystem);
			pointerEventData.position = Input.mousePosition;

			var uiRaycastResults = new List<RaycastResult>();
			eventSystem.RaycastAll(pointerEventData, uiRaycastResults);

			if (uiRaycastResults.Count > 0)
			{
				isCollidingWithUIStuff = true;

				// We assume only one Result is valid (the 1st one)
				var raycastResult = uiRaycastResults[0];
				var raycastGameObject = raycastResult.gameObject;

				if (raycastGameObject.name.Equals("Image"))
				{
					var imageComponent = raycastGameObject.GetComponent<Image>();
					// The associated SpawnPoint of the Image/Canvas (parent.parent)
					var spawnPoint = ItemSpawner.GetSpawnPoint(raycastGameObject);

					switch (imageComponent.sprite.name)
					{
					case "RemoveItem":
						if (GameManager.Instance.GameMode != GameModes.PRACTICE)
						{
							ItemSpawner.Instance.RemoveItem(spawnPoint.Find("Item").GetChild(0).gameObject);
						}

						break;

					case "AddItem":
						ItemSpawner.Instance.AddItem(spawnPoint);

						break;

					default:
						Debug.Log("PlayerMovement: UI Raycast; Name of Sprite of ImageComponent is not recognized!");

						break;
					}
				}
				else if (GameManager.Instance.GameMode == GameModes.LEARNING &&
					raycastGameObject.name.Equals("Text_KnapsackCapacity"))
				{
					m_OldKnapsackCapacity = m_KnapsackCapacity;
					m_IsEnteringNumber = true;

					SetKnapsackCapacityText("--");
				}
				else if (raycastGameObject.CompareTag("ItemInHUD"))
				{
					if (ItemSpawner.Instance.IsSelectingHUDItem &&
						Input.GetMouseButtonDown(0))
					{
						ItemSpawner.Instance.OnHUDItemSelected(raycastGameObject.transform.parent.GetComponent<ItemInHUDController>().HUDItem);
					}
				}
			}
		}

		RaycastHit raycastSceneHit;
		if (!isCollidingWithUIStuff && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastSceneHit, 100.0f, PhysicsRayCastLayer))
		{
			// Outlining the Hovered Items
			if (raycastSceneHit.collider.CompareTag("Item"))
			{
				var colliderGameObject = raycastSceneHit.collider.gameObject;

				var outline = GetComponentFromParent<Outline>(colliderGameObject.transform);
				if (outline != null)
				{
					outline.OutlineColor = Color.white;
					// If we hovered onto another Item without touchin any other Object in the Scene,
					// we have to manually disable the Outline of the "old" Item which was highlighted
					if (m_CurOutlinedObject != null && m_CurOutlinedObject != colliderGameObject)
					{
						var outlineToDisable = GetComponentFromParent<Outline>(m_CurOutlinedObject.transform);
						if (outlineToDisable != null)
						{
							outlineToDisable.OutlineColor = Color.clear;
						}
					}

					m_CurOutlinedObject = colliderGameObject;
				}

				if (Input.GetMouseButtonDown(0))
				{
					var hitObjectGameObject = raycastSceneHit.collider.gameObject;

					var itemPrefabScript = colliderGameObject.GetComponent<ItemPrefab>();
					var itemPropertiesScript = GetComponentFromParent<ItemProperties>(colliderGameObject.transform);

					// Check if our Knapsack has enough Space for the new Item
					if (Inventory.Instance.GetTotalWeight() + itemPropertiesScript.weight <= m_KnapsackCapacity)
					{
						m_PlayerInventory.AddItem(new HUDItem(itemPrefabScript.Prefab, new(itemPropertiesScript), itemPrefabScript.ObjectAsSprite));

						var spawnPoint = ItemSpawner.GetSpawnPoint(hitObjectGameObject);
						ItemSpawner.Instance.RemoveItem(spawnPoint.Find("Item").GetChild(0).gameObject);
					}
				}

				m_IsCollidingWithItem = true;
				m_HasRecentlyCollidedWithItem = true;
			}
			else
			{
				m_IsCollidingWithItem = false;
				m_CurOutlinedObject = null;
			}
		}
		else
		{
			m_IsCollidingWithItem = false;
			m_CurOutlinedObject = null;
		}

		if (m_HasRecentlyCollidedWithItem && !m_IsCollidingWithItem)
		{
			// Reset the Outlines of all Items
			var items = ItemSpawner.Instance.GetItems();
			foreach (var item in items)
			{
				var outline = GetComponentFromParent<Outline>(item.transform);
				if (outline != null)
				{
					outline.OutlineColor = Color.clear;
				}
			}

			m_HasRecentlyCollidedWithItem = false;
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
					// At most 3 Digits -> 99 = highest available
					if (m_CurrentlyTypedString.Length < 2)
					{
						m_CurrentlyTypedString += curTyped;
						SetKnapsackCapacityText(m_CurrentlyTypedString);
					}
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

		if (!m_IsEnteringNumber && Input.GetKeyDown(KeyCode.Escape))
		{
			if (PauseMenu.activeInHierarchy)
			{
				PauseMenu.SetActive(false);
			}
			else
			{
				PauseMenu.SetActive(true);
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
					SetKnapsackCapacity(m_OldKnapsackCapacity);
				}
				else
				{
					SetKnapsackCapacity(Mathf.Min(int.Parse(m_CurrentlyTypedString), 25));
				}

				anyButtonPressed = true;
			}

			if (anyButtonPressed)
			{
				m_IsEnteringNumber = false;
				m_CurrentlyTypedString = "";
			}
		}

		if (Input.GetKeyDown(KeyCode.F))
		{
			m_PlayerInventory.Maximized = !m_PlayerInventory.Maximized;
			m_PlayerInventory.UpdateInventory();
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			//GameManager.Instance.AlgoType = AlgoTypes.Dynamic;

			if (GameManager.Instance.AlgoType == AlgoTypes.Dynamic)
			{
                m_DynamicProgrAlgorithm.StartAlgorithm(ItemSpawner.Instance.GetItemProperties(), GetKnapsackCapacity(), out var selectedItems, out var steps, out int totalValue, out var totalWeight);
				DynamicProgrammingSolver.Instance.Solve(m_DynamicProgrAlgorithm);
            }	
			else if (GameManager.Instance.AlgoType == AlgoTypes.Greedy)
			{
				m_GreedyAlgorithm.StartAlgorithm(ItemSpawner.Instance.GetItemProperties(), GetKnapsackCapacity(), out var selectedItems, out var steps, out int totalValue, out var totalWeight);
				GreedySolver.Instance.Solve(m_GreedyAlgorithm);
			}
		}
	}

	// Will search for the specified Component in
	// the parent as well as in all of his Children.
	// Returns null if the Component could not be found.
	public static T GetComponentFromParent<T>(Transform parent)
	{
		if (parent.TryGetComponent<T>(out var result))
		{
			return result;
		}

		for (var i = 0; i < parent.childCount; ++i)
		{
			var childResult = GetComponentFromParent<T>(parent.GetChild(i));
			if (childResult != null)
			{
				return childResult;
			}
		}
		
		// null for Generics because T could be non-nullable
		return default;
	}

	public void OnPauseMenuContinueButtonPressed()
	{
		PauseMenu.SetActive(false);
	}

	public void OnPauseMenuQuitButtonPressed()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void SetKnapsackCapacity(int capacity)
    {
        m_KnapsackCapacity = capacity;
		SetKnapsackCapacityText(capacity.ToString());
	}

	private void SetKnapsackCapacityText(string text)
	{
		KnapsackCapacityText.text = text;
	}

	public int GetKnapsackCapacity()
    {
        return m_KnapsackCapacity;
    }
}
