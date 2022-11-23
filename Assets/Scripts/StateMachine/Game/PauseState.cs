using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Estado de juego durante el cual la escala de tiempo es fija a 1 </summary>
public class PauseState : State
{

    [Tooltip("Script que controla el flujo de la maquina de estados.")]
    private PauseMenuManager _manager;

    [Tooltip("Constante de punto flotante que indica la escala de tiempo actual.")]
    private const float SCALE = 0f;

    /// <summary> Constructor del estado de juego [Pausa Activa] </summary>
    public PauseState(PauseMenuManager manager)
    {
        _manager = manager;
        displayName = "Pausa Activa";
    }
    public override void Enter()
    {
        Time.timeScale = SCALE;
        base.Enter();
        Setup();
    }

    public override void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        base.Update();

        if (Input.GetKeyDown(KeyCode.M) && _manager.GetComponent<MapManager>().GotMap()) _manager.gameMachine.ChangeState(new MapState(_manager));

        if (Input.GetKeyDown(KeyCode.Escape) && !_manager.IsInControlPanel()) _manager.gameMachine.ChangeState(new LiveState(_manager));
    }

    public override void Exit()
    {
        base.Exit();
        for (int i = 0; i < _manager.subpanel_list.Length; i++)
        {
            if (_manager.subpanel_list[i].name == "Panel_Controles" && !_manager.subpanel_list[i].activeInHierarchy)
            {
                _manager.GetComponentInChildren<AudioManager>().SaveSoundSettings();
            }
        }
        _manager.pauseMenu.SetActive(false);
    }

    /// <summary> Metodo privado que prepara la configuración del estado. </summary>
    /// <remarks> En este caso, desactiva las ventanas abiertas de las opciones y activa las predeterminadas.</remarks>
    private void Setup()
    {
        if (_manager.GetComponent<MapManager>().GotMap()) _manager.GetComponent<MapManager>().minimap.SetActive(false);
        for (int i = 0; i < _manager.subpanel_list.Length; i++)
        {
            _manager.subpanel_list[i].SetActive(false);
        }
        _manager.mainPanel.SetActive(true);
        _manager.pauseMenu.SetActive(true);

    }
}
