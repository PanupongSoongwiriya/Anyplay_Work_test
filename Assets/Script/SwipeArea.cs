using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeArea : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        if (ThirdPersonCamera.Instance != null)
            ThirdPersonCamera.Instance.LookAround(eventData.position);
    }
}