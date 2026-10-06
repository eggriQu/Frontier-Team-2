using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GenericPlaceable[] genericPlaceables;
    [SerializeField] private EconomyPlaceable[] economyPlaceables;

    InputAction pointerAction;
    GameObject currentObject;

    private void Start()
    {
        pointerAction = InputSystem.actions.FindAction("Pointer");
        currentObject = GameObject.Instantiate(genericPlaceables[1].Prefab);
    }

    private void Update()
    {
        //OnClick(clickAction.ReadValue<bool>());
        OnPointer(pointerAction.ReadValue<Vector2>());
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnMouseDown();
        }
    }

    public void OnPointer(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 rayHit = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

            currentObject.transform.position = rayHit + genericPlaceables[1].TransformOffsets;
        }
    }

    private void OnMouseDown()
    {
        if (gameManager.Money < genericPlaceables[1].Cost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        // check for collision here
            
        currentObject = GameObject.Instantiate(genericPlaceables[1].Prefab);
        gameManager.Money -= genericPlaceables[1].Cost;
        //gameManager.Income += genericPlaceables[1].Income;
    }
}