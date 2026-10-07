using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Idle")]
public class DecisionIdle : IDecision
{
    public override bool Check(StateController fsm)
    {
        if (fsm.path.Count <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
