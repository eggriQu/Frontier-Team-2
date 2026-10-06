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
    [SerializeField] private State _pState;
    [SerializeField] private State _lastState;
    [SerializeField] private State _testState;
    [SerializeField] private State _startState;

    public State currentState => _pState;
    public State previousState => _lastState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeState(_startState);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputSystem.actions.FindAction("Jump").WasPressedThisFrame())
        {
            ChangeState(_testState);
        }
    }

    public void ObtainPath()
    {
        
    }

    public void PatrolArea()
    {

    }

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
