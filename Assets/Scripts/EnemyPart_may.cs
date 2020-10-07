using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyPartType
{
    Head,
    Body,
    Hip,
    LeftUpperArm,
    LeftLowerArm,
    LeftHand,
    RightUpperArm,
    RightLowerArm,
    RightHand,
    LeftUpperLeg,
    LeftLowerLeg,
    LeftFoot,
    RightUpperLeg,
    RightLowerLeg,
    RightFoot
}

public class EnemyPart_may : MonoBehaviour
{
    public EnemyPartType _PartType;
    public float _Health = 100;
    private float _CurrentHealth;

    public GameObject mesh;
    public bool createLimbOnDismember = true;

    Rigidbody[] rbs;
    Collider[] cols;
    List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();

    private EnemyManager_may _Enemy;
    private float mass;
    // Start is called before the first frame update
    void Start()
    {
        _CurrentHealth = _Health;

        _Enemy = transform.root.gameObject.GetComponent<EnemyManager_may>();

        rbs = GetComponentsInChildren<Rigidbody>();
        cols = GetComponentsInChildren<Collider>();
        EnemyPart_may[] otherParts = GetComponentsInChildren<EnemyPart_may>();
        SkinnedMeshRenderer skinnedMesh_;

        for (int i = 0; i < otherParts.Length; i++)
        {
            skinnedMesh_ = otherParts[i].mesh.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh_)
                meshes.Add(skinnedMesh_);
        }
        mass = 0f;

        // calculate mass of entire limb (with children)
        for (int i = 0; i < rbs.Length; i++)
        {
            mass += rbs[i].mass;
        }
    }

    // Call this function to apply damage to the limb with force
    public void Damage(float damage)
    {
        _CurrentHealth -= damage;

        Debug.Log("Damaging limb: " + _PartType + " Health left: " + _CurrentHealth);

        if (_CurrentHealth <= 0f)
        {
            // Debug.Log("Dismembering");

            _CurrentHealth = 0f;

            //LimbDie(force);
            LimbDie();
        }
        if (_Enemy)
            _Enemy.BeAttacked(damage, _PartType == EnemyPartType.Head);
    }

    void LimbDie()
    {
        if (mesh == null)
            return;

        Rigidbody rb = null;

        if (createLimbOnDismember)
        {
            GameObject goRoot = new GameObject();

            goRoot.transform.position = mesh.transform.position;
            goRoot.transform.rotation = mesh.transform.rotation;

            // since using bakemesh this needs to be one
            goRoot.transform.localScale = Vector3.one;

            int index = 0;

            Transform parent = goRoot.transform;

            for (int i = 0; i < meshes.Count; i++)
            {
                SkinnedMeshRenderer smr = meshes[i];

                if (smr.enabled)
                {
                    GameObject go = new GameObject();

                    MeshRenderer mr = go.AddComponent<MeshRenderer>();
                    MeshFilter mf = go.AddComponent<MeshFilter>();

                    Mesh newMesh = new Mesh();

                    smr.BakeMesh(newMesh);
                    mf.sharedMesh = newMesh;
                    mr.materials = smr.materials;

                    mr.receiveShadows = false;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                    // hack/fix since physx can't create meshcolliders on complex meshes
                    if (newMesh.triangles.Length < 2048)
                    {
                        MeshCollider mc = go.AddComponent<MeshCollider>();

                        mc.convex = true;
                        mc.isTrigger = false;
                        mc.sharedMesh = newMesh;
                    }
                    else
                    {
                        BoxCollider mc = go.AddComponent<BoxCollider>();
                        mc.center = mf.mesh.bounds.center;
                        mc.size = mf.mesh.bounds.extents;
                        mc.isTrigger = false;
                    }

                    // only add rb to the first object so they dont seperate
                    if (index == 0)
                    {
                        rb = go.AddComponent<Rigidbody>();
                        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                        rb.mass = mass;
                    }

                    go.transform.position = smr.transform.position;
                    go.transform.rotation = smr.transform.rotation;

                    go.transform.parent = parent;

                    go.transform.localScale = Vector3.one;

                    parent = go.transform;

                    // hide the mesh
                    smr.enabled = false;

                    index++;
                }
            }

            Destroy(goRoot, 5f);
        }
        else
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].enabled = false;
            }
        }

        //disable attached colliders
        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }
        if (_PartType == EnemyPartType.LeftFoot || _PartType == EnemyPartType.LeftLowerLeg || _PartType == EnemyPartType.LeftUpperLeg ||
            _PartType == EnemyPartType.RightFoot || _PartType == EnemyPartType.RightLowerLeg || _PartType == EnemyPartType.RightUpperLeg)
        {
            _Enemy.OnCripple();
        }


        if (_Enemy)
            _Enemy.OnLimbDie(_PartType, transform);
    }
}
