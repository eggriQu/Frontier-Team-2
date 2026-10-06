using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Test")]
public class ActionTest : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }

    public override void Execute(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }

    public override void Exit(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
    }
}
