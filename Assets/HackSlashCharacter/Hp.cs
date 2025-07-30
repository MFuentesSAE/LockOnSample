using UnityEngine;
using UnityEngine.Events;

public class Hp : MonoBehaviour
{
    [Range(1, 10)]
    public int maxHp;
    protected int currentHp;

    [Space(10)]
    public UnityEvent onTakeDamageEvent, onDieEvent;

	protected virtual void Start()
    {
        currentHp = maxHp;
	}

    public virtual void TakeDamage(int amount)
    {
        currentHp -= amount;
        if(currentHp <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        currentHp = 0;
        onDieEvent?.Invoke();
	}
}
