using System.Collections.Generic;

public abstract class AlgoBehaviour
{
    protected int _maximumWeight;
    protected List<AlgoStep> _steps = new List<AlgoStep>();
    public abstract void StartAlgorithm(object[] items, int maxWeight, out object[] selectedItems, out AlgoStep[] steps, out int selectedItemsTotalValue, out int selectedItemsTotalWeight);

    public List<AlgoStep> GetSteps()
    {
        return _steps;
    }
}
