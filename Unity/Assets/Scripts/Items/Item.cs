using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Inventory/Item")]
public class Item : ScriptableObject
{
    public string Name = "New Item";
    public GameObject Model;
    public int Value;
    public int Weight;
}
