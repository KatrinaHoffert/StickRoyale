using UnityEngine;
using System.Collections;
using System;

public class DecisionTree
{
    /// <summary>
    /// Decision that this node contains, if any (either this or <see cref="action"/> must be set).
    /// </summary>
    Func<bool> decision;

    /// <summary>
    /// Action that this node contains, if any (either this or <see cref="action"/> must be set).
    /// </summary>
    Action action;

    /// <summary>
    /// Reference to true sub tree
    /// </summary>
    public DecisionTree trueSubTree;

    /// <summary>
    /// Reference to false sub tree
    /// </summary>
    public DecisionTree falseSubTree;

    /// <summary>
    /// Recursively searches the decision tree for a action node. A decision that returns true searches left.
    /// </summary>
    public void Search()
    {
        if (decision == null && action == null)
        {
            Debug.Log("Decision tree is invalid!");
        }
        if (action != null)
        {
            action();
            return;
        }
        if (decision != null)
        {
            if (decision()) trueSubTree.Search();
            else falseSubTree.Search();
        }
    }
    
    public static DecisionTree Decision(Func<bool> decision, DecisionTree ifTrue, DecisionTree ifFalse)
    {
        return new DecisionTree
        {
            decision = decision,
            trueSubTree = ifTrue,
            falseSubTree = ifFalse
        };
    }

    public static DecisionTree Action(Action action)
    {
        return new DecisionTree
        {
            action = action
        };
    }
}
