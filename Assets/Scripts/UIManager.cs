using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image[] uiImages;          // ui 객체 배열
    public Text turnText;             // 현재 몇턴인지 보여주는 텍스트

    public float maxSteerAngle = 50f; // 최대 회전 각도
    private float wheelAngle = 0f; // 현재 핸들 각도
    private float lastWheelAngle = 0f;
    private bool isAccelPressed = false; //엑셀이 눌러졌는지 확인 

    Button[] buttons;
    void Start()
    {
        uiImages = GetComponentsInChildren<Image>();    // 0: accel    1: brake    2: handle   3: turnBar
        buttons = GetComponentsInChildren<Button>();    // 0: accel    1: brake    2: handle
        buttons[0].gameObject.AddComponent<AccelButtonListener>().Init(this);
        buttons[0].onClick.AddListener(GameManager.inst.turnManager.midTurn);
        buttons[0].onClick.AddListener(() => GameManager.inst.player.Accelerate(15f));// 가속
        buttons[1].onClick.AddListener(GameManager.inst.turnManager.midTurn);
        buttons[1].onClick.AddListener(() => GameManager.inst.player.Brake(0.5f)); // 감속



    }
    public void SetAccel(bool state)
    {
        isAccelPressed = state;
    }

    void Update()
    {
        if (isAccelPressed)
        {
            GameManager.inst.player.Accelerate(10f * Time.deltaTime); // 프레임 보정
        }

        turnText.text = string.Format("cur\nTurn: " + GameManager.inst.turnManager.curTurn);
                uiImages[2].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);
    }

    public void OnPointerDown(PointerEventData eventData)   // 터치 누를 때 작동하는 함수
    {
            lastWheelAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)       // 터치해서 드래그할 때 작동하는 함수 현재는 핸들 ui를 드래그하면 그에 맞게 핸들이랑 player 회전
    {
        float newAngle = GetAngle(eventData.position);   
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngle, newAngle);
        wheelAngle = Mathf.Clamp(wheelAngle + deltaAngle, -maxSteerAngle, maxSteerAngle);
        uiImages[2].rectTransform.localEulerAngles = new Vector3(0, 0, wheelAngle);                 // 핸들 회전
        GameManager.inst.player.transform.localEulerAngles = new Vector3(0, 0, wheelAngle + 90);    // 플레이어 객체 회전
        lastWheelAngle = newAngle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    private float GetAngle(Vector2 touchPos)        // 터치한 곳과 핸들사이 각도 계산
    {
        Vector2 dir = touchPos - (Vector2)uiImages[2].rectTransform.position;   
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }


}
