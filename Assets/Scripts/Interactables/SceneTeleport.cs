using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour, IInteractable
{
    public string sceneName;
    public Vector3 teleportLocation;
    public GameManager gameManager;
    public bool CanInteract()
    {
        return true;
    }

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Interact()
    {
        gameManager.LoadScene(sceneName, LoadSceneMode.Single, teleportLocation);
    }

    void Update()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}
