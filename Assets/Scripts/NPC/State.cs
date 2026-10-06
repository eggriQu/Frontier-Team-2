using UnityEngine;

[CreateAssetMenu(menuName = "AI/State")]
public class State : ScriptableObject
{
    public IAction[] actions;
    public Transition[] transitions;
    public void Entry(StateController fsm)
    {
        for (int i = 0; i < actions.Length; i++)
        { actions[i].Entry(fsm); }
    }

    public void Execute(StateController fsm)
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].decision.Check(fsm))
            {
                fsm.ChangeState(transitions[i].TrueState);
            }
            else
            {
                fsm.ChangeState(transitions[i].FalseState);
            }
        }
        for (int i = 0; i < actions.Length; i++)
        { actions[i].Execute(fsm); }
    }

    public void Exit(StateController fsm)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Exit(fsm);
        }
    }
}
