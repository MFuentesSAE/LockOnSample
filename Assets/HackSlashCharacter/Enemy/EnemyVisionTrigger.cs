using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyVisionTrigger : MonoBehaviour
{
	public EnemyController enemyController;
	public SphereCollider visionCollider;
	public Transform player;
	public bool playerInRange;

	private void Start()
	{
		visionCollider.isTrigger = true;
	}

	private void Update()
	{
		if (!playerInRange)
		{
			return;
		}

		if (CanSeePlayer() && enemyController.OnIdleState())
		{
			Debug.Log($"<color=yellow>SeeingPlayer onIdleState: {enemyController.OnIdleState()} enemyController State: {enemyController.enemyState}</color>");
			enemyController.UpdateState(EnemyState.Aggro);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerInRange = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerInRange = false;
		}
	}

	private bool CanSeePlayer()
	{
		Ray visionRay = new Ray(transform.position + new Vector3(0,0.5f,0), player.position - transform.position);

		return Physics.Raycast(visionRay, visionCollider.radius);
	}
}
