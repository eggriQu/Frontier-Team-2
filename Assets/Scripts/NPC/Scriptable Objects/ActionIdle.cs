using UnityEngine;

public abstract class IAction : ScriptableObject
{
    public abstract void Entry(StateController fsm);
    public abstract void Execute(StateController fsm);
    public abstract void Exit(StateController fsm);
}

[CreateAssetMenu(menuName = "AI/Action/Idle")]
public class ActionIdle : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
        fsm.currentState.Execute(fsm);
    }

    public override void Execute(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
    }

    public override void Exit(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }
}
