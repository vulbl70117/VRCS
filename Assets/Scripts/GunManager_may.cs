using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enums
public enum FireMode { Semi, Auto };
#endregion
public class GunManager_may : MonoBehaviour
{
    public FireMode fireMode = FireMode.Auto;
    public float FireRate = 0.5f;
    public int bulletsPerMagazine = 120;

    public AudioClip FireSFX;
    public AudioClip FailedShotSFX;
    public AudioClip ReloadSFX;


    public Transform muzzleTrans;
    public float bulletSpreadoffsetBase = 0.5f;
    public float range = 500f;

    public float baseDamage = 30f;
    public AnimationCurve damageDropoff = AnimationCurve.Linear(0, 1, 500f, 0.8f);

    public ImpactProcessor_may _ImpactProcessor;

    private float LastFireTime;
    private int bulletsLeft;
    private GunRenderer_may GunRenderer;
    private bool reloadInProgress;

    private void Start()
    {
        bulletsLeft = bulletsPerMagazine;
        GunRenderer = GetComponent<GunRenderer_may>();
    }
    private void Update()
    {
        Debug.DrawLine(muzzleTrans.position, muzzleTrans.position + muzzleTrans.forward * range, Color.red);
        if (!reloadInProgress)
        {
            if (fireMode == FireMode.Semi)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }
            else if (fireMode == FireMode.Auto)
            {
                if (Input.GetMouseButton(0))
                {
                    Shoot();
                }
            }

            if (!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.R))
            {
                if (bulletsLeft != bulletsPerMagazine)
                {
                    reloadInProgress = true;
                    //補充音效
                    GunRenderer.GunAudio.PlayOneShot(ReloadSFX);
                    //補充動畫
                    GunRenderer.ShowAnim(GunAnimType.Reload);
                }
            }
        }
        else
        {
            if (!GunRenderer.GunAnim.GetBool("Reloading"))
            {
                bulletsLeft = bulletsPerMagazine;
                reloadInProgress = false;
            }
        }
    }

    void Shoot()
    {
        if (Time.time >= LastFireTime + FireRate)
        {
            if (bulletsLeft > 0)
            {
                FireOneShot();
            }
            else
            {
                FailedShot();
            }
        }
    }

    void FireOneShot()
    {
        LastFireTime = Time.time;
        //射擊音效
        GunRenderer.GunAudio.PlayOneShot(FireSFX);
        //射擊動畫
        GunRenderer.ShowAnim(GunAnimType.Shoot);
        //槍口特效
        GunRenderer.ShowShootVFX();
        bulletsLeft--;
        FireRaycast();
    }
    void FailedShot()
    {
        LastFireTime = Time.time;
        //無子彈回饋音效
        GunRenderer.GunAudio.PlayOneShot(FailedShotSFX);
    }

    void FireRaycast()
    {
        RaycastHit hit_;
        Vector3 dir_ = muzzleTrans.forward + GetSpread();
        if (Physics.Raycast(muzzleTrans.position, dir_, out hit_, range))
        {
            //擊中回饋
            if (hit_.transform.CompareTag("Enemy"))
            {

                //EnemyManager_may enemy_ = hit_.transform.GetComponent<EnemyManager_may>();
                //if (enemy_)
                //    enemy_.BeAttacked(GetDamage(hit_.distance));
                EnemyPart_may enemy_ = hit_.transform.GetComponent<EnemyPart_may>();
                if (enemy_)
                    enemy_.Damage(GetDamage(hit_.distance));
                _ImpactProcessor.ProcessEnemyImpact(hit_.point, hit_.normal);
            }
            else if (hit_.transform.CompareTag("Concrete"))
                _ImpactProcessor.ProcessImpact(hit_.point, hit_.normal, ImpactType.Concrete, hit_.transform);
            else if (hit_.transform.CompareTag("Dirt"))
                _ImpactProcessor.ProcessImpact(hit_.point, hit_.normal, ImpactType.Dirt, hit_.transform);
            else if (hit_.transform.CompareTag("Metal"))
                _ImpactProcessor.ProcessImpact(hit_.point, hit_.normal, ImpactType.Metal, hit_.transform);
            else if (hit_.transform.CompareTag("Wood"))
                _ImpactProcessor.ProcessImpact(hit_.point, hit_.normal, ImpactType.Wood, hit_.transform);
            else
                _ImpactProcessor.ProcessImpact(hit_.point, hit_.normal, ImpactType.Concrete, hit_.transform);
        }
    }

    float GetDamage(float distance)
    {
        return baseDamage * damageDropoff.Evaluate(distance);
    }
    Vector3 GetSpread()
    {
        Vector3 spreadoffset = Vector3.zero;
        spreadoffset.x = Random.Range(-bulletSpreadoffsetBase, bulletSpreadoffsetBase);
        spreadoffset.y = Random.Range(-bulletSpreadoffsetBase, bulletSpreadoffsetBase);
        //spreadoffset.z = Random.Range(-bulletSpreadoffsetBase, bulletSpreadoffsetBase);

        return spreadoffset;
    }
}
