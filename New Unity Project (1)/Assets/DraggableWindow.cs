using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public Vector3 offset;
    #region IBeginDragHandler implementation
    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - (Vector3)eventData.position;
    }
    #endregion
    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector3)eventData.position + offset;
    }

    #endregion
}




