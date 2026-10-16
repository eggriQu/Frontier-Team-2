using System.Collections;
using System.IO;
using UnityEngine;

public abstract class IAction : ScriptableObject
{
    public abstract void Entry(StateController fsm);
    public abstract void Execute(StateController fsm);
    public abstract void Exit(StateController fsm);
}

[CreateAssetMenu(menuName = "AI/Action/Idle")]
public class ActionIdle : IAction
{
    public override void Entry(StateController fsm)
    {
        fsm.anim.SetBool("Idle", true);
        fsm.StartCoroutine(WaitToExecute(fsm));
    }

    public override void Execute(StateController fsm)
    {
        
    }

    public override void Exit(StateController fsm)
    {
        
    }

    public IEnumerator WaitToExecute(StateController fsm)
    {
        yield return new WaitForSeconds(2.5f);
        fsm.currentState.Execute(fsm);
    }
}
