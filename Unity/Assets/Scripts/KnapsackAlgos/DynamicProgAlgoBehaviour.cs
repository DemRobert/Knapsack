using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicProgAlgoBehaviour : AlgoBehaviour
{
    public ItemProperties[] Items;
    public ItemProperties[] SelectedItems;

    public override void StartAlgorithm(object[] items, int maxWeight, out object[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
		Items = (ItemProperties[])items;

        var matrix = new int[items.Length+1][];
        for (var i = 0; i < matrix.Length; ++i)
        {
            matrix[i] = new int[maxWeight+1];
        }

        //Debug.Log("Max Weight: " + maxWeight);
        //Debug.Log("Item Count: " + items.Length);

		for (var itemIndex = 1; itemIndex <= items.Length; ++itemIndex)
        {
            for (var curCapacity = 1; curCapacity <= maxWeight; ++curCapacity)
            {
                var curItem = Items[itemIndex-1];

				if (curItem.weight <= curCapacity)
                {
                    var valueWithoutCurItem = matrix[itemIndex-1][curCapacity];
                    var valueWithCurItem = matrix[itemIndex-1][curCapacity - curItem.weight] + curItem.value;

                    // Falls wir einen besseren Wert mit dem aktuellen Item erreichen
                    if (valueWithCurItem > valueWithoutCurItem)
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
                    matrix[itemIndex][curCapacity] = matrix[itemIndex-1][curCapacity];
				}

				CreateAlgoStep(DynamicProgAlgoStep.DynamicAlgoOperations.ONE, matrix[itemIndex][curCapacity], curCapacity, itemIndex-1);
			}
        }

        selectedItemsTotalValue = matrix[items.Length][maxWeight];

        var selectedItemsCapacity = maxWeight;
        var selectedItemsValue = selectedItemsTotalValue;

		var selectedItemsList = new List<ItemProperties>();
		for (var i = items.Length; i > 0 && selectedItemsValue > 0; --i)
        {
            if (selectedItemsValue == matrix[i-1][selectedItemsCapacity])
            {
                continue;
            }

            var curItem = Items[i-1];

			selectedItemsList.Add(curItem);
            selectedItemsCapacity -= curItem.weight;
            selectedItemsValue -= curItem.value;
		}

        selectedItemsTotalWeight = 0;
		foreach (var item in selectedItemsList)
        {
            selectedItemsTotalWeight += item.weight;
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
