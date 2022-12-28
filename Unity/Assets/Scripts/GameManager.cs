using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EventSystem EventSystem;
    public GameModes GameMode { get; private set; }
    public AlgoTypes AlgoType { get; private set; }

	private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // After Scene is loaded
        GameMode = MainMenuManager.SelectedGameMode;
        AlgoType = MainMenuManager.SelectedAlgorithm;
    }
}
public enum GameModes
{
    UNDEFINED,
    LEARNING,
    PRACTICE
}

public enum AlgoTypes
{
    Tutorial,
    Greedy,
    Dynamic
}

