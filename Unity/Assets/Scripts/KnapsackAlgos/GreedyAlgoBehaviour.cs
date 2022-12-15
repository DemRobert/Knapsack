using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GreedyAlgoBehaviour : AlgoBehaviour
{
    public override void StartAlgorithm(Item[] items, int maxWeight, out Item[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
        _maximumWeight = maxWeight;

        CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Starting, items);

        // Sort Items by Value to Weight Ratio
        SortItemsByValueToWeightRatio(items);

        // Get Most Valuable Items that don't exceed the weight limit
        selectedItems = GetMostValuableItems(items);

        // Get Total Value and Weight of Selected items
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
        steps = _steps.ToArray();
    }

    private Item[] SortItemsByValueToWeightRatio(Item[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
            {
                // Value to weight ratio
                double firstRatio = (double)items[j].Value / (double)items[j].Weight;
                double secondRatio = (double)items[j + 1].Value / (double)items[j + 1].Weight;

                CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Comparing, new Item[] { items[j], items[j + 1] });

                // Swap if first ratio is smaller
                if (firstRatio < secondRatio)
                {
                    Item temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                    CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Swapping, items);
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

    private void CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations operation, object value)
    {
        GreedyAlgoStep algoStep = new GreedyAlgoStep(operation, value);
        // Add Step to List of Steps
        _steps.Add(algoStep);
    }
}
