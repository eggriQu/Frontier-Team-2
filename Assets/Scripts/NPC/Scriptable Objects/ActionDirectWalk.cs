using System.Collections;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Direct Walk")]
public class ActionDirectWalk : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
        fsm.StartCoroutine(WaitToGeneratePath(fsm));
    }

    public override void Execute(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }

    public override void Exit(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
    }

    IEnumerator WaitToGeneratePath(StateController fsm)
    {
        yield return new WaitForSeconds(0.1f);

        if (fsm.nodeOfInterest != null)
        {
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, fsm.nodeOfInterest);
        }
        else
        {
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, AStarManager.instance.AllNodes()[Random.Range(0, AStarManager.instance.AllNodes().Length)]);
        }
    }
}
