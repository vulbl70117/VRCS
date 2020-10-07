using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsMask_may : MonoBehaviour
{
    public MeshRenderer mesh;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetMaterial(Material mat)
    {
        transform.Rotate(Vector3.up, Random.Range(0, 360f));
        mesh.material = mat;
    }
}
