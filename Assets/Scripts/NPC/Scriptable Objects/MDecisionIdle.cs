using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decision/Monster Idle")]
public class MDecisionIdle : IDecision
{
    public override bool Check(StateController fsm)
    {
        MonsterController monster = fsm.gameObject.GetComponent<MonsterController>();
        if (monster.comfort <= 3)
        {
            return true;
        }
        else
        {
            float lowestFScore = default;
            Node lowestFScoreNode = null;
            foreach (Node node in fsm.interestingNodes)
            {
                if (node != fsm.interestingNodes[0])
                {
                    if (Vector3.Distance(fsm.transform.position, node.transform.position) < lowestFScore)
                    {
                        lowestFScore = Vector3.Distance(fsm.transform.position, node.transform.position);
                        lowestFScoreNode = node;
                    }
                }
                else
                {
                    lowestFScore = Vector3.Distance(fsm.transform.position, node.transform.position);
                    lowestFScoreNode = node;
                }
            }

            fsm.nodeOfInterest = lowestFScoreNode;
            fsm.interestingNodes.Remove(fsm.nodeOfInterest);
            return false;
        }
    }
}
