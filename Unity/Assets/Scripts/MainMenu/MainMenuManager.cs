using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainMenu;
    [SerializeField]
    private GameObject _gameModeSelectionMenu;
    [SerializeField]
    private GameObject _settingsMenu;
    [SerializeField]
    private GameObject _learningModeMenu;
    [SerializeField]
    private GameObject _pracitceModeMenu;
    [SerializeField]
    private GameObject _duelModeMenu;
    [SerializeField]
    private GameObject _duelModeAlgoSelectionMenu;

    private GameModes _selectedGameMode;
    private Algorithms _selectedAlgorithm;

    private enum GameModes { LearningMode, PracticeMode, DuelMode }

    private enum Algorithms { Tutorial, GreedyAlgo, DynamicProgAlgo }



    // Start is called before the first frame update
    void Start()
    {
        if (_mainMenu == null)
            Debug.LogError("MAIN MENU NOT ASSIGNED");
        if (_gameModeSelectionMenu == null)        
            Debug.LogError("GAMEMODE SELECTION MENU NOT ASSIGNED");
        if (_settingsMenu == null)
            Debug.LogError("SETTINGS MENU NOT ASSIGNED");
        if (_learningModeMenu == null)
            Debug.LogError("LEARNING MODE MENU NOT ASSIGNED");
        if (_pracitceModeMenu == null)
            Debug.LogError("PRACTICE MODE MENU NOT ASSIGNED");
        if (_duelModeMenu == null)
            Debug.LogError("DUEL MODE MENU NOT ASSIGNED");
        if (_duelModeAlgoSelectionMenu == null)
            Debug.LogError("DUEL MODE ALGO SELECTION MENU NOT ASSIGNED");

        // Hide SettingsMenu and GameModeSelectionMenu on Start
        if (_mainMenu != null && _gameModeSelectionMenu != null && _settingsMenu != null && 
            _learningModeMenu != null && _pracitceModeMenu != null && _duelModeMenu != null && 
            _duelModeAlgoSelectionMenu != null) 
        {
            _gameModeSelectionMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _learningModeMenu.SetActive(false);
            _pracitceModeMenu.SetActive(false);
            _duelModeMenu.SetActive(false);
            _duelModeAlgoSelectionMenu.SetActive(false);
        }
    }

    public void OnButtonPlay_Click()
    {
        _mainMenu.SetActive(false);
        _gameModeSelectionMenu.SetActive(true);
    }

    public void OnButtonSettings_Click()
    {
        _mainMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    public void OnButtonExit_Click()
    {
        Application.Quit();
    }

    public void OnButtonPlayLearningMode_Click()
    {
        SelectGameMode(GameModes.LearningMode);
        _gameModeSelectionMenu.SetActive(false);
        _learningModeMenu.SetActive(true);
    }
    
    public void OnButtonPlayPracticeMode_Click()
    {
        SelectGameMode(GameModes.PracticeMode);
        _gameModeSelectionMenu.SetActive(false);
        _pracitceModeMenu.SetActive(true);
    }
    
    public void OnButtonPlayDuelMode_Click()
    {
        SelectGameMode(GameModes.DuelMode);
        _gameModeSelectionMenu.SetActive(false);
        _duelModeMenu.SetActive(true);
    }

    public void GameModeSelection_OnButtonBack_Click()
    {
        _mainMenu.SetActive(true);
        _gameModeSelectionMenu.SetActive(false);
    }

    public void Settings_OnButtonBack_Click()
    {
        _mainMenu.SetActive(true);
        _settingsMenu.SetActive(false);
    }

    public void LearningMode_OnButtonBack_Click()
    {
        _gameModeSelectionMenu.SetActive(true);
        _learningModeMenu.SetActive(false);
    }

    public void PracticeMode_OnButtonBack_Click()
    {
        _gameModeSelectionMenu.SetActive(true);
        _pracitceModeMenu.SetActive(false);
    }

    public void DuelMode_OnButtonBack_Click()
    {
        _gameModeSelectionMenu.SetActive(true);
        _duelModeMenu.SetActive(false);
    }
    public void DuelModeAlgoSelection_OnButtonBack_Click()
    {
        _duelModeMenu.SetActive(true);
        _duelModeAlgoSelectionMenu.SetActive(false);
    }

    public void DuelMode_Play1vs1_Click()
    {
        _duelModeMenu.SetActive(false);
        _duelModeAlgoSelectionMenu.SetActive(true);
    }

    public void DuelMode_Play1vs1vs1_Click()
    {
        Debug.Log("1 vs 1 vs 1 SELECTED");
    }

    public void SelectTutorial()
    {
        SelectAlgorithm(Algorithms.Tutorial);
    }

    public void SelectGreedyAlgorithm()
    {
        SelectAlgorithm(Algorithms.GreedyAlgo);
    }

    public void SelectDynamicProgAlgorithm()
    {
        SelectAlgorithm(Algorithms.DynamicProgAlgo);
    }


    private void SelectGameMode(GameModes gameMode)
    {
        _selectedGameMode = gameMode;
        Debug.Log($"GAMEMODE SELECTED: {_selectedGameMode.ToString()}");
    }

    private void SelectAlgorithm(Algorithms algorithm)
    {
        _selectedAlgorithm = algorithm;
        Debug.Log($"ALGORITHM SELECTED: {_selectedAlgorithm.ToString()}");
    }
}
