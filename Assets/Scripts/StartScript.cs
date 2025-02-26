using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{

    private void Start(){
        Time.timeScale = 0.0f;
    }
    public void StartGame()
    {
        Time.timeScale = 1.0f;
        transform.parent.gameObject.SetActive(false);
        string canvasName = "GameUICanvas";
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
