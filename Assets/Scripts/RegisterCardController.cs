using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisterCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public static Action<int> RegisterCardSelected;

    public Color DefaultColor;
    public Color HoverColor;
    public Color ClickColor;
    public Color ActiveColor;
    public Color ActiveHoverColor;
    public UnityEvent onClick;

    [SerializeField] private int id;
    private Image image;
    private bool isActive;

    private void Awake()
    {
        image = GetComponent<Image>();
        RegisterCardSelected += OnSelectRegisterCard;
        if(this.id == 0)
        {
            image.color = ActiveColor;
            this.isActive = true;
        }
        else
        {
            image.color = DefaultColor;
            this.isActive = false;
        }
    }

    private void OnSelectRegisterCard(int id)
    {
        if(this.id == id)
        {
            // select
            image.color = ActiveHoverColor;
            this.isActive = true;
        }
        else
        {
            // deselect
            image.color = DefaultColor;
            this.isActive = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isActive)
        {
            image.color = ActiveHoverColor;
        }
        else
        {
            image.color = HoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isActive)
        {
            image.color = ActiveColor;
        }
        else
        {
            image.color = DefaultColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = ClickColor;
        onClick?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RegisterCardSelected?.Invoke(this.id);
    }
}
