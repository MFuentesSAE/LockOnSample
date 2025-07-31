using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
	[Header("MainSettings")]
	public EnemyState enemyState;
	public EnemyState previousState;
	private float currentSpeed;
	public float patrolSpeed, lookAtSpeed, aggroSpeed, patrolRadius, arriveDistance, stunTime, patrolWaitTime;

	[Header("Components")]
	public NavMeshAgent agent;
	public PlayerController player;
	public Rigidbody rigidbody;
	public Animator animator;
	public List<AnimationClip> comboAttack;

	private Coroutine delegateCoroutine, attackCoroutine;
	private Vector3 currentTarget;
	private LockOnManager lockOnManager;

	private const string ANIM_SPEED = "WalkSpeed";
	private const string ANIM_HIT = "Hit";
	private const string ANIM_DEATH = "Death";
	private const string ANIM_END_ATTACK = "Idle";

	void Start()
	{
		rigidbody.isKinematic = true;
		UpdateState(EnemyState.Idle);
		lockOnManager = LockOnManager.instance;
	}

	void Update()
	{
		Debug.DrawRay(transform.position, currentTarget - transform.position, Color.red);

		if (OnStunnedState())
		{
			return;
		}

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
					break;

				case EnemyState.Attack:
					SetDestination(aggroSpeed, player.transform.position);
					LookAtTarget();
					break;
			}
		}
		else if (OnAggroState())
		{
			SetDestination(aggroSpeed, player.transform.position);
		}

		UpdateAnimatorMovement();
	}

	public bool OnIdleState()
	{
		return enemyState == EnemyState.Idle || enemyState == EnemyState.Patrol;
	}

	public bool OnAggroState()
	{
		return enemyState == EnemyState.Aggro || enemyState == EnemyState.Attack || enemyState == EnemyState.Stunned;
	}

	public bool OnStunnedState()
	{
		return enemyState == EnemyState.Stunned || enemyState == EnemyState.Dead;
	}

	public void UpdateState(EnemyState state)
	{
		if(enemyState == EnemyState.Dead)
		{
			return;
		}

		previousState = enemyState;
		enemyState = state;

		switch (state)
		{
			case EnemyState.Idle:
				TogglePhysics(false);
				rigidbody.isKinematic = true;
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
				ForceStopAttack();
				animator?.SetTrigger(ANIM_HIT);
				Stop();
				DelayedInvoke(stunTime, () => UpdateState(EnemyState.Idle));
				break;

			case EnemyState.Attack:
				Attack();
				break;

			case EnemyState.Dead:
				TogglePhysics(false);
				StopAllCoroutines();
				animator?.SetTrigger(ANIM_DEATH);
				Stop();
				EndLockOn();
				break;
		}
	}

	public void DelayedInvoke(float waitTime, Action function)
	{
		if (delegateCoroutine != null)
		{
			StopCoroutine(delegateCoroutine);
		}

		delegateCoroutine = StartCoroutine(DelayedInvokeRoutine(waitTime, function));
	}

	private bool HasReachedTarget()
	{
		return Vector3.Distance(transform.position, currentTarget) <= arriveDistance;
	}

	private void SetDestination(float speed, Vector3 target)
	{
		currentSpeed = speed;
		currentTarget = target;

		if (!agent.enabled)
		{
			return;
		}

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
		animator?.SetFloat(ANIM_SPEED, agent.velocity.magnitude);
	}

	public void Attack()
	{
		TogglePhysics(true);
		if (attackCoroutine != null)
		{
			StopCoroutine(attackCoroutine);
		}

		attackCoroutine = StartCoroutine(AttackRoutine());
	}

	private void ForceStopAttack()
	{
		TogglePhysics(false);
		if (attackCoroutine != null)
		{
			StopCoroutine(attackCoroutine);
		}
	}

	private void LookAtTarget()
	{
		Debug.Log($"<color=cyan>LookAtTarget</color>");
		Vector3 direction = currentTarget - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookAtSpeed * Time.deltaTime);
	}

	private void EndLockOn()
	{
		if(lockOnManager == null)
		{
			return;
		}

		if(lockOnManager.currentTarget == transform)
		{
			lockOnManager.EndLockOn();
		}
	}

	private void TogglePhysics(bool value)
	{
		rigidbody.isKinematic = !value;
		agent.enabled = !value;
	}

	private IEnumerator DelayedInvokeRoutine(float waitTime, Action function)
	{
		yield return new WaitForSeconds(waitTime);
		function?.Invoke();
	}

	private IEnumerator AttackRoutine()
	{
		foreach (AnimationClip clip in comboAttack)
		{
			animator.CrossFade(clip.name, 0.1f);
			yield return new WaitForSeconds(clip.length);
		}

		animator?.SetTrigger(ANIM_END_ATTACK);
		UpdateState(EnemyState.Idle);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(transform.position, patrolRadius);
		Gizmos.DrawSphere(currentTarget, 0.25f);
	}
}
