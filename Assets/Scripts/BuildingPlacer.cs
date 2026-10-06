using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    // test var
    public BasePlaceable[] placeables;      //  Array of placeable objects
    public BasePlaceable testPlaceable;     //  single placeable object for testing purposes

    InputAction pointerAction;
    GameObject currentObject;       // current object being placed

    private Quaternion currentRotation;     // its current rotation, stored so it doesn't reset when placing a new object

    private void Start()
    {
        pointerAction = InputSystem.actions.FindAction("Pointer");
        currentObject = GameObject.Instantiate(testPlaceable.Prefab);
    }

    private void Update()
    {
        //OnClick(clickAction.ReadValue<bool>());
        OnPointer(pointerAction.ReadValue<Vector2>());
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnMouseDown();
        }

        if (Keyboard.current != null & Keyboard.current.rKey.wasPressedThisFrame)
        {
            OnRotatePressed();
        }
    }

    public void OnPointer(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hit))
        {
            Vector3 rayHit = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

            currentObject.transform.position = rayHit;
            currentObject.transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        }
    }

    private void OnMouseDown()
    {
        if (gameManager.Money < testPlaceable.Cost)
        {
            Debug.Log("Not enough money!");
            return;
        }
        // check for collision here
            
        currentObject = GameObject.Instantiate(testPlaceable.Prefab);
        
        gameManager.Money -= testPlaceable.Cost;
        //gameManager.Income += testPlaceable.Income;
    }

    private void OnRotatePressed()
    {
        currentRotation.y += 90f;
        if (currentRotation.y >= 360f)
            currentRotation.y = 0f;
    }
}