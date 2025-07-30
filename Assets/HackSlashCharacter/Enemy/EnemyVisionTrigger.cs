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

		if (CanSeePlayer() && !enemyController.OnAggroState())
		{
			Debug.Log("SeeingPlayer");
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
		Ray visionRay = new Ray(transform.position, player.position - transform.position);

		return Physics.Raycast(visionRay, visionCollider.radius);
	}
}
