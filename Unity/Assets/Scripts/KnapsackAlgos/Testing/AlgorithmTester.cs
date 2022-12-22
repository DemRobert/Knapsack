using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;

public class AlgorithmTester : MonoBehaviour
{
    public enum AlgoTypes { Greedy, Dynamic }

    [SerializeField] private Item[] _items;
    [SerializeField] private AlgoTypes _algoType;
    [SerializeField] private int _maxWeight;
    private AlgoBehaviour _algorithm;
    private Item[] _selectedItems;
    private AlgoStep[] _algoSteps;
    private int _selectedItemsTotalValue;
    private int _selectedItemsTotalWeight;


    private void Start()
    {
        if (_items != null && _maxWeight > 0)
        {
            if (_algoType == AlgoTypes.Greedy)
            {
                _algorithm = new GreedyAlgoBehaviour();
                _algorithm.StartAlgorithm(_items, _maxWeight, out _selectedItems, out _algoSteps, out _selectedItemsTotalValue, out _selectedItemsTotalWeight);                
            }
            else if (_algoType == AlgoTypes.Dynamic)
            {
                //_algorithm = new DynamicProgAlgoBehaviour();
                //_algorithm.StartAlgorithm(_items, _maxWeight, out _selectedItems, out _algoSteps, out _selectedItemsTotalValue, out _selectedItemsTotalWeight);
            }

            string itemsString = GetItemsInfoString(_items);
            string selectedItemsString = GetItemsInfoString(_selectedItems);
            string algoStepsInfo = GetAlgoStepsInfoString(_algoSteps, _algoType);            

            // Output info
            LogAlgoInfo("Greedy Algo Test", itemsString, selectedItemsString, algoStepsInfo);
        }
    }

    /// <summary>
    /// Returns Info String of AlgoStep Array
    /// </summary>
    /// <param name="algoSteps"></param>
    /// <param name="algoType"></param>
    /// <returns></returns>
    private string GetAlgoStepsInfoString(AlgoStep[] algoSteps, AlgoTypes algoType)
    {
        string algoStepsInfo = string.Empty;

        // Create info string of algo steps
        foreach (AlgoStep step in algoSteps)
        {
            // Info on operation this step
            if (algoType == AlgoTypes.Greedy)
                algoStepsInfo += $"Operation: {((GreedyAlgoStep)step).Operation}, Items: ";
            else if(algoType == AlgoTypes.Dynamic)
                algoStepsInfo += $"Operation: {((DynamicProgAlgoStep)step).Operation}, Items: ";

            // Info on Items this step
            Item[] stepItems = (Item[])step.Values;
            algoStepsInfo += GetItemsInfoString(stepItems);

            // Don't put spacer after last item
            if (step != _algoSteps[_algoSteps.Length - 1])
                algoStepsInfo += " | ";

            algoStepsInfo += "\n";
        }
        return algoStepsInfo;
    }

    /// <summary>
    /// Returns Info String of Item Array
    /// </summary> 
    /// <param name="items"></param>
    /// <returns></returns>
    private string GetItemsInfoString(Item[] items)
    {
        string info = string.Empty;
        foreach (Item item in items)
        {
            info += $"{item.Name}, V-W-Ratio: {(double)((double)item.Value / (double)item.Weight)}";
            // Don't put spacer after last item
            if (item != items[items.Length - 1]) 
                info += "\t";
        }
        return info;
    }

    private void LogAlgoInfo(string algoName, string itemsInfo, string selectedItemsInfo, string algoStepsInfo)
    {
        Debug.Log($"#### {algoName.ToUpper()} ####");
        Debug.Log($"Items: {itemsInfo}");
        Debug.Log($"Selected Items: {selectedItemsInfo}");
        Debug.Log($"Maximum Weight: {_maxWeight}");
        Debug.Log($"Achieved Weight: {_selectedItemsTotalWeight.ToString()}");
        Debug.Log($"Achieved Value: {_selectedItemsTotalValue.ToString()}");
        Debug.Log($"Algo Steps: {algoStepsInfo}");
    }


}
