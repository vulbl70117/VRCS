using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r_ = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit_;
            if (Physics.Raycast(r_, out hit_, 500))
            {
                agent.SetDestination(hit_.point);
            }
        }
        //agent.SetDestination(target.position);
    }
}
