using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum EnemyPartType
//{
//    Head,
//    Body,
//    Hip,
//    LeftUpperArm,
//    LeftLowerArm,
//    LeftHand,
//    RightUpperArm,
//    RightLowerArm,
//    RightHand,
//    LeftUpperLeg,
//    LeftLowerLeg,
//    LeftFoot,
//    RightUpperLeg,
//    RightLowerLeg,
//    RightFoot
//}

public class EnemyPart : MonoBehaviour
{
    public float _Health = 100;
    public EnemyPartType _PartType;
    private float _CurrentHealth;

    private EnemyManager _Enemy;

    public GameObject mesh;

    public bool createLimbOnDismember = true;

    Rigidbody[] rbs;

    Collider[] cols;

    List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();

    float mass; // the combined mass for the limb
    // Start is called before the first frame update
    void Start()
    {
        _CurrentHealth = _Health;

        // if this was manually created we need to set the reference to the manager
        if (_Enemy == null)
        {
            _Enemy = transform.root.gameObject.GetComponent<EnemyManager>();

            if (_Enemy == null)
            {
                Debug.LogError("You need to add the EnemyManager script to the root object (" + transform.root.name + ").");
            }
        }

        if (mesh == null)
        {
            //Debug.LogError ("dismember setup error !!! No mesh assigned to bone");
            return;
        }

        cols = GetComponentsInChildren<Collider>();
        rbs = GetComponentsInChildren<Rigidbody>();

        EnemyPart[] otherParts = GetComponentsInChildren<EnemyPart>();

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

    // it creates a copy of the affected meshes, generates colliders, disables the now unused stuff (RigidBodies, Colliders, meshes etc)
    // and finally spawns the bloodeffect and executes the event
    void LimbDie(Vector3 force)
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
                        mc.inflateMesh = true;
                        mc.skinWidth = .02f;
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

        //disable any particle systems that might be playing ( limited to one, since only one is present at a time
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        if (ps)
        {
            Destroy(ps);
        }

        if (_PartType == EnemyPartType.LeftFoot || _PartType == EnemyPartType.LeftLowerLeg || _PartType == EnemyPartType.LeftUpperLeg ||
            _PartType == EnemyPartType.RightFoot || _PartType == EnemyPartType.RightLowerLeg || _PartType == EnemyPartType.RightUpperLeg)
        {
            _Enemy.OnCripple();
        }

        _Enemy.OnLimbDie(_PartType, transform);


        if (rb)
            rb.AddForce(force, ForceMode.Impulse);
    }

    // Call this function to apply damage to the limb with force
    public void Damage(float damage, Vector3 force, bool canDismember = true)
    {
        _CurrentHealth -= damage;

        //Debug.Log ("Damaging limb: " + mesh.name + " Health left: " + health + " canDismember: " + canDismember);

        if (_CurrentHealth <= 0f && canDismember)
        {
            // Debug.Log("Dismembering");

            _CurrentHealth = 0f;

            LimbDie(force);
        }
        if (_Enemy)
            _Enemy.OnDamage(damage, _PartType == EnemyPartType.Head);
    }
}
