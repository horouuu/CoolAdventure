using System.Collections;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject gameOverUI;

    IEnumerator WaitAndEndGame()
    {
        yield return new WaitForSeconds(3);
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
    public void GameOver()
    {
        StartCoroutine(WaitAndEndGame());
    }
}
