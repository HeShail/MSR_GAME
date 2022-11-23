using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary> Clase que desarrolla el comportamiento de los mapas del videojuego. </summary>
public class MapManager : MonoBehaviour
{
    [Header ("Mapas del Juego")]
    public GameObject outdoorsMap;
    public GameObject indoorsMap0;
    public GameObject indoorsMap1;
    public GameObject indoorsMap2;

    [Header("Minimapa")]
    public GameObject minimap;
    public Animator warning_minimap;
    [Header("Controlador")]
    public StarterAssets.ThirdPersonController character;

    [Header("Objetivos")]
    public NavMeshAgent objetiveTracker;
    public GameObject mapObjetive;

    void Start()
    {
        if (outdoorsMap != null) outdoorsMap.SetActive(false);
        if (indoorsMap0 != null) indoorsMap0.SetActive(false);
        if (indoorsMap1 != null) indoorsMap1.SetActive(false);
        if (indoorsMap2 != null) indoorsMap2.SetActive(false);
        if (minimap != null) minimap.SetActive(false);
        _mapEnable = false;
    }

    void Update()
    {
        //if (_mapEnable) Tracker();
        
    }

    /// <summary> Funcion publica que activa el mapa correspondiente de la zona si este no esta en uso. </summary>
    public void OpenMap(int map)
    {
        switch (map)
        {
            case 0:
                outdoorsMap.SetActive(true);
                break;
        }
    }

    /// <summary> Funcion publica que desactiva todos los mapas. </summary>
    public void CloseMap()
    {
        outdoorsMap.SetActive(false);
    }

    /// <summary> Procedimiento que desarrolla una ruta guia a través del minimapa hacia el objetivo de mision actual. </summary>
    public void Tracker()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            objetiveTracker.GetComponentInChildren<ParticleSystem>().Play();
            objetiveTracker.transform.position = character.transform.position;
            objetiveTracker.transform.position = character.transform.position;

            objetiveTracker.nextPosition = character.transform.position;
            objetiveTracker.nextPosition = character.transform.position;
            objetiveTracker.SetDestination(mapObjetive.transform.position);
        }

        if (objetiveTracker.isActiveAndEnabled && objetiveTracker.hasPath)
        {
            if (objetiveTracker.remainingDistance <= 1f)
            {
                objetiveTracker.GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                mapObjetive.GetComponentInChildren<ParticleSystem>().Play();
            }
        }
    }

    /// <summary> Metodo publico empleado para activar/desactivar el minimapa </summary>
    /// <param name="state"> Boolean que señala el estado final del minimapa </param>
    public void MinimapStatus(bool state)
    {
        minimap.SetActive(state);
    }

    [Tooltip ("¿Has conseguido el mapa?")]
    public bool _mapEnable;

    /// <summary> Funcion publica que activa los avisos del minimapa </summary>
    /// <param name="state"> Parametro entero que indica el tipo de aviso. [0] Aviso . [1] Capturado </param>
    public void MinimapWarning(int state)
    {
        if(_mapEnable && GetComponent<PauseMenuManager>().gameMachine.currentState.displayName == "Juego Activo")
        {
            switch (state)
            {
                case 0:
                    warning_minimap.SetTrigger("warning");
                    break;
                case 1:
                    warning_minimap.SetTrigger("caught");
                    break;

            }
        }
        
    }

    /// <summary> Metodo publico empleado para establecer como operativo tanto el mapa como el minimapa. </summary>
    /// <remarks> Otorgar el acceso antes imposible a estos recursos. </remarks>
    public void EnableMap()
    {
        _mapEnable = true;
    }

    /// <summary> ¿Ha conseguido el mapa? </summary>
    public bool GotMap()
    {
        return _mapEnable;
    }

}
