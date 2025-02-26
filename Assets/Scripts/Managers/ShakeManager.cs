using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Singleton manager for accessing the camera shake extension from anywhere,
/// including animation events and other systems
/// </summary>
public class ShakeManager : MonoBehaviour
{
    private static ShakeManager _instance;
    public static ShakeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance
                _instance = FindAnyObjectByType<ShakeManager>();
                
                // If no instance exists, create a new one
                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("ShakeManager");
                    _instance = managerObject.AddComponent<ShakeManager>();
                    DontDestroyOnLoad(managerObject);
                }
            }
            return _instance;
        }
    }

    // Reference to the active virtual camera
    private CinemachineCamera _activeCamera;
    private CinemachineShakeExtension _shakeExtension;
    private bool _initialized;

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        // Try to auto-initialize
        AutoInitialize();
    }
    
    private void Start()
    {
        // Try again in Start in case all cameras weren't ready in Awake
        if (!_initialized)
        {
            AutoInitialize();
        }
    }
    
    /// <summary>
    /// Automatically find and initialize with the first virtual camera in the scene
    /// </summary>
    private void AutoInitialize()
    {
        // Find the first virtual camera in the scene
        if (_activeCamera == null)
        {
            _activeCamera = FindAnyObjectByType<CinemachineCamera>();
        }
        
        if (_activeCamera != null)
        {
            // Check if the camera already has our extension
            _shakeExtension = _activeCamera.GetComponent<CinemachineShakeExtension>();
            
            // If not, add it
            if (_shakeExtension == null)
            {
                _shakeExtension = _activeCamera.gameObject.AddComponent<CinemachineShakeExtension>();
            }
            
            _initialized = true;
        }
    }

    /// <summary>
    /// Add trauma to cause camera shake (0-1 scale)
    /// </summary>
    public void ShakeCamera(float traumaAmount)
    {
        EnsureInitialized();
        if (_shakeExtension != null)
        {
            _shakeExtension.AddShake(traumaAmount);
        }
        else
        {
            Debug.LogWarning("ShakeManager: Camera not initialized. Call Initialize() first.");
        }
    }

    /// <summary>
    /// Make sure we're initialized before trying to shake the camera
    /// </summary>
    private void EnsureInitialized()
    {
        if (!_initialized)
        {
            AutoInitialize();
            
            if (!_initialized)
            {
                Debug.LogWarning("ShakeManager: No CinemachineVirtualCamera found in the scene.");
            }
        }
    }

    #region Shake Methods

    /// <summary>
    /// Trigger a camera shake with the specified intensity and automatically calculated falloff
    /// </summary>
    /// <param name="intensity">Shake intensity (0-1)</param>
    /// <param name="duration">How long the shake should last</param>
    public void ShakeCamera(float intensity, float duration)
    {
        if (_shakeExtension != null)
        {
            // Set decay speed based on duration to match desired time
            _shakeExtension.DecaySpeed = 1.0f / duration;
            _shakeExtension.SetShake(intensity);
        }
        else
        {
            Debug.LogWarning("ShakeManager: Camera not initialized. Call Initialize() first.");
        }
    }

    /// <summary>
    /// Stop any active camera shake
    /// </summary>
    public void StopShake()
    {
        if (_shakeExtension != null)
        {
            _shakeExtension.StopShake();
        }
    }
    #endregion
    
    #region Static Methods for Animation Events
    
    /// <summary>
    /// Static method to trigger small camera shake - perfect for animation events
    /// </summary>
    public static void SmallShake()
    {
        Instance.ShakeCamera(0.2f, 0.2f);
    }
    
    /// <summary>
    /// Static method to trigger medium camera shake - perfect for animation events
    /// </summary>
    public static void MediumShake()
    {
        Instance.ShakeCamera(0.5f, 0.3f);
    }
    
    /// <summary>
    /// Static method to trigger large camera shake - perfect for animation events
    /// </summary>
    public static void LargeShake()
    {
        Instance.ShakeCamera(0.8f, 0.5f);
    }
    
    /// <summary>
    /// Static method to trigger camera shake with intensity parameter - can be called from anywhere
    /// </summary>
    public static void Shake(float intensity)
    {
        Instance.ShakeCamera(intensity, intensity * 0.5f); // Duration scales with intensity
    }

    /// <summary>
    /// Static method to trigger camera shake with both intensity and duration parameters - can be called from anywhere
    /// </summary>
    public static void Shake(float intensity, float duration)
    {
        Instance.ShakeCamera(intensity, duration);
    }
    
    #endregion

    #region Built-in Testing Functions
    
    [Header("Test Input Settings")]
    [Tooltip("Enable keyboard input testing")]
    public bool enableTestInput = true;
    
    [Tooltip("Key to trigger small shake")]
    public KeyCode smallShakeKey = KeyCode.Alpha1;
    
    [Tooltip("Key to trigger medium shake")]
    public KeyCode mediumShakeKey = KeyCode.Alpha2;
    
    [Tooltip("Key to trigger large shake")]
    public KeyCode largeShakeKey = KeyCode.Alpha3;
    
    private void Update()
    {
        if (!enableTestInput) return;
        
        // Test key input for shakes
        if (Input.GetKeyDown(smallShakeKey))
        {
            SmallShake();
            Debug.Log("Small shake triggered");
        }
        
        if (Input.GetKeyDown(mediumShakeKey))
        {
            MediumShake();
            Debug.Log("Medium shake triggered");
        }
        
        if (Input.GetKeyDown(largeShakeKey))
        {
            LargeShake();
            Debug.Log("Large shake triggered");
        }
    }
    
    #endregion
}