using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    //Opens the OutdoorsScene when Play is pressed
    public void Play()
    {
        SceneManager.LoadScene("OutdoorsScene");
    }

    //Exits the game when Quit is pressed
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
