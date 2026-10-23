using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject buildCatalogue;

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

    public void CloseBuildCatalogue()
    {
        if (buildCatalogue != null)
        {
            buildCatalogue.SetActive(false);
        }
    }
}
