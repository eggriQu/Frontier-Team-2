using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardShortcuts : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button[] buttons;

    private int selectedIndex = 0;

    public static bool KeyboardNavigationActive { get; private set; }

    private Vector3 lastMousePosition;

    private void Start()
    {
        KeyboardNavigationActive = false;
        lastMousePosition = Input.mousePosition;

        //Disables the default navigation so that we can control the buttons manually
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.None;
                button.navigation = navigation;
            }
        }

        SelectButton(0);
    }

    private void Update()
    {
        if (EventSystem.current == null || buttons.Length == 0)
        {
            return;
        }

        //Detects when the mouse moves
        if ((Input.mousePosition - lastMousePosition).sqrMagnitude > 4f)
        {
            KeyboardNavigationActive = false;
        }

        lastMousePosition = Input.mousePosition;

        //Keep track of the currently selected button
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null &&
                EventSystem.current.currentSelectedGameObject == buttons[i].gameObject)
            {
                selectedIndex = i;
                break;
            }
        }

        //Move selection up
        if (Input.GetKeyDown(KeyCode.W))
        {
            KeyboardNavigationActive = true;
            MoveSelection(-1);
        }

        //Move selection down
        if (Input.GetKeyDown(KeyCode.S))
        {
            KeyboardNavigationActive = true;
            MoveSelection(1);
        }

        //Select the highlighted button
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (buttons[selectedIndex] != null &&
                buttons[selectedIndex].interactable)
            {
                buttons[selectedIndex].onClick.Invoke();
            }
        }
    }

    //Move through the buttons
    private void MoveSelection(int direction)
    {
        int nextIndex = selectedIndex;

        for (int i = 0; i < buttons.Length; i++)
        {
            nextIndex += direction;

            if (nextIndex < 0)
            {
                nextIndex = buttons.Length - 1;
            }
            else if (nextIndex >= buttons.Length)
            {
                nextIndex = 0;
            }

            if (buttons[nextIndex] != null &&
                buttons[nextIndex].interactable)
            {
                SelectButton(nextIndex);
                return;
            }
        }
    }

    //Select a button and highlight it
    private void SelectButton(int index)
    {
        if (buttons.Length == 0 || EventSystem.current == null)
        {
            return;
        }

        if (buttons[index] == null || !buttons[index].interactable)
        {
            MoveSelection(1);
            return;
        }

        selectedIndex = index;

        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
    }
}
