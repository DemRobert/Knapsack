using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;

public class GreedyAlgoStep : AlgoStep
{
    //public int CurrentWeight { get; private set; }
    //public int MaxWeight { get; private set; }
    public int CurrentIndex { get; private set; }
    public ItemPropertiesNoUnity CurrentItem { get; private set; }
    //public ItemPropertiesNoUnity[] Items { get; private set; }
    //public ItemPropertiesNoUnity[] SelectedItems { get; private set; }
    public enum GreedyAlgoOperations
    {
        HIGHLIGHT_VALUE,
        HIGHLIGHT_WEIGHT,
        COMPUTE_VW_RATIO,
		DEHIGHLIGHT_COLUMN,

        HIGHLIGHT_ALL_VW_RATIOS,
        HIGHLIGHT_CUR_BEST_VW_RATIO,
		DRAW_CUR_BEST_ITEM_INDEX,
		DRAW_CUR_BEST_VW_RATIO,
		DEHIGHLIGHT_CUR_BEST_ALL,

        HIGHLIGHT_BEST_ITEM,
        HIGHLIGHT_CAP_AND_WEIGHT,
        CHOOSE_IF_TAKE,
        ADD_TO_TOT_VAL,
        DEHIGHLIGHT_CAP_WEIGHT
	}

    public GreedyAlgoOperations Operation { get; private set; }

	public GreedyAlgoStep(GreedyAlgoOperations operation, ItemPropertiesNoUnity curItem, int itemIndex)
    {
        Operation = operation;
        CurrentItem = curItem;
        CurrentIndex = itemIndex;
	}


	/*public GreedyAlgoStep(GreedyAlgoOperations operation, int currentWeight, int maxWeight, int currentIndex, ItemPropertiesNoUnity currentItem, ItemPropertiesNoUnity[] items, ItemPropertiesNoUnity[] selectedItems)
    {
        this.Operation = operation;
        this.CurrentWeight = currentWeight;
        this.MaxWeight = maxWeight;
        this.CurrentIndex = currentIndex;
        this.CurrentItem = currentItem;
        this.Items = items;
        this.SelectedItems = selectedItems;
    }*/
        

}
