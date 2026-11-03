using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Monster Violent")]
public class MDecisionViolent : IDecision
{
    public override bool Check(StateController fsm)
    {
        MonsterController monster = fsm.gameObject.GetComponent<MonsterController>();
        if (monster.comfort < 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
