using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        SetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<EnemyHandler>().HP > 0.0f && GameObject.Find("GameHandler").GetComponent<GameLogic>().disableAI == false)
        {
            agent.SetDestination(target.position);
        }
    }
    public void ClearTarget()
    {
        target = gameObject.transform;
    }

    public void SetTarget()
    {
        target = GameObject.Find("GameHandler").GetComponent<GameLogic>().currentTarget.transform;
    }

    public void SetTargetName(GameObject objectSelected)
    {
        target = objectSelected.transform;
    }

    public Vector3 GetTargetPos()
    {
        if (target)
        {
            return target.transform.position;
        }
        return Vector3.zero;
    }
}
