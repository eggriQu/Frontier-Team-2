using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    //Opens the game when you press Play
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    //Exits the game when you press Quit
    public void Quit()
    {
        Application.Quit();

        //Stops the game when running in the Unity Editor
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
