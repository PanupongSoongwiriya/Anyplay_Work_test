using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeArea : MonoBehaviour, IDragHandler
{
    public ThirdPersonCamera TPCamera;

    public void OnDrag(PointerEventData eventData)
    {
        if (TPCamera != null)
        {
            TPCamera.LookAround(eventData.position);
        }
    }
}