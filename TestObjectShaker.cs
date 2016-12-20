using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestObjectShaker : MonoBehaviour {

    //最好写在外部,做成单例的成员,方便被调用和管理
    private List<ObjShakeInstance> _shakeInstances = new List<ObjShakeInstance>();

    /// <summary>
    /// The default position influcence of all shakes created by this shaker.
    /// </summary>
    public Vector3 DefaultPosInfluence = Vector3.one;
    /// <summary>
    /// The default rotation influcence of all shakes created by this shaker.
    /// </summary>
    public Vector3 DefaultRotInfluence = new Vector3(0.5f, 1.5f, 2.5f);
    public bool _bOverrideInfluence;
    private Vector3 _posAddShake, _rotAddShake;

    void Awake()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            SetCamaraShake(0.5f, 6f, 2f);
        }
        UpdateShakeOperator();
    }

    //两个调用震动的方法,目前没有管理器,没单例,所以外部调不到,以后有需要就外放
    public void SetCamaraShake(float magnitude, float roughness, float shakeTime, float shakeInTime = 0.1f)
    {
        if (shakeInTime == 0 && shakeTime == 0) return;
        _shakeInstances.Add(new ObjShakeInstance(magnitude, roughness, shakeInTime, shakeTime));
    }
    public ObjShakeInstance SetCameraShake(float magnitude, float roughness)//外部自己管理开关情况
    {
        ObjShakeInstance shakeInstance = new ObjShakeInstance(magnitude, roughness);
        _shakeInstances.Add(shakeInstance);
        return shakeInstance;//调用instance的startfadein和startfadeout
    }

    void UpdateShakeOperator()
    {
        _posAddShake = Vector3.zero;
        _rotAddShake = Vector3.zero;

        for (int i = 0; i < _shakeInstances.Count; i++)
        {
            ObjShakeInstance c = _shakeInstances[i];
            if (c.CurrentState == ObjShakeInstance.ShakeState.Inactive && c._bDeleteOnInactive)
            {
                _shakeInstances.RemoveAt(i);
                i--;
            }
            else if (c.CurrentState != ObjShakeInstance.ShakeState.Inactive)
            {
                Vector3 posInf = _bOverrideInfluence == true ? DefaultPosInfluence : c._v3PositionInfluence;
                Vector3 rotInf = _bOverrideInfluence == true ? DefaultRotInfluence : c._v3RotationInfluence;
                _posAddShake += MultiplyVectors(c.UpdateShake(), posInf);
                _rotAddShake += MultiplyVectors(c.UpdateShake(), rotInf);
            }
        }

        transform.localPosition = _posAddShake;
        transform.localEulerAngles = _rotAddShake;
    }

    private Vector3 MultiplyVectors(Vector3 v, Vector3 w)
    {
        v.x *= w.x;
        v.y *= w.y;
        v.z *= w.z;

        return v;
    }
}

public class ObjShakeInstance
{
    public enum ShakeState
    {
        FadingIn,
        FadingOut,
        Sustained,
        Inactive,
    }
    public float _fMagnitude;//震幅
    public float _fRoughness;//震频
    public Vector3 _v3PositionInfluence = new Vector3(0.25f, 0.25f, 0.25f);//位置影响倍率
    public Vector3 _v3RotationInfluence = Vector3.one;//角度影响倍率
    public bool _bDeleteOnInactive = true;

    float _fRoughMod = 1, _fMagnMod = 1;//有需要的话暴露出来直接改震动倍率
    float _fFadeOutDuration, _fFadeInDuration;
    bool _bSustain, _bIsShaking;
    float _fCurrentFadeTime;
    float _fTick = 0;
    Vector3 _v3Amt;

    public ObjShakeInstance(Vector3 pos, Vector3 angle, float magnitude, float roughness, float fadeInTime, float fadeOutTime)
    {
        _v3PositionInfluence = pos;
        _v3RotationInfluence = angle;
        this._fMagnitude = magnitude;
        this._fFadeOutDuration = fadeOutTime;
        this._fFadeInDuration = fadeInTime;
        this._fRoughness = roughness;
        if (fadeInTime > 0)
        {
            _bSustain = true;
            _fCurrentFadeTime = 0;
        }
        else
        {
            _bSustain = false;
            _fCurrentFadeTime = 1;
        }
        _fTick = Random.Range(-100, 100);
        _bIsShaking = true;
    }

    //用来创建自动开始自动结束的震动
    public ObjShakeInstance(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
    {
        this._fMagnitude = magnitude;
        this._fFadeOutDuration = fadeOutTime;
        this._fFadeInDuration = fadeInTime;
        this._fRoughness = roughness;
        if (fadeInTime > 0)
        {
            _bSustain = true;
            _fCurrentFadeTime = 0;
        }
        else
        {
            _bSustain = false;
            _fCurrentFadeTime = 1;
        }
        _fTick = Random.Range(-100, 100);
        _bIsShaking = true;
    }

    //用来创建手动开始和停止的震动
    public ObjShakeInstance(float magnitude, float roughness)
    {
        this._fMagnitude = magnitude;
        this._fRoughness = roughness;
        _bSustain = true;
        _fTick = Random.Range(-100, 100);
    }

    public Vector3 UpdateShake()
    {
        if (!_bIsShaking) return Vector3.zero;
        _v3Amt.x = Mathf.PerlinNoise(_fTick, 0) - 0.5f;
        _v3Amt.y = Mathf.PerlinNoise(0, _fTick) - 0.5f;
        _v3Amt.z = Mathf.PerlinNoise(_fTick, _fTick) - 0.5f;

        if (_fFadeInDuration > 0 && _bSustain)
        {
            if (_fCurrentFadeTime < 1)
                _fCurrentFadeTime += Time.deltaTime / _fFadeInDuration;
            else if (_fFadeOutDuration > 0)
                _bSustain = false;
        }

        if (!_bSustain)
        {
            _fCurrentFadeTime -= Time.deltaTime / _fFadeOutDuration;
            if (_fCurrentFadeTime <= 0) _bIsShaking = false;
        }

        if (_bSustain)
            _fTick += Time.deltaTime * _fRoughness * _fRoughMod;
        else
            _fTick += Time.deltaTime * _fRoughness * _fRoughMod * _fCurrentFadeTime;

        return _v3Amt * _fMagnitude * _fMagnMod * _fCurrentFadeTime;
    }

    //手动停止震动
    public void StartFadeOut(float fadeOutTime)
    {
        if (fadeOutTime == 0)
            _fCurrentFadeTime = 0;
        _fFadeOutDuration = fadeOutTime;
        _fFadeInDuration = 0;
        _bSustain = false;
    }
    //手动开始震动
    public void StartFadeIn(float fadeInTime)
    {
        if (fadeInTime == 0)
            _fCurrentFadeTime = 1;
        _fFadeInDuration = fadeInTime;
        _fFadeOutDuration = 0;
        _bIsShaking = true;
    }

    bool IsShaking
    { get { return _bIsShaking; } }
    bool IsFadingOut
    { get { return !_bSustain && _fCurrentFadeTime > 0; } }
    bool IsFadingIn
    { get { return _fCurrentFadeTime < 1 && _bSustain && _fFadeInDuration > 0; } }

    public ShakeState CurrentState
    {
        get
        {
            if (IsFadingIn)
                return ShakeState.FadingIn;
            else if (IsFadingOut)
                return ShakeState.FadingOut;
            else if (IsShaking)
                return ShakeState.Sustained;
            else
                return ShakeState.Inactive;
        }
    }
}
