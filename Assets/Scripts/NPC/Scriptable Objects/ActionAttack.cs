using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Attack")]
public class ActionAttack : IAction
{
    public override void Entry(StateController fsm)
    {
        if (fsm.gameObject.GetComponent<MonsterController>() != null && fsm.interestingNodes.Count < 1)
        {
            fsm.FindNodesWithWalls();
            fsm.nodeOfInterest = fsm.interestingNodes[Random.Range(0, 1)];
            fsm.path = AStarManager.instance.GeneratePath(fsm.currentNode, fsm.nodeOfInterest);
        }
    }

    public override void Execute(StateController fsm)
    {
        fsm.StartCoroutine(AttackRoutine(fsm));
    }

    public override void Exit(StateController fsm)
    {
        
    }

    IEnumerator AttackRoutine(StateController fsm)
    {
        fsm.interactCollider.radius = 2.61f;
        yield return new WaitForSeconds(0.3f);
        fsm.interactCollider.radius = 0.3f;
    }
}
