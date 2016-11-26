using UnityEngine;
using System.Collections;

public class DecisionTree : MonoBehaviour {
    /// <summary>
    /// Decision delegate, returns true or false.
    /// </summary>
    /// <returns></returns>
    public delegate bool Decision();
    /// <summary>
    /// Actio delegate. Performs an action.
    /// </summary>
    public delegate void Action();

    Decision theDecision;
    Action theAction;
    /// <summary>
    /// Reference to left sub tree
    /// </summary>
    public DecisionTree leftSubTree;
    /// <summary>
    /// Reference to right sub tree
    /// </summary>
    public DecisionTree rightSubTree;

	// Use this for initialization
	void Start () {
        leftSubTree = null;
        rightSubTree = null;
        theDecision = null;
        theAction = null;
	
	}

    /// <summary>
    /// Recursively searches the decision tree for a action node. A decision that returns true searches left.
    /// </summary>
    public void Search()
    {
        if(theDecision==null && theAction==null)
        {
            Debug.Log("no Decision or action");
        }
        if (theAction != null)
        {
            theAction();
            return;
        }
        if (theDecision!=null)
        {
            if (theDecision())
            {
                leftSubTree.Search();
            }
            else{
                rightSubTree.Search();
            }
        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Insert Decision into left child node
    /// A return value of true for a decision searches left, a false searches right
    /// </summary>
    /// <param name="dec">Decision method to be run in node</param>
    public void insertDecisionLeft(Decision dec)
    {
        leftSubTree = new DecisionTree();
        leftSubTree.theDecision = dec;
        
    }
    /// <summary>
    /// Insert Decision into right child node
    /// </summary>
    /// <param name="dec">Decision method to be run in node</param>
    public void insertDecisionRight(Decision dec)
    {
        rightSubTree = new DecisionTree();
        rightSubTree.theDecision = dec;
    }
    /// <summary>
    /// Insert action into left child node
    /// </summary>
    /// <param name="act">Action method to be run in node</param>
    public void insertActionLeft(Action act)
    {
        leftSubTree = new DecisionTree();
        leftSubTree.theAction = act;
    }
    /// <summary>
    /// Insert Action into right child node
    /// </summary>
    /// <param name="act">Action Method to be run in node</param>
    public void insertActionRight(Action act)
    {
        rightSubTree = new DecisionTree();
        rightSubTree.theAction = act;
    }
    /// <summary>
    /// Insert Action at root of tree. Should only be done if you only want a tree with one node.
    /// </summary>
    /// <param name="act"></param>
    public void insertActionRoot(Action act)
    {
        theAction = act;
    }
    /// <summary>
    /// Insert Decision at root of Tree
    /// </summary>
    /// <param name="act"></param>
    public void insertDecisionRoot(Decision act)
    {
        theDecision = act;
    }
}
