using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunAnimType
{
    Shoot,
    Reload
}
public enum GunAnimParametersName
{
    Parameter_Shoot,
    Parameter_Reload,
    Parameter_Reloading,

    Count
}
public class GunRenderer_may : MonoBehaviour
{
    public AudioSource GunAudio;
    public Animator GunAnim;
    public string[] anim_ParametersName = new string[(int)GunAnimParametersName.Count];
    public ParticleSystem muzzleVFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAnim(GunAnimType type)
    {

        switch (type)
        {
            case GunAnimType.Shoot:
                {
                    GunAnim.SetTrigger("Shoot");
                    //GunAnim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName.Parameter_Shoot]);
                    break;
                }
            case GunAnimType.Reload:
                {
                    GunAnim.SetTrigger("Reload");
                    GunAnim.SetTrigger("Reloading");
                    //GunAnim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName.Parameter_Reload]);
                    //GunAnim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName.Parameter_Reloading]);
                    break;
                }
        }
    }

    public void ShowShootVFX()
    {
        if (muzzleVFX)
            muzzleVFX.Play();
    }
}
