using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicProgAlgoStep : AlgoStep
{
    // Mögliche Operationen
    public enum DynamicAlgoOperations
    {
        NEW_CELL,

        COMPARE_WEIGHT01,
        COMPARE_WEIGHT02,
        COMPARE_WEIGHT03,

        HIGHLIGHT_BEST_OPTION,
        COMPARE_VALUES,

        END
    }

    public DynamicAlgoOperations Operation { get; private set; }
    public int CurCapacity;
    public int CurItemIndex;

    public DynamicProgAlgoStep(DynamicAlgoOperations operation, object ding, int curCapacity, int curItemIndex)
    {
        Operation = operation;
        Values = ding;

        CurCapacity = curCapacity;
        CurItemIndex = curItemIndex;
	}
}
