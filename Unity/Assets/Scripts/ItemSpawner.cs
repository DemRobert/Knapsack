using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<Item> Items = new();
    public List<GameObject> SpawnPoints = new();

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        // Beispiel
        Instantiate(Items[0].Model, SpawnPoints[0].transform);
    }
}
