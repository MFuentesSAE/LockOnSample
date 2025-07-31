using UnityEngine;

public class AttackHitboxEnemy : AttackHitbox
{
    public Rigidbody rigidbody;
    public float impulseForce = 250;

    public void AddImpulse()
    {
        rigidbody?.AddForce(transform.forward * impulseForce, ForceMode.Acceleration);
    }
}
