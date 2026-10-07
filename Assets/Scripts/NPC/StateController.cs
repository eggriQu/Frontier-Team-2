using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static UnityEditor.PlayerSettings;

public class StateController : MonoBehaviour
{
    public Animator anim;
    [SerializeField] protected State _pState;
    [SerializeField] protected State _lastState;
    [SerializeField] protected State _startState;

    public Node currentNode;
    public Node nodeOfInterest;
    public List<Node> path = new List<Node>();

    public State currentState => _pState;
    public State previousState => _lastState;  

    public bool ChangeState(State newState)
    {
        if (newState == null)
        {
            Debug.LogWarning($"{nameof(StateController)} on '{name}' ignored a null state change.", this);
            return false;
        }

        if (_pState == newState)
        {
            return false;
        }

        currentState?.Exit(this);
        _lastState = _pState;
        _pState = newState;
        currentState.Entry(this);

        return true;
    }
}
