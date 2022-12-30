using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;

public class GreedyAlgoStep : AlgoStep
{
    public int CurrentWeight { get; private set; }
    public int MaxWeight { get; private set; }
    public int CurrentIndex { get; private set; }
    public ItemProperties CurrentItem { get; private set; }
    public ItemProperties[] Items { get; private set; }
    public ItemProperties[] SelectedItems { get; private set; }
    public enum GreedyAlgoOperations { Starting, Sorting, Selecting, Ending }
    public GreedyAlgoOperations Operation { get; private set; }

    public GreedyAlgoStep(GreedyAlgoOperations operation, int currentWeight, int maxWeight, int currentIndex, ItemProperties currentItem, ItemProperties[] items, ItemProperties[] selectedItems)
    {
        this.Operation = operation;
        this.CurrentWeight = currentWeight;
        this.MaxWeight = maxWeight;
        this.CurrentIndex = currentIndex;
        this.CurrentItem = currentItem;
        this.Items = items;
        this.SelectedItems = selectedItems;
    }
        

}
