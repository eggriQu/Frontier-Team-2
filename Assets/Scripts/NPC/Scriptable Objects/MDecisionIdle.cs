using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Monster Idle")]
public class MDecisionIdle : IDecision
{
    public override bool Check(StateController fsm)
    {
        MonsterController monster = fsm.gameObject.GetComponent<MonsterController>();
        monster.comfort = monster.comfort - 2;
        if (monster.comfort <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
