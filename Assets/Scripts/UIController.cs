using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject buildCatalogue;
    [SerializeField] private GameObject placementControls;
    [SerializeField] private PlacementController placementController;

    private void Start()
    {
        if (placementControls != null)
        {
            placementControls.SetActive(false);
        }
    }

    private void Update()
    {
        if (placementControls != null && placementController != null)
        {
            placementControls.SetActive(placementController.IsPlacing);
        }
    }

    //Open the build tab
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

    //Confirm the object placement
    public void ConfirmPlacement()
    {
        if (placementController != null)
        {
            placementController.ConfirmPlacement();
        }
    }

    //Cancel the object placement
    public void CancelPlacement()
    {
        if (placementController != null)
        {
            placementController.CancelPlacement();
        }

        if (placementControls != null)
        {
            placementControls.SetActive(false);
        }
    }
}
