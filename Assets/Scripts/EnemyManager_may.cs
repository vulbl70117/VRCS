using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager_may : MonoBehaviour
{
    public float MaxHealth = 100f;

    private float Health =100f;

    private EnemyRenderer_may enemyRenderer;
    public ImpactProcessor_may _ImpactProcessor;

    private NavMeshAgent agent;
    public Transform target;
    public int MoveVelocity = 1;
    // Start is called before the first frame update
    void Start()
    {
        enemyRenderer = GetComponent<EnemyRenderer_may>();
        agent = GetComponent<NavMeshAgent>();
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

        agent.SetDestination(target.position);


        if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            enemyRenderer.animator.SetFloat("MoveVelocity", MoveVelocity);
            agent.speed = (enemyRenderer.animator.deltaPosition / Time.deltaTime).magnitude;
        }
        else
        {
            enemyRenderer.animator.SetFloat("MoveVelocity", 0);
            agent.speed = 0;
        }









        //if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        //{
        //    //enemyRenderer.animator.SetFloat("MoveVelocity", MoveVelocity);
        //    //agent.nextPosition = transform.position;
        //    //agent.updatePosition = true;
        //    //agent.updateRotation = true;
        //    //Cal_MoveVelocity = Mathf.Lerp(Cal_MoveVelocity, MoveVelocity, Time.deltaTime);
        //    //enemyRenderer.animator.SetFloat("MoveVelocity", Cal_MoveVelocity);
        //    enemyRenderer.animator.SetFloat("MoveVelocity", MoveVelocity);
        //    agent.speed = (enemyRenderer.animator.deltaPosition / Time.deltaTime).magnitude;
        //}
        //else
        //{
        //    //enemyRenderer.animator.SetFloat("MoveVelocity", 0);
        //    //agent.updatePosition = false;
        //    //agent.updateRotation = false;
        //    //Cal_MoveVelocity = 0;
        //    enemyRenderer.animator.SetFloat("MoveVelocity", 0);
        //    agent.speed = 0;
        //}
    }

    public void BeAttacked(float value,bool headshot = false)
    {
        Health -= value;

        if (Health <= 0 || headshot)
        {
            Health = 0;
            Debug.Log("Enemy Die");
            if (enemyRenderer)
                enemyRenderer.SetAnim(EnemyAnimType.Die);
        }

        //受擊
        if (enemyRenderer)
            enemyRenderer.SetAnim(EnemyAnimType.Hit);
        Debug.Log("Enemy BeAttacked/Health:" + Health);
    }
    public void OnCripple()
    {
        //爬行
        if (enemyRenderer)
            enemyRenderer.SetAnim(EnemyAnimType.Crawl);
    }
    public void OnLimbDie(EnemyPartType dmgType, Transform placement)
    {
        ////部位破壞，特效
        _ImpactProcessor.ProcessEnemyPartImpact(dmgType, placement);
    }
}
