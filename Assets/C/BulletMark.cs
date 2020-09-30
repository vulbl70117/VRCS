using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMark : MonoBehaviour
{
    public MeshRenderer _Mark;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetMark(Material _mark)
    {
        transform.Rotate(Vector3.up, Random.Range(0, 360));
        _Mark.material = _mark;
    }
}
