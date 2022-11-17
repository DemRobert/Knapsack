using UnityEngine;

public class ItemProperties : MonoBehaviour
{
    public int value = 1;
    public int weight = 1;

    public void Set(GameObject other)
    {
        ItemProperties otherProps = other.GetComponent<ItemProperties>();
        value = otherProps.value;
        weight = otherProps.weight;
    }
}
