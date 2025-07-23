using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyState enemyState;
    public NavMeshAgent agent;
    public PlayerController player;
    public SphereCollider visionTrigger;

    private float currentSpeed;
    private float waitTime = 1;
    private Vector3 currentTarget;
    public float patrolSpeed, aggroSpeed, patrolRadius, arriveDistance;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

	}

    private void UpdateState(EnemyState state)
    {
        enemyState = state;

        switch (state)
        {
            case EnemyState.Idle:
                currentSpeed = 0;
				currentTarget = GetRandomPoint();
				break;

			case EnemyState.Patrol:
                currentSpeed = patrolSpeed;
				break;

			case EnemyState.Aggro:
                currentSpeed = aggroSpeed;
				currentTarget = player.transform.position;
				break;

			case EnemyState.Stunned:
				break;

			case EnemyState.Attack:
				break;

            case EnemyState.Dead:
				break;
		}
    }

    private void SetMovement(float speed, Vector3 target)
    {
		currentSpeed = speed;
        currentTarget = target;
	}

    private Vector3 GetRandomPoint()
    {
        return Random.insideUnitSphere * patrolRadius;
	}

    private bool HasReachedTarget()
    {
        return Vector3.Distance(transform.position, currentTarget) > arriveDistance;
	}
}
