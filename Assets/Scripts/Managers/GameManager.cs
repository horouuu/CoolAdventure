using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Player player;
    public GameConstants gameConstants;
    public GameObject gameOverUI;
    public MainCamera camera;
    public AudioSource gameOverSource;
    override public void Awake()
    {
        base.Awake();
        player = Player.instance;
        camera = MainCamera.instance;
    }


    IEnumerator WaitAndEndGame()
    {
        yield return new WaitForSeconds(3);
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
    public void GameOver()
    {
        gameOverSource.PlayOneShot(gameOverSource.clip);
        StartCoroutine(WaitAndEndGame());
    }

    public void RestartGame()
    {
        Player player = Player.instance;
        ScoreManager.instance.ResetCurrentScore();
        PlayerHealth.SetCurrentHp(gameConstants.maxHp);
        player.transform.SetPositionAndRotation(gameConstants.startingPosition, player.transform.rotation);
        player.combat.Revive();
        player.movement.climbPower = false;
        player.animator.Rebind();
        player.animator.Update(0f);
        player.movement.isWallGripping = false;
        player.movement.isWallSliding = false;
        player.movement.isWallJumping = false;
        gameOverUI.SetActive(false);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("gauntlet");
    }

    public void LoadScene(string sceneName, LoadSceneMode loadSceneMode, Vector3 teleportLocation)
    {
        FollowCamera fCam = FollowCamera.instance;
        camera = MainCamera.instance;
        StartCoroutine(FCamTempDisable(fCam));
        player = Player.instance;

        SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        player.transform.SetPositionAndRotation(teleportLocation, player.transform.rotation);
        camera.transform.SetPositionAndRotation(teleportLocation + new Vector3(0, 5, 0), camera.transform.rotation);
        fCam.transform.SetPositionAndRotation(teleportLocation + new Vector3(0, 5, 0), camera.transform.rotation);
    }

    IEnumerator FCamTempDisable(FollowCamera fCam)
    {
        fCam.enabled = false;
        yield return new WaitForSeconds(0.05f);
        fCam.enabled = true;
    }
}
