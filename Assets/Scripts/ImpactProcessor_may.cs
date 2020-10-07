using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region enums
public enum ImpactType
{
    Concrete,
    Dirt,
    Metal,
    Wood,
};
#endregion
[System.Serializable]
public class BulletMarkMaterials
{
    public Material[] materials;
}
[CreateAssetMenu(fileName = "New ImpactProcessor",menuName = "ScriptableObject /Impact Processor")]
public class ImpactProcessor_may : ScriptableObject
{
    public GameObject[] impactParticles;
    public GameObject bulletMarksPrefab;

    public BulletMarkMaterials[] bulletMarksMaterials;
    public float impactLifetime = 1f;
    public float bulletMarksLifetime = 3f;
    public float bulletMarksNormalOffset = 0.1f;

    public GameObject[] enemyImpactParticles;

    public GameObject headBloodEffect;
    public GameObject defaultBloodEffect;
    public float enemyPartImpactOffset = 0.04f;
    public float enemyPartImpactLifetime = 2f;
    public void ProcessImpact(Vector3 pos, Vector3 normal, ImpactType type, Transform parent = null)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(impactParticles[(int)type], pos, rot);
        if (go)
            Destroy(go, impactLifetime);
        if (parent)
            go.transform.parent = parent;


        GameObject bm = Instantiate(bulletMarksPrefab, pos + normal * bulletMarksNormalOffset, rot);
        if (bm)
        {
            BulletsMask_may bmCode_ = bm.GetComponent<BulletsMask_may>();
            if (bmCode_)
                bmCode_.SetMaterial(bulletMarksMaterials[(int)type].materials[Random.Range(0, bulletMarksMaterials[(int)type].materials.Length)]);
            Destroy(bm, bulletMarksLifetime);
        }
        if (parent)
            bm.transform.parent = parent;
    }

    public void ProcessEnemyImpact(Vector3 pos, Vector3 normal)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
        GameObject go = Instantiate(enemyImpactParticles[Random.Range(0, enemyImpactParticles.Length)], pos, rot);
        if (go)
            Destroy(go, impactLifetime);
    }
    public void ProcessEnemyPartImpact(EnemyPartType dmgType, Transform placement)
    {
        Vector3 position = placement.position;
        Vector3 direction = (placement.position - placement.parent.position).normalized;
        position = position + direction * enemyPartImpactOffset;
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, direction);
        GameObject effect = null;
        //部位破壞，特效
        if (dmgType == EnemyPartType.Head)
        {
            if (headBloodEffect)
            {
                effect = Instantiate(headBloodEffect, position, rot);
            }
        }
        else
        {
            if (defaultBloodEffect)
            {
                effect = Instantiate(defaultBloodEffect, position, rot);
            }
        }
            if (effect)
            {
                // attach the effect to the mesh
                effect.transform.parent = placement;

                Destroy(effect, enemyPartImpactLifetime);
            }
    }
}
