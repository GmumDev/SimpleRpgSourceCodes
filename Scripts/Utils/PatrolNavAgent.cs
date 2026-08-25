using UnityEngine;
using UnityEngine.AI;

public class PatrolNavAgent : MonoBehaviour
{

    NavMeshAgent agent;

    [SerializeField]
    Transform[] target;

    public bool isRunning;
    int idx;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
		idx = 0;
        if (target.Length == 0) isRunning = false;
    }

    // Update is called once per frame
    void Update()
	{
        if (isRunning == false || ScenarioManager.Instance.IsPlaying)
        {
            agent.isStopped = true;
            return;
        }
        else
            agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.SetDestination(target[idx].position);
            idx = (idx + 1) % target.Length;
        }
    }
}
