using UnityEngine;

public class ItemController : MonoBehaviour
{
    private ItemProperties m_ItemProperties;

    private void Start()
    {
        m_ItemProperties = GetComponent<ItemProperties>();
    }
}
