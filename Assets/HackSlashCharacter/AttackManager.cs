using UnityEngine;
using DG.Tweening;
using System;

public class AttackManager : MonoBehaviour
{
    public PlayerController playerController;
    public AnimatorController animatorController;
    public float force, tweenTime;
    private Tween impulseTween;
    private bool blockAttack;

    [SerializeField]
    private int currentAttackCount;
    private const int MAX_ATTACK_COUNT = 2; 

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !blockAttack)
        {
    		playerController?.ToggleMovement(false);

            currentAttackCount++;
            if(currentAttackCount > MAX_ATTACK_COUNT)
            {
                currentAttackCount = 1;
			}

            animatorController?.AttackTrigger(currentAttackCount);
            blockAttack = true;
		}
    }

    public void AddImpusle()
    {
		Vector3 direction = transform.forward * force;
		Vector3 impulseVector = playerController.controller.velocity;

		impulseTween?.Kill();

		impulseTween = DOTween.To(() => 0f, x => { playerController.controller.Move(direction * Time.deltaTime); }, 1f, tweenTime)
			.SetEase(Ease.Linear);
	}

    public void UnlockAttack()
    {
		blockAttack = false;
		playerController?.ToggleMovement(true);

	}
}
