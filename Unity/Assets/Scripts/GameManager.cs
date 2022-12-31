using UnityEngine;
using UnityEngine.EventSystems;

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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

	public EventSystem EventSystem;
    public GameModes GameMode { get; private set; }
    public AlgoTypes AlgoType { get; set; }

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

    // Für Commit
}