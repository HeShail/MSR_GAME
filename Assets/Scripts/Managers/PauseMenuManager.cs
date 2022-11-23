using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase que altera el estado del juego, lo pausa y lo gestiona. </summary>
public class PauseMenuManager : MonoBehaviour
{
    public StateMachine gameMachine;

    [Header("Paneles de Pausa")]
    public GameObject pauseMenu;
    public GameObject[] subpanel_list;
    public GameObject mainPanel;

    [Header("Otros elementos")]
    public GameObject CinematicPlayer;
    private bool inControlPanel=false;

    void Start()
    {
        gameMachine = new StateMachine();
        gameMachine.ChangeState(new LiveState(this));
    }

    void Update()
    {
        Debug.Log("Current state: " + gameMachine.currentState.displayName);
        gameMachine.SMUpdate();
    }

    /// <summary> Funcion publica que invoca nuevos estados a la máquina. </summary>
    /// <param name="state"> Identificador entero que refiere al estado destino </param>
    public void ChangeGameState(int state)
    {
        if (!IsVideoPlaying())
        {
            switch (state)
            {
                case 0:
                    gameMachine.ChangeState(new LiveState(this));
                    break;
                case 1:
                    gameMachine.ChangeState(new PauseState(this));
                    break;
                case 2:
                    if (GetComponent<MapManager>().GotMap()) gameMachine.ChangeState(new MapState(this)); 
                    else AudioManager.audioManager.InvalidSelection();
                    break;
            }
        }
        
    }

    /// <summary> Metodo que retorna un booleano indicando si no se encuentra un video en reproduccion. </summary>
    public bool IsVideoPlaying()
    {
        return CinematicPlayer.GetComponent<CinematicPlayer>().playing;
    }

    /// <summary> Funcion publica que asigna un booleano a la variable inControlPanel. </summary>
    /// <param name="state"> Booleano que señala el estado del panel. </param>
    public void SetInControlPanel(bool state)
    {
        inControlPanel = state;
    }

    /// <summary> Metodo que retorna un booleano indicando si se encuentra o no en el panel de control. </summary>
    public bool IsInControlPanel()
    {
        return inControlPanel;
    }
}
