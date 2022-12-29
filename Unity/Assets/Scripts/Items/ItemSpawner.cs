using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public List<Item> ItemPrefabs;
    private readonly List<GameObject> m_Items = new();

    // If set to false, the PrioritySpawnPoints will be ignored and
    // Items will just spawn at the Spawn Points of SpawnPoints in order
    public bool ConsiderPrioritySpawnPoints = true;
    public int CountOfItemsToSpawn = 5;

    // A List of all possible SpawnPoints
    // The SpawnPoints will be determined automatically by their Tag. If you choose to
    // set some of the Spawn Points manually, they will be kept and not be overwritten
    public List<GameObject> SpawnPoints = new();
    // A List of SpawnPoints, which are preferred for spawning
    // Items (Items will spawn first at these SpawnPoints)
    public List<GameObject> PrioritySpawnPoints = new();

    public GameObject SignPrefab;
	public Transform SignParentObject;

	private readonly List<GameObject> m_UsedSpawnPoints = new();

    public Sprite AddItemImage;
    public Sprite RemoveItemImage;

    private static int s_CurId = 0;
    private readonly Queue<int> m_FreedIds = new();

    public GameObject AddItemMenu;
    public GameObject AddItemSelectMenu;

    private static GameObject s_ActiveAddItemMenu;
    private static GameObject s_ActiveAddItemSelectMenu;
    private static Transform s_CurrentAddItemMenuSpawnPoint;

    private GameObject m_Player;
	[HideInInspector] public bool IsSelectingHUDItem;

    private void Awake()
    {
        Instance = this;
    }

    public List<GameObject> GetItems()
    {
        return m_Items;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_Player = GameObject.FindGameObjectsWithTag("Player")[0];

		CollectSpawnPoints();
        InitializeSpawnPoints();

		SpawnItems();
        SpawnSigns();

        //DynamicProgrammingSolver.Instance.Solve();
    }

    private void CollectSpawnPoints()
    {
        var itemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawnPoint");

        for (var i = 0; i < itemSpawnPoints.Length; ++i)
        {
            if (SpawnPoints.Count > i)
            {
                if (SpawnPoints[i] == null)
                {
                    SpawnPoints[i] = itemSpawnPoints[i];
                }
            }
            else
            {
                SpawnPoints.Add(itemSpawnPoints[i]);
            }
        }
    }

    private void InitializeSpawnPoints()
    {
        foreach (var spawnPoint in SpawnPoints)
        {
            var canvas = spawnPoint.transform.Find("Canvas");
			canvas.Find("Image").GetComponent<Image>().sprite = AddItemImage;
            canvas.GetComponent<Canvas>().worldCamera = Camera.main;
		}
    }

    private void SpawnItems()
    {
        List<GameObject> spawnPoints = null;
        if (ConsiderPrioritySpawnPoints)
        {
            spawnPoints = new();

            spawnPoints.AddRange(PrioritySpawnPoints);
            spawnPoints.AddRange(SpawnPoints);
        }
        else
        {
            spawnPoints = SpawnPoints;
        }

        if (CountOfItemsToSpawn > spawnPoints.Count)
        {
            Debug.Log("Couldn't spawn Items because there were more Items requested than SpawnPoints available!");

            return;
        }

		for (var i = 0; i < CountOfItemsToSpawn; ++i)
        {
            var nextSpawnPointIndex = (int)(Random.value*spawnPoints.Count - 0.01f);
            var nextSpawnPoint = spawnPoints[nextSpawnPointIndex];
            spawnPoints.Remove(nextSpawnPoint);

            SpawnItem(nextSpawnPoint);
		}

        // Test
        var testDynProgrAlg = new DynamicProgAlgoBehaviour();

        var itemProps = new ItemProperties[m_Items.Count];
        for (var i = 0; i < itemProps.Length; ++i)
        {
            var curItemPropsComp = PlayerHUDController.GetComponentFromParent<ItemProperties>(m_Items[i].transform);

			if (curItemPropsComp != null)
            {
                itemProps[i] = curItemPropsComp;
            }
            else
            {
                Debug.Log("Couldn't Test Dynamic Progr Alg. because unable to obtain the Item Properties Components.");

                return;
            }
        }

		testDynProgrAlg.StartAlgorithm(itemProps, PlayerHUDController.Instance.GetKnapsackCapacity(), out var selectedItems, out var steps, out var totalValue, out var totalWeight);
        Debug.Log("Total Value: " + totalValue);
        Debug.Log("Total Weight: " + totalWeight);

        var selectedItemProperties = (ItemProperties[])selectedItems;
        Debug.Log("Selected Items Count: " + selectedItemProperties.Length);

        foreach (var selItem in selectedItemProperties)
        {
            Debug.Log("Value: " + selItem.value + ", Weight: " + selItem.weight);
        }
	}

    private void SetItemTagForChildren(Transform parent)
    {
        for (var i = 0; i < parent.childCount; ++i)
        {
            var curChild = parent.GetChild(i);

            curChild.tag = "Item";
            SetItemTagForChildren(curChild);
		}
    }

	private void SpawnItem(GameObject spawnPoint, int value, int weight, GameObject prefab)
    {
		var itemParent = spawnPoint.transform.Find("Item");

        var spawnPosition = itemParent.position;
        spawnPosition.y += 0.15f;

        var prefabEulerRotation = prefab.transform.rotation.eulerAngles;
        var spawnPointEulerRotation = spawnPoint.transform.rotation.eulerAngles;

		var curItem = Instantiate(prefab, spawnPosition, Quaternion.Euler(prefabEulerRotation.x, spawnPointEulerRotation.y, prefabEulerRotation.z), itemParent);
		m_UsedSpawnPoints.Add(spawnPoint);
		curItem.tag = "Item";

        // Wichtig: Weil die Itemprefabs manchmal aus mehr als nur einem
        // GameObject bestehen (Kinder besitzen), müssen wir diesen
        // (da diese die eigentlich sichtbaren Objekte sind) ebenfalls den Tag Item geben 
        SetItemTagForChildren(curItem.transform);

		m_Items.Add(curItem);

        var itemProperties = PlayerHUDController.GetComponentFromParent<ItemProperties>(curItem.transform);
		itemProperties.value = value;
		itemProperties.weight = weight;

		// If an Item has spawned on the current SpawnPoint, set its Image to an X instead of a +
		var imageComponent = spawnPoint.transform.Find("Canvas").Find("Image").GetComponent<Image>();
		imageComponent.sprite = RemoveItemImage;

        var itemPrefab = PlayerHUDController.GetComponentFromParent<ItemPrefab>(curItem.transform);
		itemPrefab.SetPrefab(prefab);

		itemProperties.Id = GetNextId();
	}

	private void SpawnItem(GameObject spawnPoint)
	{
		var randomItemTemplate = GetRandomItem();
		SpawnItem(spawnPoint, randomItemTemplate.Value, randomItemTemplate.Weight, randomItemTemplate.Model);
	}

	private void SpawnItem(GameObject spawnPoint, int value, int weight)
	{
		var randomItemTemplate = GetRandomItem();
		SpawnItem(spawnPoint, value, weight, randomItemTemplate.Model);
	}

	private int GetNextId()
    {
        if (m_FreedIds.Count > 0)
        {
            return m_FreedIds.Dequeue();
        }

        return s_CurId++;
	}

    private Item GetRandomItem()
    {
        // A Cast to an 'int' will always Floor the Value. We also have to
        // omit the actual Length of the 'itemModelPrefabs' Array because it starts with 0.
        // To now have a fair Distribution of all Items, let the last ItemModel have
        // a Range of 0 to 0.97 (0.97 will still be Floored to 0)
        var indexItem = (int)Random.Range(0.0f, ItemPrefabs.Count - 0.03f);

        return ItemPrefabs[indexItem];
    }

    private void SpawnSigns()
    {
        foreach (var curSpawnPoint in m_UsedSpawnPoints)
        {
            SpawnSign(curSpawnPoint);
		}
    }

    private void SpawnSign(GameObject spawnPoint)
    {
		// How the Sign has to be translated from the Position of the SpawnPoint
		var spawnLocationSignOffsetY = new Vector3(0.0f, -0.014f, 0.0f);

		// The spawned Item is the Child of the current SpawnPoint
		// We assume it only has this 1 Child
		var spawnedItem = spawnPoint.transform.Find("Item").GetChild(0);

		var spawnPointPos = spawnPoint.transform.position;
		// 'right' is the red Arrow in the Editor
		var curSpawnLocationSignOffset = spawnLocationSignOffsetY + spawnPoint.transform.right*0.4f;
		var signPos = spawnPointPos + curSpawnLocationSignOffset;

        //Debug.Log(SignParentObject == null);
        // Spawnt zur Runtime nicht im Parent??
		var sign = Instantiate(SignPrefab, signPos, spawnPoint.transform.rotation, SignParentObject);

		var itemProperties = PlayerHUDController.GetComponentFromParent<ItemProperties>(spawnedItem);
		var signControllerScript = PlayerHUDController.GetComponentFromParent<SignController>(sign.transform);

		signControllerScript.SetWeight(itemProperties.weight);
		signControllerScript.SetValue(itemProperties.value);

		signControllerScript.Id = itemProperties.Id;
	}

    public static Transform GetSpawnPoint(GameObject item)
    {
		// Weil das angeklickte Objekt nicht immer der Parent
		// des eigentlichen Items, welches gespawnt wurde, ist
		var result = item.transform.parent.parent;
        for (var i = 0; !result.CompareTag("ItemSpawnPoint") && i < 5; result = result.parent, ++i);

        return result;
    }

    public void RemoveItem(GameObject item)
    {
        // Only has 1 Child -> the Item(model)
		m_Items.Remove(item);
		m_FreedIds.Enqueue(PlayerHUDController.GetComponentFromParent<ItemProperties>(item.transform).Id);

		// Remove and Destroy the associated Sign
		var associatedSign = FindAssociatedSign(item);
        if (associatedSign != null)
        {
            Destroy(associatedSign);
        }

		m_UsedSpawnPoints.Remove(item);
		Destroy(item);

        var spawnPoint = GetSpawnPoint(item);
        var imageObj = spawnPoint.Find("Canvas").GetChild(0).gameObject;
		imageObj.GetComponent<Image>().sprite = AddItemImage;
	}

	public void AddItem(Transform spawnPoint)
    {
        var inventory = m_Player.GetComponent<Inventory>();
        if (inventory.GetItems().Count == 0)
        {
			if (s_ActiveAddItemMenu == null)
			{
				s_ActiveAddItemMenu = Instantiate(AddItemMenu);
				s_CurrentAddItemMenuSpawnPoint = spawnPoint;
			}
		}
        else
        {
            if (s_ActiveAddItemSelectMenu == null)
            {
				s_ActiveAddItemSelectMenu = Instantiate(AddItemSelectMenu);
				s_CurrentAddItemMenuSpawnPoint = spawnPoint;
			}
        }
	}

	public void OnAddNewItemButtonPressed()
	{
		if (s_ActiveAddItemMenu == null)
		{
			s_ActiveAddItemMenu = Instantiate(AddItemMenu);
		}

        Destroy(s_ActiveAddItemSelectMenu);
        s_ActiveAddItemSelectMenu = null;
	}

	public void OnAddInventoryItemButtonPressed()
	{
		Destroy(s_ActiveAddItemSelectMenu);
		s_ActiveAddItemSelectMenu = null;

        IsSelectingHUDItem = true;
	}

    public void OnHUDItemSelected(HUDItem hudItem)
    {
        SpawnItem(s_CurrentAddItemMenuSpawnPoint.gameObject, hudItem.ItemProperties.value, hudItem.ItemProperties.weight, hudItem.Prefab);
        SpawnSign(s_CurrentAddItemMenuSpawnPoint.gameObject);

        var inventory = m_Player.GetComponent<Inventory>();
        inventory.RemoveItem(hudItem);
		IsSelectingHUDItem = false;
	}

	public void OnAddItemMenuCancelButtonPressed()
    {
        Destroy(s_ActiveAddItemMenu);
    }

	public void OnAddItemMenuAddButtonPressed()
	{
        var menuCanvas = s_ActiveAddItemMenu.transform.Find("Canvas");
        var inputFieldValue = menuCanvas.Find("InputField_Value").GetComponent<TMP_InputField>().text;
        var inputFieldWeight = menuCanvas.Find("InputField_Weight").GetComponent<TMP_InputField>().text;

        // TODO: Check if the Entries of the InputFields are valid
        SpawnItem(s_CurrentAddItemMenuSpawnPoint.gameObject, int.Parse(inputFieldValue), int.Parse(inputFieldWeight));
        SpawnSign(s_CurrentAddItemMenuSpawnPoint.gameObject);

		var imageComponent = s_CurrentAddItemMenuSpawnPoint.Find("Canvas").Find("Image").GetComponent<Image>();
        imageComponent.sprite = RemoveItemImage;

		Destroy(s_ActiveAddItemMenu);
	}

	private GameObject FindAssociatedSign(GameObject item)
    {
        var signs = GameObject.FindGameObjectsWithTag("Sign");
        foreach (var sign in signs)
        {
            if (PlayerHUDController.GetComponentFromParent<ItemProperties>(item.transform).Id ==
				PlayerHUDController.GetComponentFromParent<SignController>(sign.transform).Id)
            {
                return sign;
            }
        }

        return null;
    }
}
