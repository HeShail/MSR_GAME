using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary> Clase publica dedicada al control del comportamiento enemigo (ET). </summary>
public class StateMachine_Enemy : MonoBehaviour 
{ 
    [Header("Navegación")]
    public StateMachine sm;
    public NavMeshAgent Agent { get; private set; }

    [Tooltip("Lista de los puntos de marcha del enemigo patrulla.")]
    public List<Transform> waypointsRoute = new List<Transform>();

    [Tooltip("Lista de los puntos de parada y guardia de los enemigos.")]
    public int[] stopWaypoints;

    [Tooltip("Variable que almacena la posición y rotación final de la camara enemiga")]
    public Transform CameraPos;

    [Header("Parametros de Enemigo")]
    public int healthPoints=3;
    public float WaitTime = 3f;
    public float timer = 0f;

    [Tooltip("¿Es enemigo estatico (no patrulla)?")]
    [SerializeField] private bool _hasIdle=true;

    [Tooltip("¿Es incapaz de perseguir?")]
    [SerializeField] private bool _isSentinel = false;

    [Tooltip("¿Dispara?")]
    [SerializeField] private bool _hasProjectiles = false;
    [SerializeField] private Animator anim;

    [Header("Detección")]
    public LayerMask playerLayer;
    public bool detect= false;
    public bool deteccionProximidad;
    [SerializeField] private bool stealthQuestEnemy;
    private bool isDead;

    [Header("Sincronización")]
    private float cont;
    private float actualDissolveStatus;
    private bool _isRotating=false;

    [Header("SFX")]
    public AudioClip stepsSFX;
    public AudioClip stepsVariantSFX;
    public AudioClip roarSFX;
    public AudioClip walkRoarSFX;
    public AudioClip bigRoarSFX;


    private void Awake()
    {
        cont = 0f;
        Agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        sm = new StateMachine();
        sm.ChangeState(new IdleState(this)); 

        actualDissolveStatus = 0f;
        isDead = false;
        if (HasIdle())
        {
            waypointsRoute.Add(transform);
            waypointsRoute[0].transform.position = transform.position;

        }
    }

    void Update()
    {
        if ((healthPoints <= 0)&& !isDead)
        {
            isDead = true;
            //EnemigoCaido();
            Destroy(gameObject, 5f);
        }
        else sm.SMUpdate();


        if (isDead)
        {
            cont -= Time.deltaTime;
            //Disolver(cuerpo.GetComponent<SkinnedMeshRenderer>());
        }
        else
        {
            RotatingTowardsPlayer();
        }
    }

    public void DamageEnemy()
    {
        healthPoints --;
    }



    //void EnemigoCaido()
    //{
    //    //gameObject.GetComponent<Ragdoll>().EnableRB(true);
    //    gameObject.GetComponent<NavMeshAgent>().enabled = false;
    //    SkinnedMeshRenderer meshRenderer = cuerpo.GetComponent<SkinnedMeshRenderer>();
    //    //meshRenderer.material = dissolveMat;
    //    actualDissolveStatus = 0f;
    //    isDead = true;
    //}
    /// <summary> [En desuso] Funcion publica de disolver cuerpo de enemigo. </summary>
    void Disolver(SkinnedMeshRenderer meshRenderer)
    {
        if (cont <= 0f)
        {
            actualDissolveStatus += 0.01f;
            //actualDissolveStatus = Mathf.Lerp(actualDissolveStatus, finalDissolveStatus, 2*Time.deltaTime);
            meshRenderer.material.SetFloat("estado", actualDissolveStatus);
            cont = 0.01f;
        }
    }
    /// <summary> ¿El enemigo ha caido? </summary>
    public bool IsDead()
    {
        return isDead;
    }

    /// <summary> ¿El enemigo es estatico? </summary>
    public bool HasIdle()
    {
        return _hasIdle;
    }

    /// <summary> ¿El enemigo es a rango? </summary>
    public bool HasProjectiles()
    {
        return _hasProjectiles;
    }

    /// <summary> Funcion publica que retorna los puntos de vida del enemigo </summary>
    public int GetHealthPoints()
    {
        return healthPoints;
    }

    /// <summary> Funcion publica que ejecuta ataque contra el jugador. </summary>
    public void HurtPlayer()
    {
        StartCoroutine("PlayerPoke");
        StartCoroutine("RotateTP");      
    }

    /// <summary> Corutina que hace daño al jugador. </summary>
    IEnumerator PlayerPoke()
    {
        GetComponentInChildren<Animator>().SetTrigger("ataque");

        yield return new WaitForSeconds(0.9f);
       
        if (GetComponent<DetectPlayer>().GetTarget() != null && _isRotating)
        {
            Component[] comp= GetComponentsInChildren<Transform>();
            foreach (Transform position in comp)
            {
                if (position.transform.CompareTag("trigger"))
                {
                    Collider[] _target = Physics.OverlapSphere(position.transform.position, 1.3f, playerLayer);
                    if (_target.Length > 0)
                    {
                        //if (!_target[0].GetComponent<StarterAssets.ThirdPersonController>().IsDead())
                        //{
                        //    GetComponent<DetectPlayer>().GetTarget().GetComponent<StarterAssets.ThirdPersonController>().HurtPlayer();
                        //}
                    }
                }
            }
            
        }
    }

    /// <summary> Funcion publica que invoca rotacion del enemigo hacia player. </summary>
    public void RotateToPlayer()
    {
        StartCoroutine("RotateTP");
    }

