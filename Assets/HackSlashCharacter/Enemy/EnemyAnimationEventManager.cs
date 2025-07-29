using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimationEventManager : MonoBehaviour
{
    [Space(10)]
    public UnityEvent onExecuteAttackEvent, onEnterIdleEvent;
}
