using UnityEngine;
using UnityEngine.EventSystems;

public class AccelButtonListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UIManager ui;

    public void Init(UIManager uiManager)
    {
        ui = uiManager;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.SetAccel(true);
        GameManager.inst.turnManager.midTurn(); // 턴 시작도 같이
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ui.SetAccel(false);
    }
}
