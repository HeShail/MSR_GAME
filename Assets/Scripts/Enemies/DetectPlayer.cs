using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary> Clase publica que gestiona el detector de jugador para los enemigos. </summary>
public class DetectPlayer : MonoBehaviour
{

    [SerializeField] private GameObject playerTarget;
    [SerializeField] private NavMeshAgent agent;

    [Header("Detectar jugador")]
    public LayerMask targetLayer;
    public float rango_avisoDet = 0f;
    public float rango_avisoVision = 0f;
    public float rangoMinimoDet = 0f;
    public float rangoDetVision = 0f;


    [Header("Cono de vision")]
    public float visionAngle = 90f;
    public float warningVisionAngle;
    public Transform angleHelperR, angleHelperL;

    [Header("Buscar cobertura")]
    public LayerMask obstacleLayer;

    /// <summary> Funcion publica que detecta al personaje jugador por proximidad al enemigo y activa los avisos de advertencia del minimapa. </summary>
    public void FindTargetByProximity()
    {

        //Distancia de detección, primero avisa al jugador de enemigo circundante. Si se acerca demasiado, lo atrapa.
        Collider[] _target = Physics.OverlapSphere(transform.position, rango_avisoDet, targetLayer);
        if (_target.Length > 0)
        {
            if (!_target[0].GetComponent<StarterAssets.ThirdPersonController>().GetTargetableStatus() || PlayerRespawn.respawnScript.isReappearing) _target = null;
            else
            {
                GameManager.gm.GetComponent<MapManager>().MinimapWarning(0);
                Collider[] _target2 = Physics.OverlapSphere(transform.position, rangoMinimoDet, targetLayer);
                if (_target2.Length > 0)
                {
                    playerTarget = _target2[0].gameObject;
                    GameManager.gm.GetComponent<MapManager>().MinimapWarning(1);

                }
                else playerTarget = null;
            }
        }
    }

    /// <summary> Funcion publica que detecta al personaje jugador por visibilidad enemiga y activa los avisos de advertencia del minimapa. </summary>
    public void FindTargetByConeVision()
    {
        //DETECTANDO JUGADOR A RANGO DE AVISO
        Collider[] _target = Physics.OverlapSphere(transform.position, rango_avisoVision, targetLayer);
        if (_target.Length > 0)
        {
            if (!_target[0].GetComponent<StarterAssets.ThirdPersonController>().GetTargetableStatus() || PlayerRespawn.respawnScript.isReappearing) _target = null;
            else
            {
                Vector3 _dir = _target[0].transform.position - transform.position;
                if (Vector3.Angle(transform.forward, _dir) < warningVisionAngle / 2f)
                {
                    //A continuanci�n comprobamos si hay alg�n obstaculo por en medio
                    if (Physics.Raycast(transform.position + Vector3.up, _dir.normalized, _dir.magnitude, obstacleLayer))
                    {

                    }
                    else
                    {
                        GameManager.gm.GetComponent<MapManager>().MinimapWarning(0);
                    }
                }
            }
            
        }

        //DETECTANDO JUGADOR A RANGO DE DETECCI�N
        _target = Physics.OverlapSphere(transform.position, rangoDetVision, targetLayer);
        if (_target.Length > 0)
        {
            if (!_target[0].GetComponent<StarterAssets.ThirdPersonController>().GetTargetableStatus() || PlayerRespawn.respawnScript.isReappearing) _target = null;
            else
            {
                Vector3 _dir = _target[0].transform.position - transform.position;
                if (Vector3.Angle(transform.forward, _dir) < visionAngle / 2f)
                {
                    //A continuanci�n comprobamos si hay alg�n obstaculo por en medio
                    if (Physics.Raycast(transform.position + Vector3.up, _dir.normalized, _dir.magnitude, obstacleLayer))
                    {
                        Debug.Log("hay un muro");
                    }
                    else
                    {
                        GameManager.gm.GetComponent<MapManager>().MinimapWarning(1);
                        if (_target.Length > 0)
                        {
                            playerTarget = _target[0].gameObject;
                            GameManager.gm.GetComponent<MapManager>().MinimapWarning(1);

                        }
                        else playerTarget = null;
                    }
                }
            }

        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoMinimoDet);
        if (angleHelperR != null)
        {
            angleHelperR.localEulerAngles = new Vector3(0f, visionAngle / 2f, 0f);
            Gizmos.DrawRay(angleHelperR.position, angleHelperR.forward * rangoDetVision);
        }
        if (angleHelperL != null)
        {
            angleHelperL.localEulerAngles = new Vector3(0f, -visionAngle / 2f, 0f);
            Gizmos.DrawRay(angleHelperL.position, angleHelperL.forward * rangoDetVision);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango_avisoDet);
        if (angleHelperR != null)
        {
            angleHelperR.localEulerAngles = new Vector3(0f, warningVisionAngle / 2f, 0f);
            Gizmos.DrawRay(angleHelperR.position, angleHelperR.forward * rango_avisoVision);
        }
        if (angleHelperL != null)
        {
            angleHelperL.localEulerAngles = new Vector3(0f, -warningVisionAngle / 2f, 0f);
            Gizmos.DrawRay(angleHelperL.position, angleHelperL.forward * rango_avisoVision);
        }
        if (playerTarget != null) Gizmos.DrawLine(transform.position, playerTarget.transform.position);

    }

    /// <summary> Funcion publica que borra la asignacion de la variable objetivo. </summary>
    public void ResetTarget()
    {
        playerTarget = null;
    }

    /// <summary> ¿El enemigo ha detectado al jugador? </summary>
    public bool HasTarget()
    {
        if (playerTarget != null) return true; else return false;
    }

    /// <summary> Devuelve el objetivo detectado. Si no ha detectado al jugador, retornará NULL. </summary>
    public GameObject GetTarget()
    {
        return playerTarget;
    }

    
}
