using System;
using System.Collections.Generic;

public class GreedyAlgoBehaviour : AlgoBehaviour
{
    public ItemPropertiesNoUnity[] Items;
    public ItemPropertiesNoUnity[] ItemsUnmodified;

    public override void StartAlgorithm(object[] items, int maxWeight, out object[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
        _maximumWeight = maxWeight;

        Items = (ItemPropertiesNoUnity[])items;
        ItemsUnmodified = new ItemPropertiesNoUnity[Items.Length];
        Array.Copy(Items, ItemsUnmodified, Items.Length);
        _steps.Clear();

		//CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Starting, 0, _maximumWeight, 0, null, (ItemProperties[])items, null);

        // Computing the Value Weight Ratios on Screen
        for (var i = 0; i < items.Length; ++i)
        {
            var curItem = Items[i];

            CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_VALUE, curItem, i);
            CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_WEIGHT, curItem, i);
            CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.COMPUTE_VW_RATIO, curItem, i);
            CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_COLUMN, curItem, i);
		}

		// Sort Items by Value to Weight Ratio
		SortItemsByValueToWeightRatio(Items);

        for (var i = 0; i < items.Length; ++i)
        {
            var curItem = Items[i];

			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_ALL_VW_RATIOS, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_CUR_BEST_VW_RATIO, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.DRAW_CUR_BEST_ITEM_INDEX, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.DRAW_CUR_BEST_VW_RATIO, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_CUR_BEST_ALL, curItem, i);
		}

		for (var i = 0; i < items.Length; ++i)
		{
			var curItem = Items[i];

			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_BEST_ITEM, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.HIGHLIGHT_CAP_AND_WEIGHT, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.CHOOSE_IF_TAKE, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.ADD_TO_TOT_VAL, curItem, i);
			CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.DEHIGHLIGHT_CAP_WEIGHT, curItem, i);
		}

		//CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Sorting, 0, _maximumWeight, 0, null, (ItemProperties[])items, null);

		// Get Most Valuable Items that don't exceed the weight limit
		selectedItems = GetMostValuableItems(Items);

        // Get Total Value and Weight of Selected items
        int values = 0;
        int weights = 0;
        foreach (ItemPropertiesNoUnity item in selectedItems) 
        {
            values += item.Value;
            weights += item.Weight;
        }
        selectedItemsTotalValue = values;
        selectedItemsTotalWeight = weights;

        //CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Ending, 0, _maximumWeight, 0, null, (ItemProperties[])items, (ItemProperties[])selectedItems);
        // Steps of Algorithm
        steps = _steps.ToArray();
    }

    private ItemPropertiesNoUnity[] SortItemsByValueToWeightRatio(ItemPropertiesNoUnity[] items)
    {
        int n = items.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - i - 1; j++)
            {
                // Value to weight ratio
                double firstRatio = (double)items[j].Value / (double)items[j].Weight;
                double secondRatio = (double)items[j + 1].Value / (double)items[j + 1].Weight;

                // Swap if first ratio is smaller
                if (firstRatio < secondRatio)
                {
					ItemPropertiesNoUnity temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
            }
        return items;
    }

    private ItemPropertiesNoUnity[] GetMostValuableItems(ItemPropertiesNoUnity[] items)
    {
        List<ItemPropertiesNoUnity> selectedItems = new();
        int totalItemWeight = 0;

        for (int i = 0; i < items.Length; i++)
        {
            //CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations.Selecting, totalItemWeight, _maximumWeight, i, items[i], items, selectedItems.ToArray());

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

    private void CreateAlgoStep(GreedyAlgoStep.GreedyAlgoOperations operation, ItemPropertiesNoUnity curItem, int itemIndex)
    {
        GreedyAlgoStep algoStep = new GreedyAlgoStep(operation, curItem, itemIndex);
        // Add Step to List of Steps
        _steps.Add(algoStep);
    }
}
