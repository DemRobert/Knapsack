using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    public int value = 1;
    public int weight = 1;

    public void Set(Item item)
    {
        value = item.Value;
        weight = item.Weight;
    }
}
