using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase publica que declara una maquina de estados y que modifica su estado en tiempo real. </summary>
public class StateMachine : MonoBehaviour
{
    public State currentState;

    public void ChangeState(State _newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = _newState;
        currentState.Enter();
    }

    public void SMUpdate()
    {
        currentState?.Update();
    }
}
