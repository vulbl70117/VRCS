using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    public enum Mode
    {
        one=0,
        auto
    }
    public Mode firemode = Mode.one;
    public AudioClip Fireshoot;
    public AudioClip Failshoot;
    public AnimationCurve FirePower = AnimationCurve.Linear(0, 1, 500, 0.5f);
    public Transform Pos;
    public int bulletMax_1 = 300;
    public float bulletdev = 0.5f;//偏移Random
    public float spacing_1=0.5f;
    public float delay = 1f;
    public float distance = 500f;//距離

    private int bulletMax_2;
    private float lasttime = 0f;
    private bool reload;

    public ImpactProcessor _ImpactProcessor;

    private MachineGunRenderer Renderer;


    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<MachineGunRenderer>();
        bulletMax_2 = bulletMax_1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!reload)
        {
            if (firemode == Mode.one)
            {
                if (Input.GetMouseButtonDown(0))

                Shoot();
            }
            else if (firemode == Mode.auto)
            {
                if(Input.GetMouseButton(0))

                Shoot();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (bulletMax_2!=bulletMax_1)
                {
                    reload=true;
                    Renderer.ShowAnim(GunAnimType.Reload);
                    //音效
                }
            }
        }
        else
        {
            if (Renderer.GunAnim.GetBool("DoReloading"))
            {
                bulletMax_2 = bulletMax_1;
                reload = false;
            }
        }
        
            
        
    }
    public void Shoot()
    {
        if (Time.time >= lasttime + spacing_1)
        {
            if (bulletMax_2 > 0)
            {
                FireShoot();
            }

            else
            {
                FailShoot();
            }
                  
        }
    }
    public void FireShoot()
    {
        lasttime = Time.time;
        bulletMax_2--;
        //動畫
        Renderer.ShowAnim(GunAnimType.Fire);
        //特效
        Renderer.ShowSFX();
        //聲音
        Renderer.GunAudio.PlayOneShot(Fireshoot);
        FireRaycast();
    }
    public void FailShoot()
    {
        lasttime = Time.time + delay;
        Renderer.GunAudio.PlayOneShot(Failshoot);
    }
    public void FireRaycast()
    {
        RaycastHit hit;
        Vector3 directions = Pos.forward + Deviation();
        if(Physics.Raycast(Pos.position,directions,out hit, distance))
        {
            if (hit.transform.CompareTag("Concrete"))
            {
                _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Concrete, hit.transform);
            }
            else if (hit.transform.CompareTag("Dirt"))
            {
                _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Dirt, hit.transform);
            }
            else if (hit.transform.CompareTag("Metal"))
            {
                _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Metal, hit.transform);
            }
            else if (hit.transform.CompareTag("Wood"))
            {
                _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Wood, hit.transform);
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                _ImpactProcessor.EnemyImpact(hit.point, hit.normal);
            }
            else
            {
                _ImpactProcessor.ProcessImpact(hit.point, hit.normal, ImpactType.Concrete, hit.transform);
            }
        }

    }
    Vector3 Deviation()
    {
        Vector3 deviation = Vector3.zero;
        deviation.x = Random.Range(-bulletdev, bulletdev);
        deviation.y = Random.Range(-bulletdev, bulletdev);

        return deviation;
    }
    //private float Direction(direction a)
    //{

    //    return 
    //}
}



