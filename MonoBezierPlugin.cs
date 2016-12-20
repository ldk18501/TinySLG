using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MonoBezierPlugin : MonoBehaviour 
{
    public Transform[] _PointArray;
    private Vector3 _vSrcPos;
    private Vector3 _vDesPos;
    private Vector3 _vCtrlPos1;
    private Vector3 _vCtrlPos2;

    public bool bIsLoop;
    public float fParamT;
    public float fFlySpeed = 1.0f;
    public GameObject CtrlObj;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SyncAllPos();
        if (CtrlObj != null)
        {
            fParamT += Time.deltaTime * fFlySpeed;
            if (bIsLoop)
            {
                if (fParamT > 1) fParamT = 0;
                if (fParamT < 0) fParamT = 1;
            }
            else
            {
                if (fParamT > 1) fParamT = 1;
                if (fParamT < 0) fParamT = 0;
            }
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
        _vDesPos = _PointArray[3].position;
    }
}
