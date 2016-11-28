using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Ai : MonoBehaviour
{
    /// <summary>
    /// The decision tree that controls AI behavior.
    /// </summary>
    private DecisionTree decisionTree;

    /// <summary>
    /// True when the AI is in the middle of some action (and thus must not try and perform another).
    /// </summary>
    private bool areWeBusy;

    private CharacterBase characterBase;
    private AttackBase attackBase;
    private Rigidbody2D rigidBody;

    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
        attackBase = GetComponent<AttackBase>();
        rigidBody = GetComponent<Rigidbody2D>();
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
        return areWeBusy;
    }

    private bool AreWeFalling()
    {
        // If there's a floor beneath us, we're definitely fine. Otherwise we look at our velocity to
        // see if we're falling. This ensures we aren't "falling" if we're falling onto a platform.
        var hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        foreach (var hit in hits)
        {
            if (hit.transform.tag == "Floor") return false;
        }
        return rigidBody.velocity.y < 0;
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