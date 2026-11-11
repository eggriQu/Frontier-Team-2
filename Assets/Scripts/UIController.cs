using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject buildCatalogue;

    // Open the build tab
    public void OpenBuildCatalogue()
    {
        if (buildCatalogue != null)
        {
            buildCatalogue.SetActive(true);
        }
        else
        {
            Debug.LogWarning("BuildCatalogue has not been assigned.");
        }
    }

    //Close the build tab
    public void CloseBuildCatalogue()
    {
        if (buildCatalogue != null)
        {
            buildCatalogue.SetActive(false);
        }
    }
}
