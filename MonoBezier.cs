using UnityEngine;
using System.Collections;

//二阶贝塞尔曲线(抛物线)：
//B(t) = (1-t)^2 * P0 + (1-t) * t * P1 + t^2 * P2
public class MonoBezier : MonoBehaviour {

    public Transform[] _PointArray;
    public bool _bDebugLine;
    private Vector3 _vSrcPos;
    private Vector3 _vDesPos;
    private Vector3 _vCtrlPos1;
    private Vector3 _vCtrlPos2;

    public float fParamT;
    //public float fHeight;
    public GameObject CtrlObj;

    private float _fRatioZ1;
    private float _fRatioZ2;

	// Use this for initialization
	void Start () {
        SyncAllPos();
        Vector3 vecSrcDes = _vDesPos - _vSrcPos;
        Vector3 vecSrcCtrl1 = _vCtrlPos1 - _vSrcPos;
        Vector3 vecSrcCtrl2 = _vCtrlPos2 - _vSrcPos;
        _fRatioZ1 = vecSrcCtrl1.z / vecSrcDes.z;
        _fRatioZ2 = vecSrcCtrl2.z / vecSrcDes.z;
	}
	
	// Update is called once per frame
	void Update () {
        if (_bDebugLine)
        {
            SyncAllPos();
        }
        else
        {
            SyncPosUpdate();
            if (CtrlObj != null)
            {
                fParamT += Time.deltaTime;
                if (fParamT > 1) fParamT = 0;
                if (fParamT < 0) fParamT = 0;
                CtrlObj.transform.position = Cal4PointBezierPos(fParamT);
                //Quaternion qRot = Quaternion.LookRotation();

                Vector3 firtMidPos = (_vCtrlPos1 - _vSrcPos) * fParamT + _vSrcPos;
                Vector3 secondMidPos = (_vCtrlPos2 - _vCtrlPos1) * fParamT + _vCtrlPos1;
                Vector3 thirdMidPos = (_vDesPos - _vCtrlPos2) * fParamT + _vCtrlPos2;

                Vector3 vHehe = (secondMidPos + (thirdMidPos - secondMidPos) * fParamT)
                    - (firtMidPos + (secondMidPos - firtMidPos) * fParamT);
                CtrlObj.transform.forward = vHehe.normalized;
            }
        }
	}

    void OnDrawGizmos()
    {
        float fPace = 0.05f;
        for (float i = 0; i < 1; i += fPace)
        {
            Vector3 vSrc = Cal4PointBezierPos(i);
            Vector3 vDes = Cal4PointBezierPos(i + fPace);
            Gizmos.DrawLine(vSrc, vDes);
        }
    }

    Vector3 Cal4PointBezierPos(float fT)
    {
        return (1 - fT) * (1 - fT) * (1 - fT) * _vSrcPos +
               3 * _vCtrlPos1 * fT * (1 - fT) * (1 - fT) +
               3 * _vCtrlPos2 * fT * fT * (1 - fT) +
               fT * fT * fT * _vDesPos;
    }


    void SyncAllPos()
    {
        _vSrcPos = _PointArray[0].position;
        _vCtrlPos1 = _PointArray[1].position;
        _vCtrlPos2 = _PointArray[2].position;
        _vDesPos = new Vector3(_vDesPos.x, _vDesPos.y, _PointArray[3].position.z);
        _PointArray[3].position = _vDesPos;

    }

    void SyncPosUpdate()
    {
        _vDesPos = new Vector3(_vDesPos.x, _vDesPos.y, _PointArray[3].position.z);
        _PointArray[3].position = _vDesPos;
        float newZ1 = _vDesPos.z * _fRatioZ1;
        float newZ2 = _vDesPos.z * _fRatioZ2;
        _vCtrlPos1 = new Vector3(_vCtrlPos1.x, _vCtrlPos1.y, newZ1);
        _vCtrlPos2 = new Vector3(_vCtrlPos2.x, _vCtrlPos2.y, newZ2);
        _PointArray[1].position = _vCtrlPos1;
        _PointArray[2].position = _vCtrlPos2;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 50), "Switch DebugMode."))
        {
            _bDebugLine = !_bDebugLine;
        }
    }
}
