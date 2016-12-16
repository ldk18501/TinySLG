using UnityEngine;
using System.Collections;

public class TestDragInput : MonoBehaviour {

    public Camera camTestInput;

    bool _bHitPlayer;
    bool _bJustDragOut;
    Vector3 _v3LastClickPos;
    GameObject _objOriginHitObj;
    GameObject _objDragOutHolding;
    float _fDragDis;

    RaycastHit _hit;


    System.Action<Vector3, Vector3> OnDragOutCallback;
    System.Action<Vector3, GameObject> OnDragEndCallback;

    // Use this for initialization
    void Start () {
        this.OnDragOutCallback = HandleDragOut;
	}
	
	// Update is called once per frame
	void Update () {
        DragToInitObj();
    }

    void LateUpdate()
    {
        TickDragOutObj();
    }

    //点击碰撞物体并拖出副本,目前只支持鼠标,要在平台加宏加touch
    void DragToInitObj()
    {
    
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(camTestInput.ScreenPointToRay(Input.mousePosition), out _hit, 1000f, 1 << LayerMask.NameToLayer("Player")))
            {
                _bHitPlayer = true;
                _bJustDragOut = false;
                _v3LastClickPos = Input.mousePosition;
                _objOriginHitObj = _hit.collider.gameObject;
                Debug.Log("hit " + _objOriginHitObj.name);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            SetOriginAble(true);
            _bJustDragOut = _bHitPlayer = false;
            _objOriginHitObj = _objDragOutHolding = null;
        }

        if (_bHitPlayer)
        {
            Vector3 deltaVec = Input.mousePosition - _v3LastClickPos;
            //if (deltaVec.sqrMagnitude > 0.1f)
            //{
            //    Debug.Log(deltaVec);
            //}
            _v3LastClickPos = Input.mousePosition;

            //TODO::拖拽结束的回调可以在这里判断,如果确实拖动了而且接下来松手,就触发

            //NGUI源码中就类似这样,tick射线
            var obj = Physics.Raycast(camTestInput.ScreenPointToRay(Input.mousePosition), out _hit, 1000f, 1 << LayerMask.NameToLayer("Player"));
            if (_hit.collider == null || _hit.collider.gameObject != _objOriginHitObj)
            {
                if (!_bJustDragOut)
                {
                    _bJustDragOut = true;
                    if (OnDragOutCallback != null)
                        OnDragOutCallback.Invoke(Input.mousePosition, _hit.point);
                }
            } 
        }

    }

    //测试一下回调,取相同的距离,创造副本物体,一次只支持一个副本,所以把原物体隐藏了
    void HandleDragOut(Vector3 screenPos, Vector3 originPos)
    {
        _objDragOutHolding = GameObject.Instantiate(_objOriginHitObj) as GameObject;
        _objDragOutHolding.name = "Clone";
        originPos = new Vector3(originPos.x, originPos.y, _objOriginHitObj.transform.position.z);
        _fDragDis = Vector3.Distance(camTestInput.transform.position, originPos);
        _objDragOutHolding.transform.position = camTestInput.ScreenPointToRay(screenPos).GetPoint(_fDragDis);
        SetOriginAble(false);
    }

    //每一帧都保持距离,跟随鼠标
    void TickDragOutObj()
    {
        if (_bHitPlayer && _objDragOutHolding != null && _objOriginHitObj != null)
        {
            //固定一个距离,不会出现指针脱离物体的情况,但是近大远小,除非正交相机
            _objDragOutHolding.transform.position = camTestInput.ScreenPointToRay(Input.mousePosition).GetPoint(_fDragDis);
        }
    }

    void SetOriginAble(bool state)
    {
        _objOriginHitObj.GetComponent<Renderer>().enabled = state;
        _objOriginHitObj.GetComponent<Collider>().enabled = state;
    }
}
