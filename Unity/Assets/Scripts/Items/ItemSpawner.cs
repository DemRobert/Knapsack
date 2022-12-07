using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public List<Item> Items = new();

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

	private List<GameObject> m_UsedSpawnPoints = new();

    public Sprite AddItemImage;
    public Sprite RemoveItemImage;

    private static int s_CurId = 0;
    private Queue<int> m_FreedIds = new();

    public GameObject AddItemMenu;
    private static GameObject s_ActiveAddItemMenu;
    private static Transform s_CurrentAddItemMenuSpawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        CollectSpawnPoints();
        InitializeSpawnPoints();

		SpawnItems();
        SpawnSigns();
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
            SpawnItem(spawnPoints[i]);
		}
    }

    private void SpawnItem(GameObject spawnPoint, int value, int weight)
    {
		var curItemTemplate = GetRandomItem();
		var itemParent = spawnPoint.transform.Find("Item");

		var curItem = Instantiate(curItemTemplate.Model, itemParent);
		m_UsedSpawnPoints.Add(spawnPoint);

        var itemProperties = curItem.GetComponent<ItemProperties>();
        if (value == -1 || weight == -1)
        {
            itemProperties.Set(curItemTemplate);
        }
        else
        {
            itemProperties.value = value;
            itemProperties.weight = weight;
        }

		// If an Item has spawned on the current SpawnPoint, set its Image to an X instead of a +
		var imageComponent = spawnPoint.transform.Find("Canvas").Find("Image").GetComponent<Image>();
		imageComponent.sprite = RemoveItemImage;

		itemProperties.Id = GetNextId();
	}

	private void SpawnItem(GameObject spawnPoint)
    {
        SpawnItem(spawnPoint, -1, -1);
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
        var indexItem = (int)Random.Range(0.0f, Items.Count - 0.03f);

        return Items[indexItem];
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
		var curSpawnLocationSignOffset = spawnLocationSignOffsetY + spawnedItem.right * 0.2f;
		var signPos = spawnPointPos + curSpawnLocationSignOffset;

        //Debug.Log(SignParentObject == null);
		var sign = Instantiate(SignPrefab, signPos, spawnPoint.transform.rotation, SignParentObject);

		var itemProperties = spawnedItem.GetComponent<ItemProperties>();
		var signControllerScript = sign.GetComponent<SignController>();

		signControllerScript.SetWeight(itemProperties.weight);
		signControllerScript.SetValue(itemProperties.value);

		signControllerScript.Id = itemProperties.Id;
	}

    public void RemoveItem(Transform spawnPoint)
    {
        var itemParent = spawnPoint.Find("Item");
        // Only has 1 Child -> the Item(model)
        var item = itemParent.GetChild(0).gameObject;

        m_FreedIds.Enqueue(item.GetComponent<ItemProperties>().Id);

		// Remove and Destroy the associated Sign
		var associatedSign = FindAssociatedSign(item);
        if (associatedSign != null)
        {
            Destroy(associatedSign);
        }

		m_UsedSpawnPoints.Remove(item);
		Destroy(item);
	}

	public void AddItem(Transform spawnPoint)
    {
        if (s_ActiveAddItemMenu == null)
        {
            s_ActiveAddItemMenu = Instantiate(AddItemMenu);
            s_CurrentAddItemMenuSpawnPoint = spawnPoint;
		}
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
            if (item.GetComponent<ItemProperties>().Id ==
				sign.GetComponent<SignController>().Id)
            {
                return sign;
            }
        }

        return null;
    }
}
