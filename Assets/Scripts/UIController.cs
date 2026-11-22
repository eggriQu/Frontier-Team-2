using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject buildCatalogue;

    //Opens the build tab
    public void OpenBuildCatalogue()
    {
        if (buildCatalogue != null)
        {
            buildCatalogue.SetActive(true);
        }
    }

    //Closes the build tab
    public void CloseBuildCatalogue()
    {
        if (buildCatalogue != null)
        {
            buildCatalogue.SetActive(false);
        }
    }
}
