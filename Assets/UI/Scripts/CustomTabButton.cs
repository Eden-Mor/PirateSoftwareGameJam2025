using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=211t6r12XPQ
//Originally by Game Dev Guide

[RequireComponent(typeof(Image))]
public class CustomTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup tabGroup;
    public Image background;


    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData) 
        => tabGroup.OnTabSelected(this);
    public void OnPointerEnter(PointerEventData eventData) 
        => tabGroup.OnTabEnter(this);
    public void OnPointerExit(PointerEventData eventData) 
        => tabGroup.OnTabExit(this);

    public void Select() 
        => onTabSelected?.Invoke();
    public void Deselect()
        => onTabDeselected?.Invoke();

}
