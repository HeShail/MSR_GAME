using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

/// <summary> Clase encargada de controlar el flujo de muerte y reaparición del personaje jugador. </summary>
public class PlayerRespawn : MonoBehaviour
{
    public static PlayerRespawn respawnScript { get; private set; }
    public CinemachineVirtualCamera _playerCamera;
    public CinemachineVirtualCamera _enemyCamera;

    [Header("Atributos")]
    public float altitudeValue;
    public float dissolveDelay;
    public const float RESPAWN_TIME = 2.5f;
    public bool isReappearing;
    private bool first_abduction;

    [Header("Referencias a Objs")]
    public GameObject player;
    public AdviserManager advisor;
    public Transform actualSpawn, newSpawn;

    [Header("Referencias a UI")]
    public Animator blackout_anim;
    public Animator abdExpansion_anim;

    [Header("Materiales")]
    private Material[] dissolve_Mats;
    private Material dissolve_Material;

    [Header("Partículas")]
    public ParticleSystem dissolve_burst;
    public ParticleSystem dissolve_slowpaced;
    public ParticleSystem abduction_FX;

    /// <summary> Inicialización del singleton. </summary>
    private void Awake()
    {
        if (respawnScript != null && respawnScript != this)
        {
            Destroy(this);
        }
        else
        {
            respawnScript = this;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        first_abduction = true;
        isReappearing = false;
        advisor.LaunchAdvisor(0);
    }


    void Update()
    {
        //Si el personaje desciende una altura l�mite, este se ve teleportado al respawn
        if ((player.transform.position.y <= altitudeValue) || (Input.GetKeyDown(KeyCode.T)))
        {
            StartCoroutine("RespawnPoint");
        }

    }

    #region Tratamiento Muerte

    /// <summary> Funcion que orienta la camara de muerte hacia el enemigo que te atrapa </summary>
    /// <param name="obj"> Parametro transform de la posicion de camara final</param>
    public void TurnEnemyCameraTowards(Transform obj)
    {
        _enemyCamera.transform.position = obj.transform.position;
        _enemyCamera.transform.rotation = obj.transform.rotation;
        _enemyCamera.Priority = 100;

    }

    /// <summary> Funcion que discrimina la camara de enemigo </summary>
    public void DeactivateEnemyCamera()
    {
        _enemyCamera.Priority = -10;

    }
    
    /// <summary> Corutina que bloquea el movimiento del jugador y enciende el algoritmo de detención, relocación y abducción de forma simultánea. </summary>
    /// <param name="hasAnim"> Booleano que señala si debe ejecutarse o no la animación de personaje. </param>
    public void Abduction(bool hasAnim)
    {
        player.GetComponent<StarterAssets.ThirdPersonController>().MoveBind();
        isReappearing = true;
        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        AudioManager.audioManager.Invoke("Abduction", 0.2f);
        abdExpansion_anim.SetTrigger("expansion");
        Invoke("DeathRespawn", PlayerRespawn.RESPAWN_TIME);
        advisor.StartCoroutine("AbductionDelay_CRT");
        abduction_FX.gameObject.transform.position = player.transform.position;
        abduction_FX.Play();
        Invoke("Fade", 1f);
        if (hasAnim) player.GetComponent<StarterAssets.ThirdPersonController>().body.GetComponent<Animator>().SetBool("abduction", true);
    }

    /// <summary> Método que desactiva la visibilidad del personaje. </summary>
    private void Fade()
    {
        List<Transform> children = new List<Transform>(player.GetComponent<StarterAssets.ThirdPersonController>().body.transform.GetComponentsInChildren<Transform>());
        int i = 0;
        foreach (Transform child in children)
        {
            if (children[i].CompareTag("SkinnedMesh"))
            {
                children[i].gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;

            }
            else if (children[i].CompareTag("Mesh"))
            {
                children[i].gameObject.GetComponent<MeshRenderer>().enabled = false;

            }

            i++;

        }
    }

    /// <summary> Método público que nos permite modificar el punto de reaparición del jugador </summary>
    public void ChangeRespawn()
    {
        actualSpawn = newSpawn;
    }

    #endregion

    #region Tratamiento POST-Muerte
    /// <summary> Función que llama a las partes del personaje jugador y las vuelve de nuevo visibles. </summary>
    public void PostAbduction()
    {
        if (first_abduction) GetComponentInChildren<AdviserManager>().LaunchAdvisor(2);
        player.GetComponent<StarterAssets.ThirdPersonController>().body.GetComponent<Animator>().SetBool("abduction", false);
        List<Transform> children = new List<Transform>(player.GetComponent<StarterAssets.ThirdPersonController>().body.transform.GetComponentsInChildren<Transform>());
        int i = 0;
        foreach (Transform child in children)
        {
            if (children[i].CompareTag("SkinnedMesh"))
            {
                children[i].gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;

            }
            else if (children[i].CompareTag("Mesh"))
            {
                children[i].gameObject.GetComponent<MeshRenderer>().enabled = true;

            }

            i++;

        }
        isReappearing = false;
        player.GetComponent<StarterAssets.ThirdPersonController>().UnbindMovement();

    }


    /// <summary> Metodo usado para dirigir la pantalla de muerte correcta.</summary>
    /// <param name="message"> Identificador entero que señala el tipo de muerte. </param>
    /// <param name="respawnTime"> Punto flotante que indica el tiempo de muerte </param>
    /// <remarks> No confundir con la función ChangeDeathMessage().Esta función prepara el estado del juego para que aquella se ejecute sin inconvenientes.</remarks>
    public void DefeatRespawn(int message, float respawnTime)
    {
        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        blackout_anim.SetTrigger("fundido");
        ChangeDeathMessage(message);
        Invoke("DeathRespawn", RESPAWN_TIME);
    }

    /// <summary> Función muleta empleada para poder disparar la función anidada RespawnPoint() con un Invoke. 
    /// Importante: No altera el mensaje de muerte, debe acompañarse con alguna llamada ChangeDeathMessage(). </summary>
    public void DeathRespawn()
    {
        StartCoroutine("RespawnPoint");
    }


    /// <summary> Metodo usado para dirigir la pantalla de muerte correcta.</summary>
    IEnumerator RespawnPoint()
    {
        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        player.transform.position = actualSpawn.position;
        blackout_anim.SetTrigger("fundido");
        yield return new WaitForSeconds(0.4f);
        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
        GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.GetComponent<Animator>().SetBool("abduction", false);
        GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.GetComponent<Animator>().SetBool("Dead", false);
        Invoke("PostAbduction", 1f);
        Advisor();
        GetComponentInChildren<TimeTrial>().ResetTimeTrial();
        QuestManager.qmanager.Papelitos[QuestManager.qmanager.Index].gameObject.SetActive(false);
    }


    /// <summary> Función que segrega los resultado respecto al parámetro de entrada </summary>
    /// <param name="index"> Identificador entero aporta la señal de tipo de muerte. </param>
    public void ChangeDeathMessage(int index)
    {
        List<Transform> children = new List<Transform>(blackout_anim.gameObject.GetComponentsInChildren<Transform>());
        switch (index)
        {
            case 0:
                children[1].GetComponent<TextMeshProUGUI>().text = "TE HAN ABDUCIDO";
                children[2].GetComponent<TextMeshProUGUI>().text = "Ten más cuidado la proxima vez";
                break;

            case 1:
                children[1].GetComponent<TextMeshProUGUI>().text = "HAS MUERTO";
                children[2].GetComponent<TextMeshProUGUI>().text = "¿Esperabas otra cosa?";
                break;

            case 2:
                children[1].GetComponent<TextMeshProUGUI>().text = "TE HAN PILLADO";
                children[2].GetComponent<TextMeshProUGUI>().text = "Has sido detectado";
                break;
        }
    }


    /// <summary> Método privado utilizado para disparar el aviso del consejero acerca de la muerte por abducción. </summary>
    private void Advisor()
    {
        if (first_abduction)
        {
            advisor.StartCoroutine("AbductionDelay_CRT");
            first_abduction = false;
        }
    }
    #endregion

    #region Old Region
    /// <summary> Funcion empleada para activar el shader desintegración
    /// para todos los objetos del personaje. </summary>
    //public void Desintegration(GameObject body)
    //{
    //    List<Transform> children = new List<Transform>(body.transform.GetComponentsInChildren<Transform>());
    //    int i = 0;
    //    GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.GetComponent<Animator>().SetBool("abduction",true);
    //    foreach (Transform child in children)
    //    {
    //        if (children[i].CompareTag("SkinnedMesh"))
    //        {
    //            if (children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials.Length > 1)
    //            {
    //                dissolve_Mats = children[i].gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
    //                StartCoroutine("DissolveMaterials_CRT");


    //            }
    //            else
    //            {
    //                dissolve_Material = children[i].gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
    //                StartCoroutine("DissolveSingleMaterial_CRT");
    //            }

    //        }else if (children[i].CompareTag("Mesh"))
    //        {
    //            dissolve_Material = children[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    //            StartCoroutine("DissolveSingleMaterial_CRT");
    //        }

    //        i++;

    //    }
    //}

    /// <summary> Función empleada para restablecer la visibilidad completa del 
    /// personaje jugador tras el efecto shader "Desintegración". </summary>
    //public void PostDesintegration()
    //{
    //    List<Transform> children = new List<Transform>(GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.transform.GetComponentsInChildren<Transform>());
    //    int i = 0;
    //    foreach (Transform child in children)
    //    {
    //        if (children[i].CompareTag("SkinnedMesh"))
    //        {
    //            if (children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials.Length > 1)
    //            {
    //                dissolve_Mats = children[i].gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
    //                int j = 0;
    //                foreach (Material mat in dissolve_Mats)
    //                {
    //                    dissolve_Mats[j].SetFloat("_Clip", 0);
    //                    j++;
    //                }
    //            }
    //            else
    //            {
    //                children[i].gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.SetFloat("_Clip", 0);
    //            }

    //        }
    //        else if (children[i].CompareTag("Mesh"))
    //        {
    //            children[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Clip", 0);

    //        }

    //        i++;

    //    }
    //    if (first_abduction) GetComponentInChildren<AdviserManager>().LaunchAdvisor(2);
    //}

    /// <summary> [OBSOLETO] Corutina que activa el efecto disolver "Desintegración" en los objetos con múltiples
    /// materiales y además pone en funcionamiento la reaparición del jugador. </summary>
    //IEnumerator DissolveMaterials_CRT()
    //{
    //    Material[] mats = dissolve_Mats;
    //    float elapsedTime = 0;
    //    float waitTime = 2f;
    //    float minValue = 0;
    //    float maxValue = 0.6f;

    //    yield return new WaitForSeconds(dissolveDelay);
    //    blackout_anim.SetTrigger("fundido");
    //    AudioManager.audioManager.Abduction();
    //    Invoke("DeathRespawn", PlayerRespawn.RESPAWN_TIME - 2f);
    //    advisor.StartCoroutine("AbductionDelay_CRT");
    //    player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
    //    dissolve_burst.Play();
    //    dissolve_slowpaced.Play();
    //    while (elapsedTime < waitTime)
    //    {
    //        float t = Mathf.Lerp(minValue, maxValue, elapsedTime / waitTime);
    //        int j = 0;
    //        foreach (Material mat in mats)
    //        {
    //            mats[j].SetFloat("_Clip", t);
    //            elapsedTime += Time.deltaTime;
    //            yield return null;
    //            j++;
    //        }
    //    }

    //}


    /// <summary> [OBSOLETO] Corutina que activa el efecto disolver "Desintegración" en los objetos y
    /// además pone en funcionamiento la reaparición del jugador. </summary>
    //IEnumerator DissolveSingleMaterial_CRT()
    //{
    //    Material mat = dissolve_Material;
    //    float elapsedTime = 0;
    //    float waitTime = 2f;
    //    float minValue = 0;
    //    float maxValue = 0.6f;

    //    yield return new WaitForSeconds(dissolveDelay);

    //    while (elapsedTime < waitTime)
    //    {
    //        float t = Mathf.Lerp(minValue, maxValue, elapsedTime / waitTime);
    //        mat.SetFloat("_Clip", t);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    mat.SetFloat("_Clip", maxValue);
    //    yield return null;

    //}
    #endregion

}
