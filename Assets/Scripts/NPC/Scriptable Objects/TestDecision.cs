using UnityEngine;

public abstract class IDecision : ScriptableObject
{
    public abstract bool Check(StateController fsm);
}

[CreateAssetMenu(menuName = "AI/Decision/Test")]
public class TestDecision : IDecision
{
    public override bool Check(StateController fsm)
    {
        return !fsm.anim.GetBool("Idle");
    }
}
