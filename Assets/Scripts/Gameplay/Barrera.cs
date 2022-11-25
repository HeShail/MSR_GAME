using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrera : MonoBehaviour
{
    public ParticleSystem ElectricParticle;
    private const float RESPAWN_TIME=3f;
    private void OnTriggerEnter(Collider other)
    {
        //IMPORTANTE

        // REINICIAR ESCENA DESPUÉS DE MORIR

        if(other.gameObject.CompareTag("Player"))
        {
            Electrochute();
            Invoke("Rerun", 4f);
            QuestManager.qmanager.gameObject.GetComponent<AdviserManager>().SetRerun();
        }
     
        other.gameObject.GetComponent<StarterAssets.ThirdPersonController>().Electrocute();
    }
    

    public void Electrochute()
    {
        //ESTO ES UN CAMBIOOOOOOOOOOO  :D
        ElectricParticle.Play();
        AudioManager.audioManager.Electrocute();
        GameObject.Find("GameManager").GetComponent<PlayerRespawn>().blackout_anim.SetTrigger("fundido");
        GameObject.Find("GameManager").GetComponent<PlayerRespawn>().ChangeDeathMessage(1);
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

    public void Rerun()
    {
        GameManager.gm.ChangeScene(2);
    }

    public void RespawnPlayer()
    {
        GameObject.Find("GameManager").GetComponent<PlayerRespawn>().DeathRespawn();
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