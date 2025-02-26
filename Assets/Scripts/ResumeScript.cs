using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeScript : MonoBehaviour
{

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        transform.parent.gameObject.SetActive(false);
        //string canvasName = "GameUICanvas";
    }
}
