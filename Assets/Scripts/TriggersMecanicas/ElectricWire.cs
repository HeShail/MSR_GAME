using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Clase encargada de controlar el sistema de corriente electrica y su consecuente proceso de abduccion </summary>
public class ElectricWire : MonoBehaviour
{
    public ParticleSystem ElectricParticle, Explosion;
    private GameManager gm;
    
    // Start is called before the first frame update
    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary> Comprueba la colision del jugador con el sistema de particulas </summary>
    private void OnParticleCollision(GameObject other)
    {   
        if((other.gameObject.CompareTag("Player")) && (!PlayerRespawn.respawnScript.isReappearing))
        {
            PlayerRespawn.respawnScript.isReappearing = true;
            Electrochute();
        }

    }

    /// <summary> Funcion encargada de ejecutar el sistema de electrocucion y sus consecuentes procesos </summary>
    public void Electrochute()
    {
        ElectricParticle.Play();
        Invoke("ExplosionServer", 3f);
        GetComponent<SceneryViewer>().ActivateCameraVisor();
        AudioManager.audioManager.Electrocute();
        GetComponent<SceneryViewer>().Invoke("EndCinematic",7f);
        Invoke("RespawnCall",4f);
        Invoke("RespawnPlayer",1f);
        GameObject.Find("GameManager").GetComponent<PlayerRespawn>().ChangeDeathMessage(0);
        List<Transform> children = new List<Transform>(GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.transform.GetComponentsInChildren<Transform>());
        int i = 0;
        foreach (Transform child in children)
        {
            if (children[i].CompareTag("SkinnedMesh"))
            {
                if (children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials.Length > 1)
                {
                    Material[] dissolve_Mats = children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials;
                    foreach (Material mat in dissolve_Mats)
                    {
                        mat.SetFloat("_ElectricBool", 1.0f);
                    }
                }
                else
                {
                    children[i].gameObject.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_ElectricBool", 1.0f);
                }

            }else if (children[i].CompareTag("Mesh"))children[i].gameObject.GetComponent<MeshRenderer>().material.SetFloat("_ElectricBool", 1.0f);
            i++;

        }
    }
    
    /// <summary> Funcion encargada de ejecutar el sistema de particulas de explosion </summary>
    public void ExplosionServer()
    {
        Explosion.Play();
        AudioManager.audioManager.Pum();
    }

    /// <summary> Funcion encargada de activar el sistema de abduccion  </summary>
    /// <param name="hasAnim"> Booleano que señala si debe ejecutarse o no la animación de personaje. </param>
    public void RespawnCall()
    {
        gm.GetComponent<PlayerRespawn>().Abduction(false);
    }

    /// <summary> Funcion encargada de desactivar el shader de electrocucion al reaparecer </summary>
    public void RespawnPlayer()
    {
        
        List<Transform> children =
            new List<Transform>(GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().body.transform.GetComponentsInChildren<Transform>());
        int i = 0;
        foreach (Transform child in children)
        {
            if (children[i].CompareTag("SkinnedMesh"))
            {
                if (children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials.Length > 1)
                {
                    Material[] dissolve_Mats = children[i].gameObject.GetComponent<SkinnedMeshRenderer>().materials;
                    foreach (Material mat in dissolve_Mats)
                    {
                        mat.SetFloat("_ElectricBool", 0.0f);
                    }
                }
                else
                {
                    children[i].gameObject.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_ElectricBool", 0.0f);
                }

            }
            else if (children[i].CompareTag("Mesh")) children[i].gameObject.GetComponent<MeshRenderer>().material.SetFloat("_ElectricBool", 0.0f);
            i++;

        }
        ElectricParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
