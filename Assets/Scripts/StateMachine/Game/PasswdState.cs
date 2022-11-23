using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Estado de juego durante el cual se ejecuta el tratamiento de secuencia del panel de acceso. </summary>
public class PasswdState : State
{
    [Tooltip("Script que controla el flujo de la maquina de estados.")]
    private PauseMenuManager _manager;

    [Tooltip("Constante de punto flotante que indica la escala de tiempo actual.")]
    private const float SCALE = 0f;

    /// <summary> Constructor del estado de juego [Panel de pausa activo] </summary>
    public PasswdState(PauseMenuManager manager)
    {
        displayName = "Panel de pausa activo";
        _manager = manager;
    }
    public override void Enter()
    {
        Time.timeScale = SCALE;
        _manager.GetComponent<PasswordPanelBehaviour>().SetPanel(true);
        base.Enter();

    }

    public override void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _manager.gameMachine.ChangeState(new LiveState(_manager));
        }

    }

    public override void Exit()
    {
        _manager.GetComponent<PasswordPanelBehaviour>().SetPanel(false);
        base.Exit();
    }

}
