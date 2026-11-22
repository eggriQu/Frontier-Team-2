using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssetItem : MonoBehaviour
{
    [Header("Asset Reference")]
    public string assetName;
    public GameObject buildPrefab;

    [Header("UI Reference")]
    public Button button;
    public TMP_Text itemLabel;
    public Image previewImage;

    [Header("Placement Controller")]
    public PlacementController placementController;

    //Assigns the name to the asset and acts as a button to select it
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

    //Select the asset that you want to place in the game
    private void SelectAsset()
    {
        if (placementController == null)
        {
            return;
        }

        if (buildPrefab == null)
        {
            return;
        }

        placementController.SelectObject(buildPrefab);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(SelectAsset);
        }
    }
}
