using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Walking")]
public class DecisionWalking : IDecision
{
    public override bool Check(StateController fsm)
    {
        if (fsm.nodeOfInterest != null || fsm.FindInterestingNodes() != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
