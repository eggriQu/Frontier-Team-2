using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/FUCK")]
public class ActionFUCK : IAction
{
    public override void Entry(StateController fsm)
    {
        
    }

    public override void Execute(StateController fsm)
    {
        Debug.Log("SHit");
    }

    public override void Exit(StateController fsm)
    {
        
    }
}
