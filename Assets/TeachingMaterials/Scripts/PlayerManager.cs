using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int _MaxHP = 100;       //血量設定
    private float _CurrentHP;    //當前血量

    public LayerMask _WeaponHitLayers;   //攻擊目標Layer

    private float _LastHitTime;      //最後被攻擊時間
    public float _AutomaticRecoveryTime = 3.5f;      //等待自動回血(一定時間未受到攻擊)
    public float _AutomaticRecoveryHP = 35;          //每秒回多少血

    private bool _IsDead = false;        //是否死亡
    public bool IsDead
    { get { return _IsDead; } }
    public float _WaitResurrectionTime = 5f;     //等待復活時間
    private float _TillResurrectionTime = 0f;

    private bool _IsProtect = false;
    public float _SpawnProtectionTime = 10f;     //復活保護時間
    private float _TillProtectionEndTime = 0f;
    public GameObject _ShieldObj;        //保護狀態物件


    public PlayerHUD _PlayerHUD;

    #region Test
    [Header("Test")]
    public Transform HitTarget;
    public float damage = 10;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsDead)
        {
            _TillResurrectionTime -= Time.deltaTime;
            if (_TillResurrectionTime <= 0)
            {
                Init();
            }
        }
        if (_IsProtect)
        {
            _TillProtectionEndTime -= Time.deltaTime;
            if (_TillProtectionEndTime <= 0)
            {
                Protect(false);
            }
        }

        AutomaticRecovery();

        #region Test
        if (Input.GetKeyDown(KeyCode.A))
        {
            BeAttacked(HitTarget.position, damage, 0);
        }
        #endregion
    }

    private void Init()
    {
        _CurrentHP = _MaxHP;
        _IsDead = false;
        _TillResurrectionTime = 0;

        _IsProtect = false;
        _TillProtectionEndTime = 0;

        float percentHP_ = _CurrentHP / _MaxHP;
        if(_PlayerHUD)
            _PlayerHUD.DisplayHurtState(percentHP_, _IsDead);

        Protect(true);
    }

    public void BeAttacked(Vector3 attackFrom, float damage,int AttackType)
    {
        if (_IsDead ||_IsProtect)
            return;

        _CurrentHP -= damage;

        _LastHitTime = Time.time;

        if (_CurrentHP <= 0)
        {
            _CurrentHP = 0;
            Die();
        }

        _PlayerHUD.DisplayShot(attackFrom);
        float percentHP_ = _CurrentHP / _MaxHP;
        _PlayerHUD.DisplayHurtState(percentHP_, _IsDead);

        //if (AttackType == Boss)
        //{
        //    _PlayerHUD.DisplayBossAttack();
        //}
    }

    private void Die()
    {
        _IsDead = true;
        _TillResurrectionTime = _WaitResurrectionTime;
        //生成ragdoll
    }

    private void Protect(bool isProtect)
    {
        _IsProtect = isProtect;
        //計時
        if(_IsProtect)
            _TillProtectionEndTime = _SpawnProtectionTime;
        else
            _TillProtectionEndTime = 0;

        if(_ShieldObj)
            _ShieldObj.SetActive(_IsProtect);
    }

    private void AutomaticRecovery()
    {
        if (_IsDead || _CurrentHP >= _MaxHP)
            return;

        if (Time.time >= _LastHitTime + _AutomaticRecoveryTime)
        {
            _CurrentHP += _AutomaticRecoveryHP * Time.deltaTime;
        }

        if (_CurrentHP >= _MaxHP)
            _CurrentHP = _MaxHP;
    }
}
