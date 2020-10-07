using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager_bk : MonoBehaviour
{
    public float _Health = 100;
    private float _CurrentHealth;

    public ImpactProcessor _ImpactProcessor;
    public GameObject headBloodEffect;
    public GameObject defaultBloodEffect;

    private EnemyRenderer enemyRenderer; 
    // Start is called before the first frame update
    void Start()
    {
        enemyRenderer = GetComponent<EnemyRenderer>();
        _CurrentHealth = _Health;
    }

    // Update is called once per frame
    void Update()
    {

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
