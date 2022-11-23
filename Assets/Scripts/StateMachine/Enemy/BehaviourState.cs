using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;


/// <summary> Clase que opera como un estado para gestionar la actitud ofensiva de la IA extraterrestre. </summary>
public class BehaviourState : State
{
    private StateMachine_Enemy enemy;
    private DetectPlayer P_detection;
    private GameObject targetPlayer;
    private bool detected;
    private float time;
    private float reactTime;
    private float roarTime;
    private bool isRetiring;


    /// <summary> Constructor de la clase a la que le inicializan los componentes de detección y el personaje detectado. </summary>
    public BehaviourState(StateMachine_Enemy _enemy)
    {
        displayName = "Agressor";
        enemy = _enemy;
        if (enemy.deteccionProximidad == true)
        {
            P_detection = enemy.gameObject.GetComponent<DetectPlayer>();
        }
        targetPlayer = P_detection.GetTarget();

    }

    /// <summary> Función sobrecargada de Enter() que continua modificando los parámetros 
    /// del enemigo para este nuevo estado. </summary>/// 
    /// <remarks> Entre otras, asigna los tiempos de reacción a los estímulos, la frecuencia de
    /// emisión de sonidos, distancia de detención del navegador, etc.</remarks>
    public override void Enter()
    {
        base.Enter();
        if (P_detection.GetTarget() != null) enemy.RotateToPlayer();
        if (!enemy.HasProjectiles()) enemy.Agent.stoppingDistance = 1f;
        else enemy.Agent.stoppingDistance = 20f;
        time = UnityEngine.Random.Range(1.5f, 1f);
        reactTime = 0f;
        roarTime = 12f;
        isRetiring = false;
        detected = false;
    }

    /// <summary> Función update del estado ofensivo del enemigo en la que atrapa al jugador y 
    /// provoca su abducción si se cumples las condiciones necesarias. </summary>/// 
    /// <remarks> El enemigo actual atrapara al jugador con el contacto visual mientras
    /// la función GetStealthStatus sea positiva. Quiere decir, si el resultado es negativo significa
    /// que el enemigo combatirá con el jugadore y la mecánica de sigilo quedará desactivada.</remarks>
    public override void Update()
    {
        base.Update();
        if (!enemy.IsDead())
        {
            //if (targetPlayer.GetComponent<StarterAssets.ThirdPersonController>().GetHealth() <= 0 ||
            //    Vector3.Distance(enemy.transform.position, enemy.waypointsRoute[0].transform.position) >= 25f && !isRetiring) FinalRoar();

            if (P_detection.GetTarget() != null && !isRetiring && roarTime >= 0f)
            {
                //Ataque
                if (enemy.GetStealthStatus() && !detected)
                {
                    enemy.StartCoroutine("Shout_CRT");
                    P_detection.GetTarget().GetComponent<StarterAssets.ThirdPersonController>().MoveBind();
                    detected = true;

                }
                roarTime -= Time.deltaTime;
            }
            else
            {
                P_detection.ResetTarget();
                enemy.sm.ChangeState(new MoveState(enemy));
            }
        }
        if (isRetiring && roarTime <= 0f)
        {
            P_detection.ResetTarget();
            enemy.sm.ChangeState(new MoveState(enemy));

        }

    }


    #region CombatScript
    //private void NormalEnemyBehaviour()
    //{
    //    //Tratamiento si jugador dentro de rango
    //    if (Vector3.Distance(targetPlayer.transform.position, enemy.transform.position) < 2.5f)
    //    {

    //        enemy.GetComponentInChildren<Animator>().SetBool("perseguir", false);
    //        enemy.GetComponentInChildren<Animator>().SetBool("idle", true);
    //        reactTime = UnityEngine.Random.Range(0.5f, 0.75f);
    //        if (time >= 0f) time -= Time.deltaTime;
    //        else
    //        {
    //            enemy.HurtPlayer();
    //            time = UnityEngine.Random.Range(1.5f, 2f);
    //        }


    //    }
    //    //Comportamiento si el jugador sale del area de influencia
    //    if (reactTime <= 0f)
    //    {
    //        //Intenta alcanzar al objetivo
    //        enemy.Agent.speed = 4f;
    //        enemy.Agent.SetDestination(targetPlayer.transform.position);
    //        if (P_detection.GetTarget() != null) enemy.RotateToPlayer();
    //        enemy.GetComponentInChildren<Animator>().SetBool("perseguir", true);
    //        enemy.GetComponentInChildren<Animator>().SetBool("idle", false);
    //        time = UnityEngine.Random.Range(0.5f, 1.5f);

    //        //Si no puede alcanzar al objetivo vuelve a su puesto
    //        if (enemy.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial || enemy.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) FinalRoar();


    //    }
    //}
    //private void RangedEnemyBehaviour()
    //{
    //    if (Vector3.Distance(targetPlayer.transform.position, enemy.transform.position) <= enemy.Agent.stoppingDistance)
    //    {

    //        enemy.GetComponentInChildren<Animator>().SetBool("perseguir", false);
    //        enemy.GetComponentInChildren<Animator>().SetBool("idle", false);
    //        reactTime = UnityEngine.Random.Range(1f, 2f);
    //        if (time >= 0f) time -= Time.deltaTime;
    //        else
    //        {
    //            //Ejecutar disparo y asignar rotaci�n en corutina
    //            //if (enemy.GetRock()!= null)
    //            //{
    //            //    enemy.ThrowRock();
    //            //    enemy.RotateToPlayer();
    //            //    time = UnityEngine.Random.Range(1.5f, 2.5f);
    //            //}
    //        }


    //    }
    //    else
    //    {
    //        //Comportamiento si el jugador sale del area de influencia y no es centinela
    //        if (reactTime <= 0f && !enemy.IsSentinel())
    //        {
    //            enemy.Agent.speed = 4f;
    //            enemy.Agent.SetDestination(targetPlayer.transform.position);
    //            if (enemy.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial || enemy.Agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) FinalRoar();
    //            if (P_detection.GetTarget() != null) enemy.RotateToPlayer();
    //            enemy.GetComponentInChildren<Animator>().SetBool("perseguir", true);
    //            enemy.GetComponentInChildren<Animator>().SetBool("idle", false);
    //            time = UnityEngine.Random.Range(0.5f, 1.5f);

    //        }

    //        //Comportamiento si el jugador sale del area de influencia y es centinela
    //        if (reactTime <= 0f && enemy.IsSentinel() && !isRetiring) FinalRoar();
    //    }

    //}
    #endregion
}
