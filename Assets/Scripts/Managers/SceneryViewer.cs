using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;

public class SceneryViewer : MonoBehaviour
{
    [Header("Cinematica")]
    private StarterAssets.ThirdPersonController player;
    private CinemachineVirtualCamera playerCamera;
    private CinemachineVirtualCamera viewCamera;
    private CinemachineVirtualCamera activeCamera = null;
    public GameObject message;
    public bool observado;
    public bool noText;
    [Header("Trial")]
    public TimeTrial timeTrial;
    public bool isTrial, isLocked;
    [Header("Progresion")]
    public Image DescriptionViewer;
    public Sprite TextViewer;
    public QuestManager qManager;
    public Material Completed;



    void Start()
    {
        observado = false;
        viewCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        
    }

    //Comprueba si el jugador entra en el visor
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateCameraVisor();
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (observado)
                {
                    Invoke("AwakeCinematic", 1f);
                    GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
                    player.MoveBind();
                }
            }
            if((Input.GetKey(KeyCode.Return)) && (message.activeSelf))
            {    
                EndCinematic();              
            }
        }
        
          
            
    }
    //Comprueba si el jugador sale del visor
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    private void ExitVisor()
    {   
        activeCamera = null;
        GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
        qManager.Papelitos[qManager.Index].gameObject.SetActive(true);
        if ((!observado)&& (!noText)) qManager.ActivateAdvise(0);
        if(isTrial == true) 
        {
            timeTrial.StartTimeTrial();
            GetComponent<MeshRenderer>().enabled = false;
        }    
    }

    public void ActivateCameraVisor()
    {
        if(isLocked == false)
            {   
                GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
                activeCamera = playerCamera;
                player = GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>();
                playerCamera = GameObject.Find("MoveCamera").GetComponent<CinemachineVirtualCamera>();
                if (!observado)
                {
                    if(isTrial)
                {
                    timeTrial.isTrial = true;
                } 
                Invoke("AwakeCinematic", 1f);
                player.MoveBind();
                }
                else 
                {
                GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "Interactuar con tecla E";
                } 
            }
            else
            {
                GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "Objetivo no disponible.Vuelve más tarde";
            }
    }

    public bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return activeCamera == camera;
    }

    public void AwakeCinematic()
    {
        if((qManager.Index != 7 ) && (!noText)) Invoke("ActivateVisor",2.5f);
       
        player.enabled = false;
        viewCamera.Priority = 20;
    }
    public void EndCinematic()
    {
        viewCamera.Priority = 0;
        if(!noText) message.SetActive(false);    
        Invoke("MobilityReturn", 2f);
        Invoke("ExitVisor", 2.5f);
    }

    public void MobilityReturn()
    {
        player.enabled = true;
        player.UnbindMovement();
        
    }
    public void ActivateVisor()
    {
        message.SetActive(true);
        DescriptionViewer = GameObject.Find("Descriptor_ArtistValley").GetComponent<Image>();
        DescriptionViewer.sprite = TextViewer;
    }
    public void DeactivateQuest()
    {
        isTrial = false;
        GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
        if(!isLocked)GetComponent<MeshRenderer>().material =Completed;
        GetComponent<MeshRenderer>().enabled = true;
    }


    

}
