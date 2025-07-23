using UnityEngine;
using UnityEngine.Rendering;

public class AnimatorController : MonoBehaviour
{
    private const string MOVE_SPEED = "MoveSpeed";
    private const string MOVE_X = "MoveX";
    private const string MOVE_Y = "MoveY";
	private const string ATTACK_TRIGGER = "Attack";
	private const string ATTACK_STRAFE = "Strafe";
	private const string ATTACK_COUNTER = "AttackCount";

	public Animator animator;

	public void SetMovementSpeed(Vector3 movement)
	{
		animator?.SetFloat(MOVE_SPEED, movement.sqrMagnitude);
		animator?.SetFloat(MOVE_X, movement.x);
		animator?.SetFloat(MOVE_Y, movement.z);
	}
	public void SetMovementStrafe(bool starfeValue)
	{
		animator?.SetBool(ATTACK_STRAFE, starfeValue);
	}
	public void AttackTrigger(int attackCount)
	{
		animator?.SetTrigger(ATTACK_TRIGGER);	
		animator?.SetInteger(ATTACK_COUNTER, attackCount);
	}
}
