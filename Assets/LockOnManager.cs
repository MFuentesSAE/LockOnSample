using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.Cinemachine;

public class LockOnManager : MonoBehaviour
{
    public float sphereRadius;
    public float lockOnRotationSpeed;
    public float maxLockOnDistance;
    public bool lockingOn;
    private string targetTag = "Enemy";
    public LayerMask searchMask;
    public KeyCode lockonKey;

    public Transform currentTarget;
    public List<Transform> targetList, elapsedTargetList;

    public UnityEvent onLockOnEvent, onEndLockOnEvent;

    public CinemachineInputAxisController cinemachineController;
    public CinemachineOrbitalFollow orbitalFollow;


    void Start()
    {
        
    }

    void Update()
    {
        //Seguir al objetivo
        if (lockingOn)
        {
            LookAtTaget();
            if(Vector3.Distance(transform.position, currentTarget.position) >= maxLockOnDistance)
            {
                EndLockOn();
                return;
            }
        }

        //Input para iniciar el lockon
        if (Input.GetKeyDown(lockonKey))
        {
            if (lockingOn == true)
            {
                EndLockOn();
                return;
            }

            SearchTargets();

            lockingOn = targetList.Count > 0;

            if (lockingOn)
            {
				currentTarget = GetClosestTarget();
				onLockOnEvent.Invoke();
                LockCamera(true);
            }
        }

        //Input para cambiar de objetivo
        if (Input.GetMouseButtonDown(2) && lockingOn)
        {
            SearchTargets();

            if(elapsedTargetList.Count >= targetList.Count) 
            {
                elapsedTargetList.Clear();
            }
            currentTarget = GetClosestTarget();
        }
    }

    private void SearchTargets()
    {
        Collider[] objectsFound = Physics.OverlapSphere(transform.position, sphereRadius, searchMask);
        for(int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].CompareTag(targetTag) && targetList.Contains(objectsFound[i].transform) == false)
            {
                targetList.Add(objectsFound[i].transform);
            }
        }
    }

    private Transform GetClosestTarget()
    {
        if(targetList.Count <= 0)
        {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestTraget = null;

        //Buscar al enemigo más cercano al jugador
        for (int i = 0; i < targetList.Count; i++)
        {

            //Si el target a evaluar ya esta en la lista de ignorados, saltarse al sigiente candidato.
            if (elapsedTargetList.Contains(targetList[i]))
            {
                continue;
            }

            float distanceToTarget = Vector3.Distance(transform.position, targetList[i].position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTraget = targetList[i];
            }
        }

        if (!elapsedTargetList.Contains(closestTraget) && closestTraget != null)
        {
            elapsedTargetList.Add(closestTraget);
        }

        return closestTraget;
    }

    private void LookAtTaget()
    {
        if(currentTarget == null)
        {
            return;
        }

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0;

        Quaternion lockOnRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lockOnRotation, lockOnRotationSpeed * Time.deltaTime);
    }

    private void LockCamera(bool locked)
    {
        if (locked)
        {
            cinemachineController.enabled = false;
            orbitalFollow.HorizontalAxis.Recentering.Enabled = true;
            orbitalFollow.VerticalAxis.Recentering.Enabled = true;

            //Hacer que la cámara siga la rotación del personaje;
            orbitalFollow.TrackerSettings.BindingMode = Unity.Cinemachine.TargetTracking.BindingMode.LockToTargetWithWorldUp;
        }
        else
        {
            cinemachineController.enabled = true;
            orbitalFollow.HorizontalAxis.Recentering.Enabled = false;
            orbitalFollow.VerticalAxis.Recentering.Enabled = false;

            //Hacer que la cámara NO siga la rotación del personaje;
            orbitalFollow.TrackerSettings.BindingMode = Unity.Cinemachine.TargetTracking.BindingMode.WorldSpace;
        }
    }

    private void EndLockOn()
    {
        lockingOn = false;
        currentTarget = null;
        targetList.Clear();
        elapsedTargetList.Clear();
        LockCamera(false);
        onEndLockOnEvent.Invoke();
    }

    private void OnDrawGizmos()
    {
        Color color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
