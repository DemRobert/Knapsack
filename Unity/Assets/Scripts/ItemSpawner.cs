using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance;

    public List<Item> Items = new();

    // If set to false, the PrioritySpawnPoints will be ignored and
    // Items will just spawn at the Spawn Points of SpawnPoints in order
    public bool ConsiderPrioritySpawnPoints = true;
    public int CountOfItemsToSpawn = 5;

    // A List of all possible SpawnPoints
    // The SpawnPoints
    public List<GameObject> SpawnPoints = new();
    // A List of SpawnPoints, which are preferred for spawning
    // Items (Items will spawn first at these SpawnPoints)
    public List<GameObject> PrioritySpawnPoints = new();

    public GameObject signPrefab;
    public GameObject signParentObject;

    private List<GameObject> m_UsedSpawnPoints = new();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        CollectSpawnPoints();

        SpawnItems();
        SpawnSigns();
    }

    private void CollectSpawnPoints()
    {
        // Assume there is only 1 Object with this Tag
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
            var curItemTemplate = GetRandomItem();
            var usedSpawnPoint = spawnPoints[i];

            var curItem = Instantiate(curItemTemplate.Model, usedSpawnPoint.transform);
            m_UsedSpawnPoints.Add(usedSpawnPoint);

            var itemProperties = curItem.GetComponent<ItemProperties>();
            itemProperties.Set(curItemTemplate);
        }
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
        // How the Sign has to be translated from the Position of the SpawnPoint
        var spawnLocationSignOffsetY = new Vector3(0.0f, -0.014f, 0.0f);

        foreach (var curSpawnPoint in m_UsedSpawnPoints)
        {
            var spawnPointPos = curSpawnPoint.transform.position;

            // The spawned Item is the Child of the current SpawnPoint
            // We assume it only has this 1 Child
            var spawnedItem = curSpawnPoint.transform.GetChild(0);

            // 'right' is the red Arrow in the Editor
            var curSpawnLocationSignOffset = spawnLocationSignOffsetY + spawnedItem.right*0.2f;
            var signPos = spawnPointPos + curSpawnLocationSignOffset;

            var sign = Instantiate(signPrefab, signPos, curSpawnPoint.transform.rotation, signParentObject.transform);

            var itemProperties = spawnedItem.GetComponent<ItemProperties>();
            var signControllerScript = sign.GetComponent<SignController>();

            signControllerScript.SetWeight(itemProperties.weight);
            signControllerScript.SetValue(itemProperties.value);
        }
    }
}
