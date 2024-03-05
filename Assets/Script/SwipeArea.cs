using UnityEngine;
using UnityEngine.EventSystems;

//This class it takes the position that the player swipes the area in 'View'
public class SwipeArea : MonoBehaviour, IDragHandler
{
    [SerializeField] private ThirdPersonCamera TPCamera;

    //Send the location the player swiped to ThirdPersonCamera.
    public void OnDrag(PointerEventData eventData)
    {
        if (TPCamera != null)
        {
            TPCamera.LookAround(eventData.position);
        }
    }
}