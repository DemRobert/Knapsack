using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invetory : MonoBehaviour
{
    public static Invetory Instance;

    public List<Item> Items = new();
    public int TotalWeight = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple Instances of Inventory!");
            return;
        }
        Instance = this;
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
        TotalWeight += item.Weight;
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        TotalWeight -= item.Weight;
    }
}
