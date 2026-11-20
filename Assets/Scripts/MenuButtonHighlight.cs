using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Highlight Settings")]
    [SerializeField] private float scaleMultiplier = 1.08f;
    [SerializeField] private float darkenMultiplier = 0.65f;

    private Button button;
    private Graphic targetGraphic;
    private Color originalColour;
    private Vector3 originalScale;

    private void Awake()
    {
        button = GetComponent<Button>();

        originalScale = transform.localScale;

        if (button != null)
        {
            targetGraphic = button.targetGraphic;
        }

        if (targetGraphic != null)
        {
            originalColour = targetGraphic.color;
        }
    }

    //Highlight the button when the mouse hovers over it
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (KeyboardShortcuts.KeyboardNavigationActive)
        {
            return;
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        ShowHighlight();
    }

    //Unhighlight the button whenever the mouse is not hovering over it 
    public void OnPointerExit(PointerEventData eventData)
    {
        if (KeyboardShortcuts.KeyboardNavigationActive)
        {
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        HideHighlight();
    }

    //Highlight the button when selecting one using the keyboard
    public void OnSelect(BaseEventData eventData)
    {
        ShowHighlight();
    }

    //Unhighlight when another button is selected
    public void OnDeselect(BaseEventData eventData)
    {
        HideHighlight();
    }

    //Darken and enlarge the button when highlighted
    private void ShowHighlight()
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = new Color(
                originalColour.r * darkenMultiplier,
                originalColour.g * darkenMultiplier,
                originalColour.b * darkenMultiplier,
                originalColour.a
            );
        }

        transform.localScale = originalScale * scaleMultiplier;
    }

    //Return the button to its original form when unhighlighted
    private void HideHighlight()
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = originalColour;
        }

        transform.localScale = originalScale;
    }
}
