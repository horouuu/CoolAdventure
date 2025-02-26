using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{


    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        //transform.parent.gameObject.SetActive(false);
        string canvasName = "PauseCanvas";
        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject.name == canvasName)
            {
                canvas.gameObject.SetActive(true);
                return;
            }
        }
        
        Debug.LogWarning("Canvas named '" + canvasName + "' was not found!");
    }
}
