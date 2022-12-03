using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GreedyAlgoBehaviour : MonoBehaviour
{
    //public Item[] Items;
    [SerializeField] private int _maximumWeight;
    //[SerializeField] private Item[] _mostValueableItems;
    [SerializeField] private List<AlgorithmStep> steps = new List<AlgorithmStep>();

    // Start is called before the first frame update
    void Start()
    {
        /* DEBUG TEST
        //_items = ItemSpawner.Instance.Items.ToArray();

        int itemWeightSum = 0;
        foreach(Item item in Items)
        {
            itemWeightSum += item.Weight;
        }
        // Testweise maxweight = 2/3 des gesamtgewichts
        MaximumWeight = itemWeightSum * 2 / 3;

        //SortedItems = SortItemsByValue(Items);
        SortItemsByValueToWeightRatio(Items);
        _mostValueableItems = GetMostValuableItems(Items);

        // DEBUG
        Debug.Log("Sorted:");
        foreach(Item item in Items)
        {
            Debug.Log($"{item.Name}: Value={item.Value}, Weight={item.Weight}, Ratio={(double)item.Value / (double)item.Weight}");
        }


        Debug.Log($"MaxWeight: {MaximumWeight}");
        foreach(Item item in _mostValueableItems)
        {
            Debug.Log($"{item.Name}: Value={item.Value}, Weight={item.Weight}");
        }
         */
    }

    public void StartAlgorithm(Item[] items, int maxWeight, out Item[] selectedItems, out AlgorithmStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
        _maximumWeight = maxWeight;
        // Sort Items by Value to Weight Ratio
        SortItemsByValueToWeightRatio(items);
        // Get Most Valuable Items that don't exceed the weight limit
        selectedItems = GetMostValuableItems(items);
        // Get Total Value/Weight of Selected items
        int values = 0;
        int weights = 0;
        foreach (Item item in selectedItems) 
        {
            values += item.Value;
            weights += item.Weight;
        }
        selectedItemsTotalValue = values;
        selectedItemsTotalWeight = weights;
        // Steps of Algorithm
        steps = null;
    }

    private Item[] SortItemsByValue(Item[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
                // Swap if first value is smaller
                if (items[j].Value < items[j + 1].Value)
                {
                    Item temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
        return items;
    }

    private Item[] SortItemsByValueToWeightRatio(Item[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
            {
                // Value To Weight Ratio
                double firstRatio = (double)items[j].Value / (double)items[j].Weight;
                double secondRatio = (double)items[j + 1].Value / (double)items[j + 1].Weight;

                // Swap if first ratio is smaller
                if (firstRatio < secondRatio)
                {
                    Item temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
            }
        return items;
    }

    private Item[] GetMostValuableItems(Item[] items)
    {
        List<Item> selectedItems = new();
        int totalItemWeight = 0;

        for (int i = 0; i < items.Length; i++)
        {
            if (totalItemWeight == _maximumWeight)
                break;
            else if (totalItemWeight + items[i].Weight > _maximumWeight)
                continue;
            else
            {
                totalItemWeight += items[i].Weight;
                selectedItems.Add(items[i]);
            }
        }
        return selectedItems.ToArray();
    }



}
