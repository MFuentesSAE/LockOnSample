using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Cinemachine;

public class LockOn : MonoBehaviour
{
	public int enemyLayer;
	public float searchRadius, rotationLockOnSpeed, distanceThreshold;
	public LayerMask searchMask;
	private bool lockon;

	public KeyCode toggleLockOnKey;

	[Space(10)]
	public UnityEvent onLockOnEvent, onEndLockOnEvent;

	[Space(10)]
	public CinemachineCamera virtualCamera;
	public CinemachineOrbitalFollow orbitalFollow;
	public CinemachineInputAxisController inputAxisController;

	private Transform currentTarget;

	[SerializeField]
	private List<Transform> lockOnTargets = new List<Transform>();

	[SerializeField]
	private List<Transform> elapsedTargets = new List<Transform>();

	private void Start()
	{
		orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();
	}

	private void Update()
	{
		if (lockon)
		{
			LookAtLockTarget();
			DistanceLockOnEnd();
		}

		if (Input.GetKeyDown(toggleLockOnKey))
		{
			if (!lockon)
			{
				lockon = SearchLockOnTargets();
				SwitchTargets();
				onLockOnEvent?.Invoke();
			}
			else
			{
				lockOnTargets.Clear();
				elapsedTargets.Clear();
				EndLockOn();
			}

			LockCamera(lockon);
		}

		if (Input.GetMouseButtonDown(2) && lockon)
		{
			SwitchTargets();
			SearchLockOnTargets();
		}
	}

	private bool SearchLockOnTargets()
	{
		Collider[] targets = Physics.OverlapSphere(transform.position, searchRadius, searchMask);

		for (int i = 0; i < targets.Length; i++)
		{
			if (lockOnTargets.Contains(targets[i].transform))
			{
				continue;
			}

			Ray visionRay = new Ray(transform.position, targets[i].transform.position - transform.position);
			if (Physics.Raycast(visionRay, searchRadius))
			{
				lockOnTargets.Add(targets[i].transform);
			}
		}


		return lockOnTargets.Count > 0;
	}

	private Transform GetClosestTarget()
	{
		if (lockOnTargets == null || lockOnTargets.Count <= 0)
		{
			return null;
		}

		float closestDistance = Mathf.Infinity;
		Transform closestTarget = null;

		for (int i = 0; i < lockOnTargets.Count; i++)
		{
			if (elapsedTargets.Contains(lockOnTargets[i]))
			{
				continue;
			}

			float distance = Vector3.Distance(transform.position, lockOnTargets[i].position);
			if (distance < closestDistance)
			{
				closestTarget = lockOnTargets[i];
				closestDistance = distance;
			}
		}

		if (closestTarget != null)
		{
			elapsedTargets.Add(closestTarget);
		}

		return closestTarget;
	}

	private void SwitchTargets()
	{
		if (elapsedTargets.Count >= lockOnTargets.Count)
		{
			elapsedTargets.Clear();
		}

		currentTarget = GetClosestTarget();
	}

	private void LookAtLockTarget()
	{
		if (currentTarget == null)
		{
			return;
		}

		Vector3 direction = currentTarget.transform.position - transform.position;
		direction.y = 0;

		Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (rotationLockOnSpeed) * Time.deltaTime);
	}

	private void DistanceLockOnEnd()
	{
		if (currentTarget == null)
		{
			return;
		}

		float distance = Vector3.Distance(transform.position, currentTarget.position);
		if (distance >= distanceThreshold)
		{
			EndLockOn();
		}
	}

	private void EndLockOn()
	{
		lockon = false;
		LockCamera(false);
		onEndLockOnEvent?.Invoke();
		elapsedTargets.Clear();
		lockOnTargets.Clear();
	}

	private void LockCamera(bool lockCamera)
	{
		if (lockCamera)
		{
			orbitalFollow.TrackerSettings.BindingMode = Unity.Cinemachine.TargetTracking.BindingMode.LockToTargetWithWorldUp;
			inputAxisController.enabled = false;
			orbitalFollow.HorizontalAxis.Recentering.Enabled = true;
			orbitalFollow.VerticalAxis.Recentering.Enabled = true;
		}
		else
		{
			orbitalFollow.TrackerSettings.BindingMode = Unity.Cinemachine.TargetTracking.BindingMode.LazyFollow;
			inputAxisController.enabled = true;
			orbitalFollow.HorizontalAxis.Recentering.Enabled = false;
			orbitalFollow.VerticalAxis.Recentering.Enabled = false;
		}
	}
}