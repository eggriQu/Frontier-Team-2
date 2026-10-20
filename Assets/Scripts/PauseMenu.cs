using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    //Return back to the game
    public void BackToGame()
    {
        Time.timeScale = 1f;

        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    //Exit the game
    public void QuitGame()
    {

        Time.timeScale = 1f;

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
