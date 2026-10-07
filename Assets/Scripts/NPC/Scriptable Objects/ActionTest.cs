using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Test")]
public class ActionTest : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);

        if (fsm.path.Count == 0)
        {
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, AStarManager.instance.AllNodes()[Random.Range(0, AStarManager.instance.AllNodes().Length)]);
        }
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
