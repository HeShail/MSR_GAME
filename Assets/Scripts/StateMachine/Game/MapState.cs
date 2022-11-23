using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Estado de juego concreto en el que se activa el mapa. </summary>
public class MapState : State
{
    [Tooltip("Script que controla el flujo de la maquina de estados.")]
    private PauseMenuManager _manager;

    [Tooltip("Constante de punto flotante que indica la escala de tiempo actual.")]
    private const float SCALE = 0f;

    /// <summary> Constructor del estado de juego [Mapa Activo] </summary>
    public MapState(PauseMenuManager manager)
    {
        displayName = "Mapa Activo";
        _manager = manager;
    }
    public override void Enter()
    {
        base.Enter();
        Time.timeScale = SCALE;
        _manager.GetComponent<MapManager>().OpenMap(0);

    }

    public override void Update()
    {
        base.Update();
        Time.timeScale = SCALE;
        Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown(KeyCode.M))
        {
            _manager.gameMachine.ChangeState(new LiveState(_manager));
        }

    }
    public override void Exit()
    {
        base.Exit();
        _manager.GetComponent<MapManager>().CloseMap();
    }
}
