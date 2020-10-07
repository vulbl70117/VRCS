using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAnimType
{
    Hit,
    Die,
    Crawl
}
public class EnemyRenderer_may : MonoBehaviour
{
    public Animator animator;
    public void SetAnim(EnemyAnimType type)
    {
        if (animator == null)
            return;

        switch (type)
        {
            case EnemyAnimType.Hit:
                {
                    animator.SetTrigger("Hit");
                    break;
                }
            case EnemyAnimType.Die:
                {
                    animator.SetTrigger("Die");
                    break;
                }
            case EnemyAnimType.Crawl:
                {
                    animator.SetTrigger("Crawl");
                    break;
                }
        }
    }
}
