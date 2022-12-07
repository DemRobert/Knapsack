using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
	public enum GameModes
    {
        UNDEFINED,

        LEARNING,
        PRACTICE,
        DUEL
    }

	public static GameManager Instance;

    public EventSystem EventSystem;
    public GameModes GameMode; 

	private void Awake()
    {
        Instance = this;
    }
}
