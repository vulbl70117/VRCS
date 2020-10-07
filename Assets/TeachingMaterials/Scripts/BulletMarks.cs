using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMarks : MonoBehaviour
{
    public Renderer bulletMarksRenderer;

    public void SetMaterial(Material mat, float lifeTime)
    {
        transform.Rotate(Vector3.up, Random.Range(0, 360f));

        bulletMarksRenderer.sharedMaterial = mat;

        transform.localScale = Vector3.one;

        Destroy(gameObject, lifeTime);
    }
}