    /// <summary>Corutina encargada de activar la rotacion del personaje enemigo en dirección al jugador al momento de atacar.</summary>
    IEnumerator RotateTP()
    {
        StartCoroutine("ResetSentinelRotation");
        if (HasProjectiles() && GetComponent<DetectPlayer>().GetTarget() != null && Vector3.Distance(transform.position, GetComponent<DetectPlayer>().transform.position) < 50f) _isRotating = true;
        if (!HasProjectiles() && GetComponent<DetectPlayer>().GetTarget() != null && Vector3.Distance(transform.position, GetComponent<DetectPlayer>().transform.position) < 10f) _isRotating = true;
        yield return new WaitForSeconds(1.0f);
        _isRotating = false;

    }

    /// <summary> Funcion publica que rota el enemigo hacia player de acuerdo al tiempo que se mantiene activado el booleano _isRotating. </summary>
    public void RotatingTowardsPlayer()
    {
        if (GetComponent<DetectPlayer>().GetTarget() != null && _isRotating)
        {
            Vector3 worldAimTarget = GetComponent<DetectPlayer>().GetTarget().transform.position;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 10f);
        }
    }

    /// <summary> Corutina privada que devuelve al enemigo a su rotacion inicial. </summary>
    /// <remarks> Necesario unicamente para enemigos estaticos que no persiguen. </remarks>
    private IEnumerator ResetSentinelRotation()
    {
        Quaternion rot = transform.rotation;
        yield return new WaitForSeconds(9f);
        if (_hasIdle) transform.rotation = rot;
    }

    /// <summary> Funcion publica que invoca disparo al enemigo. </summary>
    public void ThrowRock()
    {
        StartCoroutine("LaunchProjectile");
    }

    //IEnumerator LaunchProjectile()
    //{
    //    GetComponentInChildren<Animator>().SetTrigger("ataqueD");
    //    yield return new WaitForSeconds(1.2f);
    //    if (!rock.isPlaying && GetComponent<DetectPlayer>().GetTarget() != null)
    //    {
    //        GameObject obj = GetComponent<DetectPlayer>().GetTarget().gameObject;
    //        rock.gameObject.transform.LookAt(obj.GetComponent<StarterAssets.ThirdPersonController>().CinemachineCameraTarget.transform);
    //        rock.Play();
    //    }
    //}

    /// <summary> Funcion publica que devuelve el animator del enemigo. </summary>
    public Animator GetAnim()
    {
        return anim;
    }

    /// <summary> ¿El enemigo se mueve tras detectar al jugador? </summary>
    public bool IsSentinel()
    {
        return _isSentinel;
    }

    /// <summary> Corutina encargada de disparar el cambio de camara y rugir tras haber pillado al jugador con un enemigo. </summary>
    IEnumerator Shout_CRT()
    {
        PlayerRespawn.respawnScript.TurnEnemyCameraTowards(CameraPos);
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("shout", true);
        Invoke("Shout_Continuation_I", 2f);
    }

    /// <summary> Metodo privado que desactiva el nuevo estado de la camara y devuelve al enemigo a sus animaciones de guardia. </summary>
    private void Shout_Continuation_I()
    {
        PlayerRespawn.respawnScript.DeactivateEnemyCamera();
        GetComponent<Animator>().SetBool("idle", true);
        GetComponent<Animator>().SetBool("shout", false);
        Invoke("Shout_Continuation_II", 2.1f);
    }

    /// <summary> Metodo privado que activa la abduccion del jugador. </summary>
    private void Shout_Continuation_II()
    {
        PlayerRespawn.respawnScript.Abduction(true);
    }

    /// <summary> Funcion publica que ejecuta los clips de sonido para las pisadas de los enemigos. </summary>
    public void StepsSound()
    {
        System.Random rnd= new System.Random();
        int i = rnd.Next(10);
        GetComponentInChildren<AudioSource>().volume = 0.3f;
        if (i >= 2) GetComponentInChildren<AudioSource>().PlayOneShot(stepsVariantSFX);
        else GetComponentInChildren<AudioSource>().PlayOneShot(stepsSFX);
    }

    /// <summary> Funcion vacia que ejecuta el clip de audio del rugido enemigo. </summary>
    public void Roar()
    {
        System.Random rnd = new System.Random();
        int i = rnd.Next(10);
        if (i <= 4)
        {
            GetComponentInChildren<AudioSource>().volume = 0.5f;
            GetComponentInChildren<AudioSource>().PlayOneShot(roarSFX);
        }
    }

    /// <summary> Funcion vacia que ejecuta el clip de audio de gruñido enemigo al patrullar. </summary>
    public void WalkRoar()
    {
        System.Random rnd = new System.Random();
        int i = rnd.Next(10);
        if (i <= 3)
        {
            GetComponentInChildren<AudioSource>().volume = 0.6f;
            GetComponentInChildren<AudioSource>().PlayOneShot(walkRoarSFX);
        }
    }

    /// <summary> Funcion vacia que ejecuta el clip de audio de gran rugido enemigo. </summary>
    public void ShoutSFX()
    {
        GetComponentInChildren<AudioSource>().volume = 0.8f;
        GetComponentInChildren<AudioSource>().PlayOneShot(bigRoarSFX);
    }

    /// <summary> Metodo publico que retorna la lista de etapas del camino en la que el enemigo se detiene. </summary>
    public int[] GetStopWaypoints()
    {
        return stopWaypoints;
    }

    /// <summary> ¿Es enemigo de Modo Visita? </summary>
    public bool GetStealthStatus()
    {
        return stealthQuestEnemy;
    }
}
