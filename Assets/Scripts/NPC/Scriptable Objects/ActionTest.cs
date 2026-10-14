using System.Collections;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action/Test")]
public class ActionTest : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
        fsm.nodeOfInterest = null;
        fsm.StartCoroutine(WaitToExecute(fsm));
    }

    public override void Execute(StateController fsm)
    {
        fsm.anim.SetBool("Idle", false);
    }

    public override void Exit(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
    }

    public IEnumerator WaitToExecute(StateController fsm)
    {
        yield return new WaitForSeconds(2.5f);
        fsm.currentState.Execute(fsm);
    }
}
