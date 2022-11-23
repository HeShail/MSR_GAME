using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase encargada de ejecutar la retirada del enemigo hacia su posición inicial. </summary>
public class MoveState : State
{
    private StateMachine_Enemy enemy;
    private DetectPlayer P_detection;
    public MoveState(StateMachine_Enemy _enemy)
    {
        displayName = "RETIRING";
        enemy = _enemy;
        P_detection = enemy.gameObject.GetComponent<DetectPlayer>();
    }

    public override void Enter()
    {
        base.Enter();
        enemy.GetAnim().SetBool("idle", false);
        enemy.Agent.SetDestination(enemy.waypointsRoute[0].transform.position);
        enemy.Agent.speed = 1f;
        enemy.Agent.stoppingDistance = 0;
    }

    public override void Update()
    {
       base.Update();
       //enemy.GetAnim().SetBool("perseguir", true);


        if ((enemy.transform.position.x <= enemy.waypointsRoute[0].transform.position.x + 1f)
         && (enemy.transform.position.x >= enemy.waypointsRoute[0].transform.position.x - 1f))
        {
            if ((enemy.transform.position.z <= enemy.waypointsRoute[0].transform.position.z + 1f)
                 && (enemy.transform.position.z >= enemy.waypointsRoute[0].transform.position.z - 1f))
            {
                enemy.sm.ChangeState(new IdleState(enemy));
            }
        }

    }

}
