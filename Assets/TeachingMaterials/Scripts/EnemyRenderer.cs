using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum EnemyAnimType
//{
//    Hit,
//    Die,
//    Crawl
//}
public class EnemyRenderer : MonoBehaviour
{
    public Animator animator;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

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
