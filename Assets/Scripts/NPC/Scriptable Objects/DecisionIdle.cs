using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Idle")]
public class DecisionIdle : IDecision
{
    public override bool Check(StateController fsm)
    {
        if (fsm.FindInterestingNodes() == null && !fsm.currentNode.hasAttraction)
        {
            return true;
        }
        else
        {
            fsm.nodeOfInterest = fsm.interestingNodes[0];
            return false;
        }
    }
}
