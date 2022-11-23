using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary> Clase encargada de controlar el desafio contrarreloj </summary>
public class TimeTrial : MonoBehaviour
{
    [Tooltip ("Gestiona la duracion del desafio contrarreloj")]
    public float timer;
    [Tooltip ("¿Tiene que activarse el desafio contrarreloj?")]
    public bool isTrial;
    [Tooltip ("Gestiona cuando se activa o desactiva el desafio contrarreloj")]
    public bool startTrial = false;

    [Header("COMPONENTES")]
    public GameObject timeTrialDisplay;
    public GameObject [] SV;
    public TextMeshProUGUI timeText,infoText;
    public GameObject [] Enemy;
   
    // Start is called before the first frame update
    private void Start()
    {
        timeTrialDisplay.SetActive(false);      
    }

    private void Update()
    {
        if(timeText != null)
        {    
            timeText.text = timer.ToString("F2");
        } 
        
        if (startTrial == true)
        {   
            timer -= Time.deltaTime;
            QuestManager.qmanager.ActivateAdvise(2);

            if (timer <= 15f)       
            {
                QuestManager.qmanager.ActivateAdvise(3);
            }
            if((timer <= 0f) && (startTrial == true))
            {
                GameObject.Find("PlayerController").GetComponent<StarterAssets.ThirdPersonController>().MoveBind();
                GameObject.Find("GameManager").GetComponent<PlayerRespawn>().Abduction(true);
                ResetTimeTrial();
            }
        }   
    }

    /// <summary> Funcion que inicia el desafio contrarreloj correspondiente  </summary>
    /// <param name="isTrial"> Booleano que señala si debe ejecutarse o no el desafio contrarreloj </param> 
    public void StartTimeTrial()
    {
        if(isTrial == true)
        {   
            timeTrialDisplay.SetActive(true);
            timeText = GameObject.Find("TimeTrial").GetComponent<TextMeshProUGUI>(); 
            startTrial = true;
            SetTimeTrial();
            SV[QuestManager.qmanager.Index].GetComponent<CapsuleCollider>().enabled = false;
        }    
    }

    
    /// <summary> Funcion que completa el desafio correspondiente al conseguir la pista </summary>
    /// <param name="isTrial"> Booleano que señala si debe ejecutarse o no el desafio contrarreloj </param>
    public void CompleteTrial()
    {
        if(isTrial == true)
        {
            timeText = null;
            isTrial = false;
            startTrial = false;
            SetTimeTrial();
            QuestManager.qmanager.DeactivateAdvise();
            timeTrialDisplay.SetActive(false);
            if(QuestManager.qmanager.Index < 7)
            {
                SV[QuestManager.qmanager.Index].GetComponent<SceneryViewer>().observado = true;
                SV[QuestManager.qmanager.Index].GetComponent<CapsuleCollider>().enabled = true;
            }
            else
            {
                SV[QuestManager.qmanager.Index-1].GetComponent<SceneryViewer>().observado = true;
                SV[QuestManager.qmanager.Index-1].GetComponent<CapsuleCollider>().enabled = true;
            }
            
        }
    }

    
    /// <summary> Funcion que reinicia el desafio si se acaba el tiempo o el jugador es detectado durante el desafio  </summary>
    public void ResetTimeTrial()
    {
        startTrial = false;
        SetTimeTrial();
        QuestManager.qmanager.DeactivateAdvise();
        timeTrialDisplay.SetActive(false);
        SV[QuestManager.qmanager.Index].GetComponent<CapsuleCollider>().enabled = true;
    }
    
   
    /// <summary> Funcion encargada de asignar las variables correspondientes al desafio. Ademas, reestablece los valores al fallar o completar el desafio  </summary>
    /// <param name="Index"> Variable que gestiona el desafio que debe activarse o desactivarse </param>
    public void SetTimeTrial()
    {
        switch (QuestManager.qmanager.Index)
        {
            case 0:
                timer = 20f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(0);
                else AudioManager.audioManager.StopCrono();
                break;

            case 1:
                timer = 30f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(0);
                else AudioManager.audioManager.StopCrono();
                break;

            case 2:
                timer = 30f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(0);
                else AudioManager.audioManager.StopCrono();
                break;

            case 3:
                timer = 30f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(0);
                else AudioManager.audioManager.StopCrono();
                break;

            case 4:
                timer = 45f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(1);
                else AudioManager.audioManager.StopCrono();
                break;

            case 5:
                timer = 45f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) 
                {
                    AudioManager.audioManager.Cronometer(1);
                    Enemy[0].SetActive(true);
                }
                else 
                {
                    AudioManager.audioManager.StopCrono();
                    Enemy[0].SetActive(false);
                }
                break;

            case 6:
                timer = 45f;
                SV[QuestManager.qmanager.Index].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(1);
                else AudioManager.audioManager.StopCrono();
                Enemy[0].SetActive(false);
                break;

            case 7:
                timer = 60f;
                SV[QuestManager.qmanager.Index-1].GetComponent<MeshRenderer>().enabled = true;
                if(startTrial == true) AudioManager.audioManager.Cronometer(1);
                else AudioManager.audioManager.StopCrono();
                break;
        }
    }
}
