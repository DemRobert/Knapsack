using System.Collections;
using System.Collections.Generic;
using UnityEngineInternal;

public class GreedyAlgoStep : AlgoStep
{
    public enum GreedyAlgoOperations { Starting, Sorting, Selecting, Ending }
    public GreedyAlgoOperations Operation { get; private set; }
    public GreedyAlgoStep(GreedyAlgoOperations operation, object values)
    {
        this.Operation = operation;
        this.Values = values;        
    }

    public class SelectionValues
    {
        public int CurrentWeight { get; private set; }
        public int MaxWeight { get; private set; }
        public ItemProperties CurrentItem { get; private set; }
        public ItemProperties[] SelectedItems { get; private set; }

        public SelectionValues(int currentWeight, int maxWeight, ItemProperties currentItem, ItemProperties[] selectedItems)
        {
            CurrentWeight = currentWeight;
            MaxWeight = maxWeight;
            CurrentItem = currentItem;
            SelectedItems = selectedItems;
        }
    }
        

}
