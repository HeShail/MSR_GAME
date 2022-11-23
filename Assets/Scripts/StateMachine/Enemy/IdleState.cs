using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase que maneja el comportamiento patrulla de los enemigos ET. </summary>
public class IdleState : State
{
    private StateMachine_Enemy enemy;
    private DetectPlayer P_detection;

    [Tooltip("Destino actual de patrulla")]
    private int waypoint;

    [Tooltip("Variable que se�ala el tiempo de guardia en cada parada")]
    private float guardTime;

    [Tooltip ("Velocidad de movimiento en patrulla")]
    private float _speed=1f;

    public IdleState(StateMachine_Enemy _enemy)
    {
        displayName = "IDLE";
        enemy = _enemy;
        P_detection = enemy.gameObject.GetComponent<DetectPlayer>();

    }
    public override void Enter()
    {
        base.Enter();
        waypoint = 0;
        P_detection.ResetTarget();
        guardTime = UnityEngine.Random.Range(4f, 7f);
        enemy.GetAnim().SetBool("perseguir", false);
        enemy.Agent.stoppingDistance = 0f;
        enemy.Agent.speed = _speed;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!enemy.IsDead())
        {
            P_detection.FindTargetByProximity();
            P_detection.FindTargetByConeVision();
            if (P_detection.GetTarget() != null)
            {
                enemy.Agent.speed = 0f;
                enemy.sm.ChangeState(new BehaviourState(enemy));
            }
            else
            {
                if (enemy.HasIdle())
                {
                    enemy.GetComponentInChildren<Animator>().SetBool("idle", true);

                }
                else
                {
                    if (guardTime > 0f)
                    {
                        enemy.GetComponentInChildren<Animator>().SetBool("patrullar", false);
                        enemy.GetComponentInChildren<Animator>().SetBool("idle", true);
                        enemy.Agent.speed = 0f;

                    }
                    else
                    {
                        enemy.Agent.speed = _speed;
                        enemy.GetComponentInChildren<Animator>().SetBool("idle", false);
                        enemy.GetComponentInChildren<Animator>().SetBool("patrullar", true);
                        enemy.Agent.SetDestination(enemy.waypointsRoute[waypoint].transform.position);

                    }
                    CheckPointRoute();
                    guardTime -= Time.deltaTime;
                }
            }

        }
    }

    /// <summary> Funcion que detecta si el punto actual es de guardia. </summary>
    /// <remarks> En caso verdadero, asigna tiempo de guardia. </remarks>
    void CheckPointRoute()
    {

        if ((enemy.transform.position.x <= enemy.waypointsRoute[waypoint].transform.position.x + 0.5f)
         && (enemy.transform.position.x >= enemy.waypointsRoute[waypoint].transform.position.x - 0.5f))
        {
            if ((enemy.transform.position.z <= enemy.waypointsRoute[waypoint].transform.position.z + 0.5f)
                 && (enemy.transform.position.z >= enemy.waypointsRoute[waypoint].transform.position.z - 0.5f))
            {

                if (waypoint < enemy.waypointsRoute.Count-1) waypoint++; else waypoint = 0;
                int[] index = enemy.GetStopWaypoints();
                if (index!= null)
                {
                    for (int i = 0; i < index.Length; i++)
                    {
                        if (index[i] == waypoint) guardTime = UnityEngine.Random.Range(2f, 6f);
                    }
                }

                //switch (waypoint)
                //{
                //    case 0:
                //        guardTime = UnityEngine.Random.Range(2f, 6f);
                //        break;

                //    case 2:
                //        guardTime = UnityEngine.Random.Range(2f, 6f);
                //        break;

                //    case 4:
                //        guardTime = UnityEngine.Random.Range(2f, 6f);
                //        break;
                //}
            }
        }


    }
}
