using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
	public EnemyState enemyState;
	public NavMeshAgent agent;
	public PlayerController player;
	public Animator animator;

	private float currentSpeed;
	private Coroutine coroutine;
	private Vector3 currentTarget;
	public float patrolSpeed, lookAtSpeed, aggroSpeed, patrolRadius, arriveDistance, stunTime, patrolWaitTime;
	private int attackCount;

	private const string ANIM_SPEED = "WalkSpeed";
	private const string ANIM_ATTACK = "Attack";
	private const string ANIM_ATTACK_COUNTER = "AttackCounter";

	void Start()
	{
		UpdateState(EnemyState.Idle);
	}

	// Update is called once per frame
	void Update()
	{
		if (HasReachedTarget())
		{
			Stop();
			switch (enemyState) 
			{ 
				case EnemyState.Patrol:
					UpdateState(EnemyState.Idle);
					break;

				case EnemyState.Aggro:
					UpdateState(EnemyState.Attack);
					SetDestination(aggroSpeed, player.transform.position);
					break;

				case EnemyState.Attack:
					LookAtTarget();
					break;
			}
		}

		UpdateAnimatorMovement();
	}

	public bool OnAggroState()
	{
		return enemyState == EnemyState.Aggro || enemyState == EnemyState.Attack || enemyState == EnemyState.Stunned;
	}

	public void UpdateState(EnemyState state)
	{
		enemyState = state;

		switch (state)
		{
			case EnemyState.Idle:
				SetDestination(patrolSpeed, GetRandomPoint());
				Stop();
				DelayedInvoke(patrolWaitTime, () => UpdateState(EnemyState.Patrol));
				break;

			case EnemyState.Patrol:

				SetDestination(patrolSpeed, GetRandomPoint());
				break;

			case EnemyState.Aggro:
				SetDestination(aggroSpeed, player.transform.position);
				break;

			case EnemyState.Stunned:
				Stop();
				DelayedInvoke(stunTime, () => UpdateState(EnemyState.Idle));
				break;

			case EnemyState.Attack:
				UpdateAnimationAttack();
				DelayedInvoke(patrolWaitTime, () => UpdateState(EnemyState.Idle));
				break;

			case EnemyState.Dead:
				Stop();
				break;
		}
	}

	public void DelayedInvoke(float waitTime, Action function)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
		}

		coroutine = StartCoroutine(DelayedInvokeRoutine(waitTime, function));
	}

	private bool HasReachedTarget()
	{
		return Vector3.Distance(transform.position, currentTarget) <= arriveDistance;
	}

	private void SetDestination(float speed, Vector3 target)
	{
		currentSpeed = speed;
		currentTarget = target;
		agent.destination = currentTarget;
		agent.speed = currentSpeed;
	}

	private Vector3 GetRandomPoint()
	{

		Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;

		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
		{
			return hit.position;
		}

		return Vector3.zero;
	}

	private void Stop()
	{
		agent.speed = 0;
	}

	private void UpdateAnimatorMovement()
	{
		animator?.SetFloat(ANIM_SPEED, currentSpeed);
	}

	private void UpdateAnimationAttack()
	{
		//attackCount++;
		animator?.SetTrigger(ANIM_ATTACK);
	}

	private void LookAtTarget()
	{
		Vector3 direction = currentTarget - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookAtSpeed * Time.deltaTime);
	}

	private IEnumerator DelayedInvokeRoutine(float waitTime, Action function)
	{
		yield return new WaitForSeconds(waitTime);
		function?.Invoke();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(transform.position, patrolRadius);
		Gizmos.DrawSphere(currentTarget, 0.25f);
	}
}
