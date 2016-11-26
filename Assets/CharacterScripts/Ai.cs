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
            PlayerInAttackRange,
            DecisionTree.Action(AttackPlayer),
            DecisionTree.Action(DoNothing)
        );
    }

    void FixedUpdate()
    {
        decisionTree.Search();
    }

    private bool PlayerInAttackRange()
    {
        return true;
    }

    private void AttackPlayer()
    {
        attackBase.Attack1();
    }

    private void DoNothing()
    {

    }
}