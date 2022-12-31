using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicProgAlgoBehaviour : AlgoBehaviour
{
    public ItemPropertiesNoUnity[] Items;
    public ItemPropertiesNoUnity[] SelectedItems;

    public override void StartAlgorithm(object[] items, int maxWeight, out object[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
		Items = (ItemPropertiesNoUnity[])items;

        var matrix = new int[items.Length+1][];
        for (var i = 0; i < matrix.Length; ++i)
        {
            matrix[i] = new int[maxWeight+1];
        }

        //Debug.Log("Max Weight: " + maxWeight);
        //Debug.Log("Item Count: " + items.Length);
        _steps.Clear();

		for (var itemIndex = 1; itemIndex <= items.Length; ++itemIndex)
        {
            for (var curCapacity = 1; curCapacity <= maxWeight; ++curCapacity)
            {
                var curItem = Items[itemIndex-1];

				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.NEW_CELL, null, curCapacity, itemIndex-1);
				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT01, null, curCapacity, itemIndex-1);

                var isItemOk = curItem.Weight <= curCapacity;
				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT02, isItemOk, curCapacity, itemIndex-1);

                var withCurItemOldCapacity = curCapacity - curItem.Weight;
				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_WEIGHT03, withCurItemOldCapacity, curCapacity, itemIndex-1);

				if (isItemOk)
                {
                    var valueWithoutCurItem = matrix[itemIndex-1][curCapacity];
                    var valueWithCurItem = matrix[itemIndex-1][withCurItemOldCapacity] + curItem.Value;

                    var isBetterWithCurItem = valueWithCurItem > valueWithoutCurItem;
					CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.HIGHLIGHT_BEST_OPTION, isBetterWithCurItem, curCapacity, itemIndex-1);
					CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_VALUES, isBetterWithCurItem, curCapacity, itemIndex-1);

					// Falls wir einen besseren Wert mit dem aktuellen Item erreichen
					if (isBetterWithCurItem)
                    {
                        matrix[itemIndex][curCapacity] = valueWithCurItem;
					}
                    else
                    {
						matrix[itemIndex][curCapacity] = valueWithoutCurItem;
					}
				}
				// Falls das aktuelle Item nicht in einen 'Rucksack' mit der Kapazität
				// von 'curCapacity' passt, übernimm einfach den maximal erreichbaren
				// Wert, den wir eine Stufe zuvor (ohne dieses Item) erreicht haben
				else
				{
                    var value = matrix[itemIndex-1][curCapacity];
					CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.COMPARE_VALUES, value, curCapacity, itemIndex-1);
					matrix[itemIndex][curCapacity] = value;
				}

				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.END, null, curCapacity, itemIndex-1);
			}
		}

        selectedItemsTotalValue = matrix[items.Length][maxWeight];

        var selectedItemsCapacity = maxWeight;
        var selectedItemsValue = selectedItemsTotalValue;

		var selectedItemsList = new List<ItemPropertiesNoUnity>();
		for (var i = items.Length; i > 0 && selectedItemsValue > 0; --i)
        {
            if (selectedItemsValue == matrix[i-1][selectedItemsCapacity])
            {
                continue;
            }

            var curItem = Items[i-1];

			selectedItemsList.Add(curItem);
            selectedItemsCapacity -= curItem.Weight;
            selectedItemsValue -= curItem.Value;
		}

        selectedItemsTotalWeight = 0;
		foreach (var item in selectedItemsList)
        {
            selectedItemsTotalWeight += item.Weight;
		}

		SelectedItems = selectedItemsList.ToArray();
        selectedItems = SelectedItems;

        steps = _steps.ToArray();
	}

	private void CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations operation, object ding, int curCapacity, int curItemIndex)
	{
		var algoStep = new DynamicProgAlgoStep(operation, ding, curCapacity, curItemIndex);
		// Add Step to List of Steps
		_steps.Add(algoStep);
	}
}
