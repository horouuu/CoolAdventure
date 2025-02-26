using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class GameData
{
    public string sceneName = ""; // Default to empty string
    public int score = 0;
    public int highScore = 0; // Persistent high score
    public int playerHealth = 3; // Set a default value
    public Vector3 playerPosition = Vector3.zero;
    public bool climbPower = false;
}


public class SaveSystem : MonoBehaviour
{
    [Serializable]
    private class HighScoreData
    {
        public int highScore;
    }

    public static SaveSystem Instance { get; private set; }
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");
    private static string HighScoreSavePath => Path.Combine(Application.persistentDataPath, "highscore.json");
    
    public static GameData CurrentData { get; private set; } = new GameData();

    [Header("Save Key Settings")]
    [Tooltip("Key to press for saving the game")]
    public KeyCode saveKey = KeyCode.F5;
    
    [Tooltip("Key to press for loading the game")]
    public KeyCode loadKey = KeyCode.F6;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadGame();
    }

    private void Start()
    {
        // Apply loaded data after all objects are initialized
        //ApplyLoadedData();
        ScoreManager.instance.highScore = CurrentData.highScore;
    }

    private void Update()
    {
        // Check for save key
        if (Input.GetKeyDown(saveKey))
        {
            Save();
            Debug.Log("Game saved using " + saveKey + " key");
        }
        
        // Check for load key
        if (Input.GetKeyDown(loadKey))
        {
            LoadGame();
            ApplyLoadedData();
            Debug.Log("Game loaded using " + loadKey + " key");
        }
    }
    
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);
        string highscoreJson = JsonUtility.ToJson(new HighScoreData { highScore = CurrentData.highScore });
        File.WriteAllText(HighScoreSavePath, highscoreJson);
        Debug.Log($"Game saved to {SavePath}");
    }
    
    public void LoadGame()
    {
        // Load game data
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game data loaded");
        }
        else
        {
            CurrentData = new GameData();
            Debug.Log("No save file found - creating new game data");
        }
        
        // Load high score separately
        if (File.Exists(HighScoreSavePath))
        {
            string highscoreJson = File.ReadAllText(HighScoreSavePath);
            HighScoreData highScoreData = JsonUtility.FromJson<HighScoreData>(highscoreJson);
            CurrentData.highScore = highScoreData.highScore;
            Debug.Log("High score loaded: " + CurrentData.highScore);
        }
    }
    
    // Call this when you want to save - e.g., checkpoints, menu, etc.
    public void Save()
    {
        // Update CurrentData with latest values from your game
        CurrentData.sceneName = SceneManager.GetActiveScene().name;
        
        // Make sure these instances exist before trying to access them
        if (ScoreManager.instance != null)
            CurrentData.score = ScoreManager.instance.score;

            // Update high score if current score is higher
            if (ScoreManager.instance.score > CurrentData.highScore)
            {
                CurrentData.highScore = ScoreManager.instance.highScore;
                Debug.Log("New high score: " + CurrentData.highScore);
            }
            
        if (Player.instance != null)
        {
            CurrentData.playerPosition = Player.instance.transform.position;
            
            if (Player.instance.health != null)
                CurrentData.playerHealth = PlayerHealth.playerCurrentHp;

            if (Player.instance.movement != null)
                CurrentData.climbPower = Player.instance.movement.climbPower;
        }
        
        SaveGame();
    }
    
    // Call this to apply loaded data
    public void ApplyLoadedData()
    {
        // Check if we need to load a different scene
        if (!string.IsNullOrEmpty(CurrentData.sceneName) && 
            CurrentData.sceneName != SceneManager.GetActiveScene().name)
        {
            // Load the saved scene
            SceneManager.LoadScene(CurrentData.sceneName);
            
            // Register a callback to finish applying data after scene load
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }
        
        // Apply data directly if we're already in the correct scene
        ApplyDataToScene();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Apply data after scene has loaded
        ApplyDataToScene();
        
        // Unregister the callback
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void ApplyDataToScene()
    {
        // Wait for one frame to ensure all objects are initialized
        StartCoroutine(ApplyDataNextFrame());
    }
    
    private System.Collections.IEnumerator ApplyDataNextFrame()
    {
        // Wait for the next frame
        yield return null;
        
        // Apply loaded data to your game objects (which should now be initialized)
        if (ScoreManager.instance != null)
            ScoreManager.instance.score = CurrentData.score;
            ScoreManager.instance.highScore = CurrentData.highScore;
            
        if (Player.instance != null)
        {
            Player.instance.transform.position = CurrentData.playerPosition;
            
            if (Player.instance.health != null)
                PlayerHealth.playerCurrentHp = CurrentData.playerHealth;
                
            if (Player.instance.movement != null)
                Player.instance.movement.climbPower = CurrentData.climbPower;
        }
        
        Debug.Log("Loaded data applied to scene");
    }
    
    // Helper method to delete save file (for testing or "New Game")
    public void DeleteSaveData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
        
        if (File.Exists(HighScoreSavePath))
        {
            File.Delete(HighScoreSavePath);
        }
        
        CurrentData = new GameData();
        Debug.Log("Save data deleted");
    }
}