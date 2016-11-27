using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Ai : MonoBehaviour
{
    DecisionTree decisionTree;

    private CharacterBase characterBase;
    private AttackBase attackBase;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        attackBase = GetComponent<AttackBase>();
    }

    void Start()
    {
        decisionTree = DecisionTree.Decision(
            AreWeBusy,
            ifTrue: DecisionTree.Action(() => {}),
            ifFalse: DecisionTree.Decision(
                AreWeFalling,
                ifTrue: DecisionTree.Action(JumpTowardsFloor),
                ifFalse: DecisionTree.Decision(
                    PlayerInAttackRange,
                    ifTrue: DecisionTree.Decision(
                        AreAttacksOnCooldown,
                        ifTrue: DecisionTree.Action(StepAway),
                        ifFalse: DecisionTree.Action(Attack)
                    ),
                    ifFalse: DecisionTree.Decision(
                        PlayerInAttackRangeIfWeTurn,
                        ifTrue: DecisionTree.Action(Turn),
                        ifFalse: DecisionTree.Action(MoveTowardsPlayer)
                    )
                )
            )
        );
    }

    void FixedUpdate()
    {
        decisionTree.Search();
    }
    
    private bool AreWeBusy()
    {
        // TODO: Implement
        return true;
    }

    private bool AreWeFalling()
    {
        // TODO: Implement
        return false;
    }

    private void JumpTowardsFloor()
    {
        // TODO: Implement
    }

    private bool PlayerInAttackRange()
    {
        // TODO: Implement
        return false;
    }

    private bool AreAttacksOnCooldown()
    {
        // TODO: Implement
        return false;
    }

    private void StepAway()
    {
        // TODO: Implement
    }
    
    private void Attack()
    {
        // TODO: Implement
    }

    private bool PlayerInAttackRangeIfWeTurn()
    {
        // TODO: Implement
        return false;
    }

    private void Turn()
    {
        // TODO: Implement
    }

    private void MoveTowardsPlayer()
    {
        // TODO: Implement
    }
}