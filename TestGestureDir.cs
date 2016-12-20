using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestGestureDir : MonoBehaviour
{
    private float _fStartTouchTime;
    private Vector3 _v3StartTouchPos;

    public float fSampleTime = 0.1f;
    public float fSampleDisThreshold = 5f;
    private float _fSampleCounter;
    private List<Vector3> _sampleVecList = new List<Vector3>();


    // Use this for initialization
    void Start () {
        _fSampleCounter = fSampleTime;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _fStartTouchTime = Time.realtimeSinceStartup;
            _v3StartTouchPos = Input.mousePosition;
            _sampleVecList.Add(_v3StartTouchPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_fStartTouchTime > 0)
                JudgeDragMove(Input.mousePosition);
        }

        if (_fStartTouchTime > 0)
        {
            if (_fSampleCounter > 0)
            {
                _fSampleCounter -= Time.deltaTime;
                if (_fSampleCounter <= 0)
                {
                    _fSampleCounter = fSampleTime;
                    _sampleVecList.Add(Input.mousePosition);
                }
            }

            if (Time.realtimeSinceStartup - _fStartTouchTime > 0.5f)
                JudgeDragMove(Input.mousePosition);
        }
    }

    void JudgeDragMove(Vector3 lastPos)
    {
        if (Vector3.Distance(lastPos, _v3StartTouchPos) > fSampleDisThreshold)
        {
            Vector3 sumDisVec = Vector3.zero;
            for (int i = 0; i < _sampleVecList.Count - 1; i++)
            {
                sumDisVec += _sampleVecList[i + 1] - _sampleVecList[i];
            }

            sumDisVec = sumDisVec / (_sampleVecList.Count - 1);
            Debug.Log(sumDisVec.normalized);
        }
        _sampleVecList.Clear();
        _fStartTouchTime = -1;
    }
}
