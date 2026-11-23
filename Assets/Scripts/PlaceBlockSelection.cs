using UnityEngine;

public class PlaceBlockSelection : MonoBehaviour
{
    [Header("Object Selection")]
    [SerializeField] private float doubleClickTime = 0.3f;
    [SerializeField] private Material outlineMaterial;

    private PlaceBlockMarker selectedObject;
    private PlaceBlockMarker lastClickedObject;
    private float lastClickTime;

    //Checks whether an object has been selected
    public PlaceBlockMarker HandleObjectSelection(Camera buildCamera, float rayDistance)
    {
        if (buildCamera == null ||
            !Physics.Raycast(buildCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, rayDistance))
        {
            ClearSelection();
            return null;
        }

        PlaceBlockMarker clickedObject = hit.collider.GetComponentInParent<PlaceBlockMarker>();

        if (clickedObject == null)
        {
            ClearSelection();
            return null;
        }

        //Checks whether the same object has been double-clicked
        if (clickedObject == lastClickedObject && Time.time - lastClickTime <= doubleClickTime)
        {
            selectedObject = clickedObject;
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
        Destroy(selectedObject.gameObject);
        ClearSelection();
    }

    //Updates the selected object
    public void SetSelectedObject(PlaceBlockMarker marker)
    {
        if (selectedObject != null)
        {
            ObjectOutline previous = selectedObject.GetComponent<ObjectOutline>();
            if (previous != null)
                previous.ShowOutline(false);
        }

        selectedObject = marker;

        if (selectedObject != null)
        {
            ObjectOutline outline = selectedObject.GetComponent<ObjectOutline>();

            if (outline == null)
                outline = selectedObject.gameObject.AddComponent<ObjectOutline>();

            outline.SetMaterial(outlineMaterial);
            outline.ShowOutline(true);
        }

        lastClickedObject = null;
    }

    //Clears the selected object
    public void ClearSelection()
    {
        if (selectedObject != null)
        {
            ObjectOutline outline = selectedObject.GetComponent<ObjectOutline>();

            if (outline != null)
                outline.ShowOutline(false);
        }

        selectedObject = lastClickedObject = null;
    }
}
