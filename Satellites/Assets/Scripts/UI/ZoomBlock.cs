using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MainCamera.ZoomIsBlocked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MainCamera.ZoomIsBlocked = false;
    }

    private void OnDisable()
    {
        MainCamera.ZoomIsBlocked = false;
    }
}