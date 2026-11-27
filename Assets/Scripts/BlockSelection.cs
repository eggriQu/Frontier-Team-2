using System.Collections.Generic;
using UnityEngine;

public class BlockSelection : MonoBehaviour
{
    [Header("Object Selection")]
    [SerializeField] private float doubleClickTime = 0.3f;
    [SerializeField] private Material blockSelectionMaterial;

    private BlockMarker selectedObject;
    private BlockMarker lastClickedObject;
    private float lastClickTime;

    private readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    //Checks whether an object has been selected
    public BlockMarker HandleObjectSelection(Camera buildCamera, float rayDistance)
    {
        if (buildCamera == null ||
            !Physics.Raycast(buildCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, rayDistance))
        {
            ClearSelection();
            return null;
        }

        BlockMarker clickedObject = hit.collider.GetComponentInParent<BlockMarker>();

        if (clickedObject == null)
        {
            ClearSelection();
            return null;
        }

        //Checks whether the same object has been double-clicked on
        if (clickedObject == lastClickedObject && Time.time - lastClickTime <= doubleClickTime)
        {
            SetSelectedObject(clickedObject);
            lastClickedObject = null;
            return clickedObject;
        }

        SetSelectedObject(clickedObject);
        lastClickedObject = clickedObject;
        lastClickTime = Time.time;

        Debug.Log("Selected placed object: " + clickedObject.name);
        return null;
    }

    //Removes the selected placed object
    public void RemoveSelectedObject()
    {
        if (selectedObject == null) return;

        Debug.Log("Removed placed object: " + selectedObject.name);
        RestoreOriginalMaterials();
        Destroy(selectedObject.gameObject);
        ClearSelection();
    }

    //Updates the selected object
    public void SetSelectedObject(BlockMarker marker)
    {
        RestoreOriginalMaterials();

        selectedObject = marker;

        if (selectedObject != null)
        {
            //Applies the yellow selection material
            if (blockSelectionMaterial != null)
            {
                foreach (Renderer renderer in selectedObject.GetComponentsInChildren<Renderer>())
                {
                    originalMaterials[renderer] = renderer.sharedMaterials;

                    Material[] materials = renderer.sharedMaterials;

                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = blockSelectionMaterial;

                    renderer.sharedMaterials = materials;
                }
            }
        }

        lastClickedObject = null;
    }

    //Restores the original object materials
    private void RestoreOriginalMaterials()
    {
        foreach (KeyValuePair<Renderer, Material[]> entry in originalMaterials)
        {
            if (entry.Key != null)
                entry.Key.sharedMaterials = entry.Value;
        }

        originalMaterials.Clear();
    }

    //Clears the selected object
    public void ClearSelection()
    {
        RestoreOriginalMaterials();
        selectedObject = lastClickedObject = null;
    }
}
