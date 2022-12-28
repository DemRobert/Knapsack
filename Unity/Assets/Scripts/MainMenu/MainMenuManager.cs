using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //public static MainMenuManager Instance { get; private set; }

    public static GameModes SelectedGameMode { get; private set; } = GameModes.UNDEFINED;
    public static AlgoTypes SelectedAlgorithm { get; private set; } = AlgoTypes.Tutorial;

    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_gameModeSelectionMenu;
    [SerializeField] private GameObject m_settingsMenu;
    [SerializeField] private GameObject m_learningModeMenu;
    [SerializeField] private GameObject m_pracitceModeMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        if (m_mainMenu == null)
            Debug.LogError("MAIN MENU NOT ASSIGNED");
        if (m_gameModeSelectionMenu == null)        
            Debug.LogError("GAMEMODE SELECTION MENU NOT ASSIGNED");
        if (m_settingsMenu == null)
            Debug.LogError("SETTINGS MENU NOT ASSIGNED");
        if (m_learningModeMenu == null)
            Debug.LogError("LEARNING MODE MENU NOT ASSIGNED");
        if (m_pracitceModeMenu == null)
            Debug.LogError("PRACTICE MODE MENU NOT ASSIGNED");

        // Hide SettingsMenu and GameModeSelectionMenu on Start
        if (m_mainMenu != null && m_gameModeSelectionMenu != null && m_settingsMenu != null && 
            m_learningModeMenu != null && m_pracitceModeMenu != null) 
        {
            m_gameModeSelectionMenu.SetActive(false);
            m_settingsMenu.SetActive(false);
            m_learningModeMenu.SetActive(false);
            m_pracitceModeMenu.SetActive(false);
        }
    }

    public void OnButtonPlay_Click()
    {
        m_mainMenu.SetActive(false);
        m_gameModeSelectionMenu.SetActive(true);
    }

    public void OnButtonSettings_Click()
    {
        m_mainMenu.SetActive(false);
        m_settingsMenu.SetActive(true);
    }

    public void OnButtonExit_Click()
    {
        Application.Quit();
    }

    public void OnButtonPlayLearningMode_Click()
    {
        SelectGameMode(GameModes.LEARNING);
        m_gameModeSelectionMenu.SetActive(false);
        m_learningModeMenu.SetActive(true);
    }
    
    public void OnButtonPlayPracticeMode_Click()
    {
        SelectGameMode(GameModes.PRACTICE);
        m_gameModeSelectionMenu.SetActive(false);
        m_pracitceModeMenu.SetActive(true);
    }

    public void GameModeSelection_OnButtonBack_Click()
    {
        m_mainMenu.SetActive(true);
        m_gameModeSelectionMenu.SetActive(false);
    }

    public void Settings_OnButtonBack_Click()
    {
        m_mainMenu.SetActive(true);
        m_settingsMenu.SetActive(false);
    }

    public void LearningMode_OnButtonBack_Click()
    {
        m_gameModeSelectionMenu.SetActive(true);
        m_learningModeMenu.SetActive(false);
    }

    public void PracticeMode_OnButtonBack_Click()
    {
        m_gameModeSelectionMenu.SetActive(true);
        m_pracitceModeMenu.SetActive(false);
    }

    public void SelectTutorial()
    {
        SelectAlgorithm(AlgoTypes.Tutorial);
    }

    public void SelectGreedyAlgorithm()
    {
        SelectAlgorithm(AlgoTypes.Greedy);
    }

    public void SelectDynamicProgAlgorithm()
    {
        SelectAlgorithm(AlgoTypes.Dynamic);
    }


    private void SelectGameMode(GameModes gameMode)
    {
        SelectedGameMode = gameMode;
        Debug.Log($"GAMEMODE SELECTED: {SelectedGameMode.ToString()}");
    }

    private void SelectAlgorithm(AlgoTypes algorithm)
    {
        SelectedAlgorithm = algorithm;
        Debug.Log($"ALGORITHM SELECTED: {SelectedAlgorithm.ToString()}");

        // Saving Info on GameMode and AlgoType on GameManager

        // Load Scenes of GameModes
        switch (SelectedGameMode)
        {
            case GameModes.PRACTICE:                
                SceneManager.LoadScene("PracticeMode");
                break;
            case GameModes.LEARNING:
                SceneManager.LoadScene("LearningMode");
                break;
            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }
}
