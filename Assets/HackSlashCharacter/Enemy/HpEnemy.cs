using UnityEngine;

public class HpEnemy : Hp
{
	public EnemyController enemyController;

	public override void TakeDamage(int amount)
	{
		base.TakeDamage(amount);
		enemyController?.UpdateState(EnemyState.Stunned);
	}

	public override void Die()
	{
		base.Die();
		enemyController?.UpdateState(EnemyState.Dead);
	}
}
