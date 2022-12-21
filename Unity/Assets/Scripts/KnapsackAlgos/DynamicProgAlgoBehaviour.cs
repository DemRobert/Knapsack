using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicProgAlgoBehaviour : AlgoBehaviour
{
    public override void StartAlgorithm(Item[] items, int maxWeight, out Item[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight)
    {
        var matrix = new int[items.Length+1][];
        for (var i = 0; i < matrix.Length; ++i)
        {
            matrix[i] = new int[maxWeight+1];
        }

        var selectedItemsList = new List<Item>();

        for (var itemIndex = 1; itemIndex <= items.Length; ++itemIndex)
        {
            for (var curCapacity = 1; curCapacity <= maxWeight; ++curCapacity)
            {
                var curItem = items[itemIndex-1];

                if (curItem.Weight <= curCapacity)
                {
                    var valueWithoutCurItem = matrix[itemIndex-1][curCapacity];
                    var valueWithCurItem = matrix[itemIndex-1][curCapacity - curItem.Weight] + curItem.Value;

                    // Falls wir einen besseren Wert mit dem aktuellen Item erreichen
                    if (valueWithCurItem > valueWithoutCurItem)
                    {
                        matrix[itemIndex][curCapacity] = valueWithCurItem;
                        selectedItemsList.Add(curItem);
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
            }
        }

        selectedItemsTotalValue = matrix[items.Length][maxWeight];
        selectedItems = selectedItemsList.ToArray();

        selectedItemsTotalWeight = 0;
		foreach (var item in selectedItems)
        {
            selectedItemsTotalWeight += item.Weight;
		}

        steps = null;
	}
}
