using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Estado de juego durante el cual la escala de tiempo es fija a 1 </summary>
public class LiveState : State
{
    [Tooltip ("Script que controla el flujo de la maquina de estados.")]
    private PauseMenuManager _manager;

    [Tooltip("Constante de punto flotante que indica la escala de tiempo actual.")]
    private const float SCALE= 1f;

    /// <summary> Constructor del estado de juego [Juego Activo] </summary>
    public LiveState(PauseMenuManager manager)
    {
        displayName = "Juego Activo";
        _manager = manager;
    }

    public override void Enter()
    {
        if (_manager.GetComponent<MapManager>().GotMap()) _manager.GetComponent<MapManager>().minimap.SetActive(true);
        Time.timeScale = SCALE;
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.M) && _manager.GetComponent<MapManager>().GotMap() && !_manager.IsVideoPlaying())
        {
            _manager.gameMachine.ChangeState(new MapState(_manager));
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !_manager.IsVideoPlaying())
        {
            _manager.gameMachine.ChangeState(new PauseState(_manager));
        }
    }

    public override void Exit()
    {
        _manager.GetComponent<MapManager>().minimap.SetActive(false);
        base.Exit();

    }

}
