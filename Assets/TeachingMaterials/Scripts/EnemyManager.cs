using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public float _Health = 100;
    private float _CurrentHealth;

    public ImpactProcessor _ImpactProcessor;
    public GameObject headBloodEffect;
    public GameObject defaultBloodEffect;

    private EnemyRenderer enemyRenderer;
    public NavMeshAgent agent;
    public Transform target;
    public int MoveVelocity = 1;
    private float Cal_MoveVelocity = 0;
    public float ChaseAngleThreshold = 120;
    // Start is called before the first frame update
    void Start()
    {
        enemyRenderer = GetComponent<EnemyRenderer>();
        _CurrentHealth = _Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T))
            Chase();
        else if (Input.GetKeyUp(KeyCode.T))
        {
            Idle();
        }
    }

    private void Chase()
    {
        if (target == null)
            return;

        if (!enemyRenderer.animator.GetBool("ZombieIsTurning"))
        {
            float Angle = GetAngle(transform.forward, agent.desiredVelocity, transform.up);
            Debug.Log(Angle);

            if (Angle > ChaseAngleThreshold || Angle < -ChaseAngleThreshold)
            {
                Idle();
                int val = Mathf.Sign(Angle) > 0 ? 1 : -1;

                enemyRenderer.animator.SetInteger("ZombieTurn", val);
                return;
            }
        }
        else
            return;

        agent.SetDestination(target.position);
        if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            agent.nextPosition = transform.position;
            agent.updatePosition = true;
            agent.updateRotation = true;
            Cal_MoveVelocity = Mathf.Lerp(Cal_MoveVelocity, MoveVelocity, Time.deltaTime);
            enemyRenderer.animator.SetFloat("MoveVelocity", Cal_MoveVelocity);
            agent.speed = (enemyRenderer.animator.deltaPosition / Time.deltaTime).magnitude;
        }
        else
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            Cal_MoveVelocity = 0;
            enemyRenderer.animator.SetFloat("MoveVelocity", 0);
            agent.speed = 0;
        }
    }
    float GetAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        if (toVector == Vector3.zero)
            return 0;

        float Angle = Vector3.Angle(fromVector, toVector);

        Vector3 normal = Vector3.Cross(fromVector, toVector);

        Angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

        return Angle;
    }
    private void Idle()
    {
        agent.updateRotation = false;
        agent.updatePosition = false;
        Cal_MoveVelocity = 0;
        enemyRenderer.animator.SetFloat("MoveVelocity", 0);
        agent.speed = 0;
    }
    public void OnDamage(float damage, bool headshot)
    {
        _CurrentHealth -= damage;

        if (_CurrentHealth <= 0 || headshot)
        {
            _CurrentHealth = 0;
            Debug.Log("Enemy Die");
            if (enemyRenderer)
                enemyRenderer.SetAnim(EnemyAnimType.Die);
        }

        //受擊
        if (enemyRenderer)
            enemyRenderer.SetAnim(EnemyAnimType.Hit);
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
