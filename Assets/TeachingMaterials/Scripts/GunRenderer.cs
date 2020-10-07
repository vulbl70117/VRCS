using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GunAnimParametersName_
{
    Shoot,
    Reload,
    Reloading,


    Count
}
public class GunRenderer : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public Animator anim;
    public string[] anim_ParametersName = new string[(int)GunAnimParametersName.Count];

    public Renderer[] allWeaponRenderers;

    [Header("Muzzle Flash")]
    public ParticleSystem muzzleFlash;

    [Header("UI")]
    public Animator animUI;
    public Text BulletTxt;

    //#region Cached values
    //[HideInInspector]
    //public bool cachedMuzzleFlashEnabled;

    //[HideInInspector]
    //public AudioClip cachedFireSound;

    //[HideInInspector]
    //public AudioClip cachedFireSoundThirdPerson;

    //[HideInInspector]
    //public float cachedFireSoundThirdPersonMaxRange = 300f;

    //[HideInInspector]
    //public AnimationCurve cachedFireSoundThirdPersonRolloff = AnimationCurve.EaseInOut(0f, 1f, 300f, 0f);
    //#endregion

#if UNITY_EDITOR
    void OnEnable()
    {
        for (int i = 0; i < allWeaponRenderers.Length; i++)
        {
            if (!allWeaponRenderers[i])
            {
                Debug.LogError("Weapon renderer from " + gameObject.name + " at index " + i + " not assigned.");
            }
        }
    }
#endif

    public bool visible
    {
        get
        {
            for (int i = 0; i < allWeaponRenderers.Length; i++)
            {
                if (!allWeaponRenderers[i].enabled)
                    return false;
            }

            return true;
        }
        set
        {
            for (int i = 0; i < allWeaponRenderers.Length; i++)
            {
                allWeaponRenderers[i].enabled = value;
            }
        }
    }

    //public void SetFireEffect()
    //{
    //    cachedMuzzleFlashEnabled = true;
    //}

    public void DisplayAmmo(int bl)
    {
        BulletTxt.text = bl.ToString("000");
    }
    public void DisplayAmmoReloadHint()
    {
        if (animUI && !animUI.GetBool("Reload") && !animUI.GetBool("Reloading"))
        {
            animUI.SetTrigger("Reload");
            animUI.SetTrigger("Reloading");
        }
    }

    public void SetAnim(GunAnimType type)
    {
        switch (type)
        {
            case GunAnimType.Shoot:
                {
                    anim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName_.Shoot]);
                    break;
                }
            case GunAnimType.Reload:
                {
                    anim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName_.Reload]);
                    anim.SetTrigger(anim_ParametersName[(int)GunAnimParametersName_.Reloading]);
                    break;
                }
        }
    }
    public void SetmuzzleFlash()
    {
        if (muzzleFlash)
        {
            muzzleFlash.Play(true);
        }
    }
}