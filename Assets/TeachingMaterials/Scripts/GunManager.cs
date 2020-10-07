using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    #region Settings
    [Header("Settings")]
    public FireMode fireMode = FireMode.Semi;
    
    public float fireRate = 0.1f;

    public int bulletsPerMagazine = 30;

    public float baseDamage = 30f;

    public float range = 500f;

    public AnimationCurve damageDropoff = AnimationCurve.Linear(0, 1, 500f, 0.8f);

    public Transform muzzleTrans;

    public ImpactProcessor _ImpactProcessor;

    public float _HitForce = 5f;
    #endregion

    #region Animation Settings
    public bool setOutOfAmmoBool;
    #endregion

    #region Bullet Spread
    [Header("Bullet Spread")]
    public float bulletSpreadHipBase = 0.1f;
    public float bulletSpreadHipVelocityAdd = 0.1f;
    public float bulletSpreadHipVelocityReference = 6f;
    #endregion

    #region Recoil
    [Header("Recoil")]
    public Vector2 recoilPerShotMin = new Vector2 { x = -0.1f, y = 0.5f };
    public Vector2 recoilPerShotMax = new Vector2 { x = 0.1f, y = 1f };
    public float recoilApplyTime = 0.1f;
    public float recoilReturnSpeed = 6f;
    #endregion

    #region Reload
    [Header("Reload")]
    //public float reloadTimeOne, reloadTimeTwo;
    public AudioClip reloadSound;
    #endregion

    #region Sounds
    [Header("Sounds")]
    public AudioSource _GunAudio;
    public AudioClip fireSound;
    public AudioClip dryFireSound;
    #endregion

    private PlayerManager Owner;
    private GunRenderer _GunRenderer;
    private int bulletsLeft;
    private float lastFireTime;
    private bool reloadInProgress;
    // Start is called before the first frame update
    void Start()
    {
        if (_GunAudio == null)
        {
            _GunAudio = GetComponent<AudioSource>();
            if(_GunAudio == null)
                _GunAudio = gameObject.AddComponent<AudioSource>();
        }
        _GunRenderer = GetComponent<GunRenderer>();
        bulletsLeft = bulletsPerMagazine;
        if (GameManager.Instance)
            Owner = GameManager.Instance._Player;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateWeaponUpdate();
    }
    public void CalculateWeaponUpdate()
    {
            if (!reloadInProgress)
            {
                if (!Owner.IsDead)
                {
                        if (fireMode == FireMode.Semi)
                        {
                            if (Input.GetMouseButtonDown(0))
                            //if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
                            {
                                if (Time.time >= lastFireTime + fireRate)
                                {
                                    if (bulletsLeft > 0)
                                    {
                                        FireOneShot();
                                    }
                                    else
                                    {
                                        DryFire();
                                    }
                                }
                            }
                        }
                        else if (fireMode == FireMode.Auto)
                        {
                            if (Input.GetMouseButton(0))
                            //if (ViveInput.GetPress(HandRole.RightHand, ControllerButton.Trigger))
                            {
                                if (Time.time >= lastFireTime + fireRate)
                                {
                                    if (bulletsLeft > 0)
                                    {
                                        FireOneShot();
                                    }
                                    else
                                    {
                                        DryFire();
                                    }
                                }
                            }
                        }

                    //計算武器角度差(>45)
                    float diffAngle_ = Vector3.Angle(Owner.transform.forward, Vector3.ProjectOnPlane(transform.forward, Owner.transform.right));

                    if (!Input.GetMouseButton(0) && (diffAngle_ >= 45))
                    //if (!ViveInput.GetPress(HandRole.RightHand, ControllerButton.Trigger) && (diffAngle_ >= 45))
                    {
                            if (bulletsLeft != bulletsPerMagazine)
                            {
                                reloadInProgress = true;

                                //data.reloadNextEnd = Time.time + reloadTimeOne;

                                //_GunRenderer.anim.Play("Reload Full", 0, 0f);
                                _GunRenderer.SetAnim(GunAnimType.Reload);

                                _GunAudio.PlayOneShot(reloadSound);
                            }
                    }
                }

                if (setOutOfAmmoBool)
                {
                    _GunRenderer.anim.SetBool("OutOfAmmo", bulletsLeft == 0);
                }
            }
            else
            {
                if (setOutOfAmmoBool)
                {
                    _GunRenderer.anim.SetBool("OutOfAmmo", false);
                }

                if (!_GunRenderer.anim.GetBool("Reloading"))
                {
                        bulletsLeft = bulletsPerMagazine;
                        reloadInProgress = false;
                }
                //if (Time.time >= data.reloadNextEnd)
                //{
                //    if (data.reloadPhase == 0)
                //    {
                //        bulletsLeft = bulletsPerMagazine;

                //        data.reloadPhase = 1;
                //        data.reloadNextEnd = Time.time + reloadTimeTwo;
                //    }
                //    else if (data.reloadPhase == 1)
                //    {
                //        reloadInProgress = false;
                //        data.reloadPhase = 0;
                //    }
                //}
            }

            //if (data.shellEjectEnabled)
            //{
            //    if (Time.time >= data.shellEjectNext || shellEjectionTime <= 0)
            //    {
            //        data.shellEjectEnabled = false;

            //        EjectShell(pb, data);
            //    }
            //}

            _GunRenderer.DisplayAmmo(bulletsLeft);
    }

    void FireOneShot()
    {
        lastFireTime = Time.time;

        _GunAudio.PlayOneShot(fireSound);

        if (bulletsLeft == 1)
        {
            _GunRenderer.anim.Play("Fire Last", 0, 0f);
        }
        else
        {
            _GunRenderer.SetAnim(GunAnimType.Shoot);
            //_GunRenderer.anim.Play("Fire", 0, 0f);
        }

        //if (shellEjectionPrefab)
        //{
        //    data.shellEjectEnabled = true;
        //    data.shellEjectNext = Time.time + shellEjectionTime;
        //}
        _GunRenderer.SetmuzzleFlash();

        bulletsLeft--;

        FireRaycast();

        //Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.WeaponApplyRecoil(this, data, pb, RandomExtensions.RandomBetweenVector2(recoilPerShotMin, recoilPerShotMax), recoilApplyTime));
    }


    public void FireRaycast()
    {
        RaycastHit hit;

        Vector3 dir = muzzleTrans.forward + GetSpread();

        if (Physics.Raycast(muzzleTrans.position, dir, out hit, range, Owner._WeaponHitLayers))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyPart enemyPart_ = hit.transform.GetComponent<EnemyPart>();
                if (enemyPart_)
                    enemyPart_.Damage(GetDamage(Vector3.Distance(Owner.transform.position, hit.point)), dir* _HitForce);
                //Kit_BotBehaviour hitPb = hit.transform.root.GetComponent<Kit_BotBehaviour>();

                //GenericDismembering hitpart = hit.transform.GetComponent<GenericDismembering>();

                //int hitparttype = -1;

                //if (hitpart != null)
                //{
                //    hitparttype = (int)hitpart.bodyPart;
                //}

                //hitPb.LocalDamage(GetDamage(Vector3.Distance(Owner.transform.position, hit.point)), dir, hitparttype);

                _ImpactProcessor.ProcessEnemyImpact(hit.point, hit.normal);
            }
            else if (hit.collider.CompareTag("Explode"))   //May Oil barrel
            {
                //ExplodeManager explode_ = hit.transform.GetComponent<ExplodeManager>();
                //if (explode_)
                //    explode_.Explode();
                //_ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Metal, hit.transform);
            }
            else
            {
                if (hit.collider.CompareTag("Dirt"))
                {
                    _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Dirt, hit.transform);
                }
                else if (hit.collider.CompareTag("Metal"))
                {
                    _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Metal, hit.transform);
                }
                else if (hit.collider.CompareTag("Wood"))
                {
                    _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Wood, hit.transform);
                }
                else if (hit.collider.CompareTag("Blood"))
                {
                    _ImpactProcessor.ProcessEnemyImpact(hit.point, hit.normal);
                }
                else
                {
                    _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Concrete, hit.transform);
                }
            }
        }
    }

    float GetDamage(float distance)
    {
        return baseDamage * damageDropoff.Evaluate(distance);
    }

    Vector3 GetSpread()
    {
        Vector3 spreadHip = Vector3.zero;
        spreadHip.x = Random.Range(-bulletSpreadHipBase, bulletSpreadHipBase);
        spreadHip.y = Random.Range(-bulletSpreadHipBase, bulletSpreadHipBase);
        spreadHip.z = Random.Range(-bulletSpreadHipBase, bulletSpreadHipBase);

        return spreadHip;
    }

    void DryFire()
    {
        _GunAudio.PlayOneShot(dryFireSound);

        lastFireTime = Time.time + 1f;

        _GunRenderer.anim.Play("Dry Fire", 0, 0f);

        _GunRenderer.DisplayAmmoReloadHint();
    }
}
