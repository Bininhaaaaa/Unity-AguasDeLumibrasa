using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    public GameObject uiCamera;
    public GameObject characterSelectionUi;
    public GameObject gameplayUI;
    public GameObject fpsCharacter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
            // If not, set instance to this
            Instance = this;
            // Make sure this object is not destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If instance already exists, destroy this
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uiCamera.SetActive(true);
        fpsCharacter.SetActive(false);
        gameplayUI.SetActive(false);
        characterSelectionUi.SetActive(true);
        UI_GameTimer.OnTimerFinished += StartGame;
    }

    public void StartGame()
    {
        uiCamera.SetActive(false);
        characterSelectionUi.SetActive(false);
        gameplayUI.SetActive(true);
        fpsCharacter.SetActive(true);
        // start ui transition, switch cameras, start first person character
        Debug.Log("Game Started!");
    }
}

