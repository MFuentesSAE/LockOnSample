using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public Transform pivot;
    public LayerMask hitMask;
    public float hitRadius;
    protected int damage = 1;

    public virtual void EnableHitbox()
    {
        if(pivot == null)
        {
            pivot = transform;
		}

		Collider[] colliders = Physics.OverlapSphere(pivot.position, hitRadius, hitMask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Hp hp = colliders[i].GetComponent<Hp>();
            hp?.TakeDamage(damage);
        }
	}

	protected void OnDrawGizmos()
	{

		if (pivot == null)
		{
            return;
		}

		Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pivot.position, hitRadius);
	}
}
