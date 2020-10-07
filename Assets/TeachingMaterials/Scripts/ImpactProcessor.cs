using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Impact Processors")]
public class ImpactProcessor : ScriptableObject
{
    public GameObject[] impactParticles;
    public float impactLifetime = 1f;

    public GameObject bulletMarksPrefab;

    public BulletMarkMaterials[] bulletMarksMaterials;

    public float bulletMarksNormalOffset;

    public float bulletMarksLifetime = 60f;

    public GameObject[] enemyImpactParticles;

    public GameObject headBloodEffect;
    public GameObject defaultBloodEffect;
    public float enemyPartImpactLifetime = 2f;
    public float enemyPartImpactOffset = 0.04f;

    public void ProcessImpact(Vector3 pos, Vector3 normal, ImpactType materialType, Transform parent = null)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);

        GameObject go = Instantiate(impactParticles[(int)materialType], pos, rot);

        if (go)
            Destroy(go, impactLifetime);

        if (parent)
            go.transform.parent = parent;

        GameObject bm = Instantiate(bulletMarksPrefab, pos + normal * bulletMarksNormalOffset, rot);

        bm.GetComponent<BulletMarks>().SetMaterial(bulletMarksMaterials[(int)materialType].materials[Random.Range(0, bulletMarksMaterials[(int)materialType].materials.Length)], bulletMarksLifetime);

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
        position = position + direction* enemyPartImpactOffset;
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