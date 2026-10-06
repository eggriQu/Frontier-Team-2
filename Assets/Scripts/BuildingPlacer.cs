using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private Placeable[] placeables;
    //[SerializeField] private GameObject testPlaceable;

    InputAction clickAction;
    InputAction pointerAction;
    GameObject currentObject;

    private void OnEnable()
    {
        //clickAction.Enable();
        clickAction.performed += OnMouseDown;
    }

    private void Start()
    {
        clickAction = InputSystem.actions.FindAction("Click");
        pointerAction = InputSystem.actions.FindAction("Pointer");
        currentObject = GameObject.Instantiate(placeables[0].prefab);
    }

    private void Update()
    {
        //OnClick(clickAction.ReadValue<bool>());
        OnPointer(pointerAction.ReadValue<Vector2>());
    }

    public void OnPointer(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.point);
            Vector3 rayHit = new(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));

            currentObject.transform.position = rayHit + placeables[0].transformOffsets;
        }
    }

    private void OnMouseDown(InputAction.CallbackContext context)
    {
        currentObject = GameObject.Instantiate(placeables[0].prefab);
    }
}
