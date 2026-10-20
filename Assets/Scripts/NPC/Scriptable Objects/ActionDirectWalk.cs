using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Direct Walk")]
public class ActionDirectWalk : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }

    public override void Execute(StateController fsm)
    {
        fsm.StartCoroutine(WaitToGeneratePath(fsm));
    }

    public override void Exit(StateController fsm)
    {
        
    }

    IEnumerator WaitToGeneratePath(StateController fsm)
    {
        yield return new WaitForSeconds(0.3f);

        if (fsm.nodeOfInterest != null)
        {
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, fsm.nodeOfInterest);
        }
        else
        {
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, fsm.availableNodes[Random.Range(0, fsm.availableNodes.Count - 1)]);
        }
    }
}
