using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuOpener : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Checks if the pause menu is open
            if (SceneManager.GetSceneByName("PauseMenu").isLoaded)
            {
                //Closes the pause menu
                SceneManager.UnloadSceneAsync("PauseMenu");
                Time.timeScale = 1f;
            }
            else
            {
                //Pauses the game and opens the pause menu
                Time.timeScale = 0f;
                SceneManager.LoadScene
                (
                    "PauseMenu",
                    LoadSceneMode.Additive
                );
            }
        }
    }
}
