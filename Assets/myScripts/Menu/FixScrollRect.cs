using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FixScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect MainScroll;

    public static bool isDragging { get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        MainScroll.OnBeginDrag(eventData);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        MainScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MainScroll.OnEndDrag(eventData);
        isDragging = false;
    }

    public void OnScroll(PointerEventData data)
    {
        MainScroll.OnScroll(data);
    }

    

}