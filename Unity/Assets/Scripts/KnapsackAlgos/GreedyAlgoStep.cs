using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;

public class GreedyAlgoStep : AlgoStep
{
    public enum GreedyAlgoOperations { Starting, Sorting, Comparing, Swapping, Ending }
    public GreedyAlgoOperations Operation { get; private set; }
    public GreedyAlgoStep(GreedyAlgoOperations operation, object values)
    {
        this.Operation = operation;
        this.Values = values;        
    }
}
