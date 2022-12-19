using System.Collections.Generic;

public abstract class AlgoBehaviour
{
    protected int _maximumWeight;
    protected List<AlgoStep> _steps = new List<AlgoStep>();
    public abstract void StartAlgorithm(Item[] items, int maxWeight, out Item[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight);
}
