using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform[] itemSpawnPoints;
    public GameObject[] itemModelPrefabs;

    public GameObject itemPrefab;
    public GameObject signPrefab;

    public int numItems = 3;
    // Save the SpawnPoint and the associated Item GameObject which tells us
    // whether an Item has been placed if the GameObject is not null
    private Dictionary<Transform, GameObject> m_SpawnPointStates = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        m_SpawnPointStates.EnsureCapacity(itemSpawnPoints.Length);
        for (int i = 0; i < itemSpawnPoints.Length; ++i)
        {
            m_SpawnPointStates.Add(itemSpawnPoints[i], null);
        }

        // Always Spawn the first 5 Items on the Top
        // Shelf in Order the SpawnPoints were defined in
        int itemsToSpawnLeft = numItems;

        for (int i = 0; i < 5; ++i)
        {
            if (itemsToSpawnLeft <= 0)
            {
                break;
            }

            Transform curSpawnPoint = itemSpawnPoints[i];
            GameObject newItem = Instantiate(itemPrefab, curSpawnPoint);
            ItemProperties newItemProps = newItem.GetComponent<ItemProperties>();

            // Add a random Item Model + its Properties to the just created One
            GameObject newItemModel = GetRandomModel();
            // Copy its Item Properties
            newItemProps.Set(newItemModel);

            Instantiate(newItemModel, newItem.transform);

            // Save that an Item has been placed at the current SpawnPoint
            m_SpawnPointStates[curSpawnPoint] = newItemModel;

            --itemsToSpawnLeft;
        }

        if (itemsToSpawnLeft > 0)
        {
            // TODO: Spawn Objects on other random Locations
        }

        // The Parent of all Signs
        Transform signsParent = GameObject.Find("Signs").transform;

        // How the Sign has to be translated from the Position of the SpawnPoint
        Vector3 spawnLocationSignOffset = new(0.2f, -0.014f, 0.0f);
        // Spawn Signs next to the SpawnPoints where Items have been placed
        foreach (var curSpawnPointState in m_SpawnPointStates)
        {
            GameObject curItem = curSpawnPointState.Value;
            // If no Item has been placed at the current SpawnPoint
            if (curItem == null)
            {
                continue;
            }

            Transform curSpawnPoint = curSpawnPointState.Key;
            Vector3 signLocation = curSpawnPoint.position + spawnLocationSignOffset;

            GameObject newSign = Instantiate(signPrefab, signsParent);
            newSign.transform.position = signLocation;

            SignController curSignController = newSign.GetComponent<SignController>();
            ItemProperties curItemProps = curItem.GetComponent<ItemProperties>();

            curSignController.SetValue(curItemProps.value);
            curSignController.SetWeight(curItemProps.weight);
        }
    }

    private GameObject GetRandomModel()
    {
        // A Cast to an 'int' will always Floor the Value. We also have to
        // omit the actual Length of the 'itemModelPrefabs' Array because it starts with 0.
        // To now have a fair Distribution of all Items, let the last ItemModel have
        // a Range of 0 to 0.97 (0.97 will still be Floored to 0)
        int indexModel = (int)Random.Range(0.0f, itemModelPrefabs.Length - 0.03f);

        return itemModelPrefabs[indexModel];
    }
}
