using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicProgAlgoStep : AlgoStep
{
    // Mögliche Operationen
    public enum DynamicAlgoOperations { }
    public DynamicAlgoOperations Operation { get; private set; }
    public DynamicProgAlgoStep(DynamicAlgoOperations operation, object values)
    {
        this.Operation = operation;
        this.Values = values;
    }
}
