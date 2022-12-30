using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GreedyAlgoBehaviour : AlgoBehaviour
{
    public override void StartAlgorithm(object[] items, int maxWeight, out object[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
        _maximumWeight = maxWeight;

        CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Starting, 0, _maximumWeight, 0, null, (ItemProperties[])items, null);

        // Sort Items by Value to Weight Ratio
        SortItemsByValueToWeightRatio((ItemProperties[])items);

        CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Sorting, 0, _maximumWeight, 0, null, (ItemProperties[])items, null);

        // Get Most Valuable Items that don't exceed the weight limit
        selectedItems = GetMostValuableItems((ItemProperties[])items);

        // Get Total Value and Weight of Selected items
        int values = 0;
        int weights = 0;
        foreach (ItemProperties item in selectedItems) 
        {
            values += item.value;
            weights += item.weight;
        }
        selectedItemsTotalValue = values;
        selectedItemsTotalWeight = weights;

        CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Ending, 0, _maximumWeight, 0, null, (ItemProperties[])items, (ItemProperties[])selectedItems);
        // Steps of Algorithm
        steps = _steps.ToArray();
    }

    private ItemProperties[] SortItemsByValueToWeightRatio(ItemProperties[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
            {
                // Value to weight ratio
                double firstRatio = (double)items[j].value / (double)items[j].weight;
                double secondRatio = (double)items[j + 1].value / (double)items[j + 1].weight;

                // Swap if first ratio is smaller
                if (firstRatio < secondRatio)
                {
                    ItemProperties temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
            }
        return items;
    }

    private ItemProperties[] GetMostValuableItems(ItemProperties[] items)
    {
        List<ItemProperties> selectedItems = new();
        int totalItemWeight = 0;

        for (int i = 0; i < items.Length; i++)
        {
            CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Selecting, totalItemWeight, _maximumWeight, i, items[i], items, selectedItems.ToArray());

            if (totalItemWeight == _maximumWeight)
                break;
            else if (totalItemWeight + items[i].weight > _maximumWeight)
                continue;
            else
            {
                totalItemWeight += items[i].weight;
                selectedItems.Add(items[i]);
            }
        }
        return selectedItems.ToArray();
    }

    private void CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations operation, int currentWeight, int maxWeight, int currentIndex, ItemProperties currentItem, ItemProperties[] items, ItemProperties[] selectedItems)
    {
        GreedyAlgoStep algoStep = new GreedyAlgoStep(operation, currentWeight, maxWeight, currentIndex, currentItem, items, selectedItems);
        // Add Step to List of Steps
        _steps.Add(algoStep);
    }
}
