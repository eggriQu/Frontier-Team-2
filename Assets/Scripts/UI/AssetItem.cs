using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Placement;

public class AssetItem : MonoBehaviour
{
    [Header("Build Asset")]
    public string assetName;
    public GameObject buildPrefab;

    [Header("UI References")]
    public Button button;
    public TMP_Text itemLabel;
    public Image previewImage;

    [Header("Placement")]
    //public PlacementController placementController;
    [SerializeField] private BuildingPlacer placementController;
    

    private void Awake()
    {
        if (itemLabel != null)
        {
            itemLabel.text = assetName;
        }

        if (button != null)
        {
            button.onClick.AddListener(SelectAsset);
        }
    }

    //Asset selection
    private void SelectAsset()
    {
        if (placementController == null)
        {
            Debug.LogWarning(
                "Placement Controller is not assigned on " + assetName
            );
            return;
        }

        if (buildPrefab == null)
        {
            Debug.LogWarning(
                "Build Prefab is not assigned on " + assetName
            );
            return;
        }

        //placementController.SelectObject(buildPrefab);
        placementController.SetPlaceable(buildPrefab);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(SelectAsset);
        }
    }
}
