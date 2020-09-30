using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType
{
    Concrete,
    Dirt,
    Metal,
    Wood,
}
[System.Serializable]
public class Bulletmark
{
    public Material[] _Material;
}

[CreateAssetMenu(fileName = "New ImpactProcessor", menuName = "ScriptableObject/Impact Processor")]
public class ImpactProcessor : ScriptableObject
{
    public GameObject[] impactParticles;
    public GameObject[] _EnemyImpact;
    public GameObject bulletMarksPrefab;
    public Bulletmark[] Bullet_Material;

    public float impactLifeTime = 1f;
    public float bulletMarksLifeTime = 3f;
    public float bulletMarksNormalOffSet = 0.1f;

    
    public void ProcessImpact(Vector3 pos, Vector3 normal, ImpactType type,Transform parent = null)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(impactParticles[(int)type], pos, rot);
        if(go)
        {
            Destroy(go, impactLifeTime);
        }
        if (parent)
        {
            go.transform.parent = parent;
        }
        GameObject bm = Instantiate(bulletMarksPrefab, pos + normal * bulletMarksNormalOffSet, rot);
        if (bm)
        {
            BulletMark material = bm.GetComponent<BulletMark>();
            if (material)
            {
                material.SetMark(Bullet_Material[(int)type]._Material[Random.Range(0, Bullet_Material[(int)type]._Material.Length)]);
            }
            Destroy(bm, bulletMarksLifeTime);
        }
        if (parent)
        {
            bm.transform.parent = parent;
        }
    }
    public void EnemyImpact(Vector3 pos, Vector3 normal)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(_EnemyImpact[Random.Range(0,_EnemyImpact.Length)], pos, rot);
    }
}
