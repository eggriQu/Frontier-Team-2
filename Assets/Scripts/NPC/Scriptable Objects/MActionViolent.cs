using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Violence")]
public class MActionViolent : IAction
{
    public override void Entry(StateController fsm)
    {
        MonsterController monster = fsm.gameObject.GetComponent<MonsterController>();
        fsm.nodeOfInterest = null;

        if (monster != null && monster.comfort <= 3)
        {
            /*
            monster.enclosureNodes.Clear();
            monster.StartCoroutine(monster.GenerateEnclosureNodes());
            */
            fsm.StartCoroutine(WaitToGenerate(fsm));
        }
    }

    public override void Execute(StateController fsm)
    {
        
    }

    public override void Exit(StateController fsm)
    {
        
    }

    IEnumerator WaitToGenerate(StateController fsm)
    {
        yield return new WaitForSeconds(0.3f);

        fsm.availableNodes.Clear();
        fsm.availableNodes = AStarManager.instance.AllNodes().ToList();
    }
}
