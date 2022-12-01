using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public EventSystem EventSystem;

	private void Awake()
    {
        Instance = this;
    }
}
