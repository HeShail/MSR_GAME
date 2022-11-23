using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Estado orientado a la destrucción de barricadas, estado de acceso exclusivo para enemigo luchador y corredor.
public class CleanState : State
{
    private StateMachine_Enemy enemy;
    private float time;
    private GameObject barricada;
    public CleanState(StateMachine_Enemy _enemy)
    {
        time = 2f;
        displayName = "Destroy Da Walls!";
        enemy = _enemy;
        //enemy.IsBlocked(true);
        //barricada = enemy.GetWallTarget();


    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        if ((!enemy.IsDead()) && barricada != null)
        {
            if (Vector3.Distance(barricada.transform.position, enemy.transform.position) <= 3f)
            {
                enemy.GetComponentInChildren<Animator>().SetBool("perseguir", false);
                enemy.GetComponentInChildren<Animator>().SetTrigger("ataque");
                time -= Time.deltaTime;
                if (time >= 0f) enemy.Agent.SetDestination(enemy.transform.position);
                if (time <= 0f)
                {
                    //Atacar obstáculo

                    //barricada.GetComponent<Trap>().DañarBarricada();
                    time = 3f;

                }
            }
            else
            {
                enemy.Agent.SetDestination(barricada.transform.position);
                time = 2f;
            }
        }
        else
        {
            enemy.sm.ChangeState(new MoveState(enemy));

        }

    }
}

