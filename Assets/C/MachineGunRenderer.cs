using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunAnimType
{
    Fire,
    Reload
}
public enum GunAnimName
{
    Fire,
    Reload,
    Reloading,

    Count
}

public class MachineGunRenderer : MonoBehaviour
{
    public AudioSource GunAudio;
    public Animator GunAnim;
    public string[] AnimName = new string[(int)GunAnimName.Count];
    public ParticleSystem GunVFX;
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
            case GunAnimType.Fire:
                {
                    GunAnim.SetTrigger(AnimName[(int)GunAnimName.Fire]);
                    break;
                }
            case GunAnimType.Reload:
                {
                    GunAnim.SetTrigger(AnimName[(int)GunAnimName.Reload]);
                    GunAnim.SetTrigger(AnimName[(int)GunAnimName.Reloading]);
                    break;
                }
        }
    }
    public void ShowSFX()
    {
        GunVFX.Play();
    }
}
