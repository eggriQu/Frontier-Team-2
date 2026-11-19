using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColour = Color.yellow;
    [SerializeField] private float scaleMultiplier = 1.08f;
    [SerializeField] private Vector2 outlineThickness = new Vector2(5f, 5f);

    private Outline outline;
    private Vector3 originalScale;
    private bool isPointerOver;

    private void Awake()
    {
        originalScale = transform.localScale;

        outline = GetComponent<Outline>();

        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        outline.effectColor = highlightColour;
        outline.effectDistance = outlineThickness;
        outline.useGraphicAlpha = true;
        outline.enabled = false;
    }

    //Highlight the button when the mouse hovers over it
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        EventSystem.current.SetSelectedGameObject(gameObject);

        ShowHighlight();
    }

    //Unhighlight the button whenever the mouse is not hovering over it 
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            HideHighlight();
        }
    }

    //Highlight the button when selecting one using the keyboard
    public void OnSelect(BaseEventData eventData)
    {
        ShowHighlight();
    }

    //Unhighlight when another button is selected
    public void OnDeselect(BaseEventData eventData)
    {
        if (!isPointerOver)
        {
            HideHighlight();
        }
    }

    //Show the yellow border and enlarge the button when highlighted
    private void ShowHighlight()
    {
        outline.enabled = true;
        transform.localScale = originalScale * scaleMultiplier;
    }

    //Return the button to its original form when unhighlighted
    private void HideHighlight()
    {
        outline.enabled = false;
        transform.localScale = originalScale;
    }
}
