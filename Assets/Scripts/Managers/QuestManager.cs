using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager qmanager { get; private set; }
    [Header("Panel Quest")]
    public GameObject PanelQuest;
    public GameObject ClosedPanel;
    public TextMeshProUGUI Quests;
    public GameObject StandInfo;
    public GameObject[] VisoresQuest;
    public GameObject[] Papelitos;
    public bool activePanel = true;

    [Header ("Avisos")]
    public Animator pastillaAviso;
    public TextMeshProUGUI pastillaClaves;
    private const float ADVISE_TEMP = 7f;
    private bool adviseIsOn;
    public int Index = 0;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (qmanager != null && qmanager != this)
        {
            Destroy(this);
        }
        else
        {
            qmanager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.J)) QuestPanel();    
    }
    
    public void UpdateQuest()
    {
        Index++;
        Debug.Log(""+ Index);
        if (StandInfo == null)
        {
           if(Index <= VisoresQuest.Length) VisoresQuest[Index].gameObject.SetActive(true);
            pastillaClaves.text = "CLAVES: " + Index + "/7";
            GameObject.Find("CinematicManager").GetComponent<CinematicPlayer>().Invoke("PlayCinematic", 1f);

        }
    }

    public string AvisoCorto(int id)
    {
        string texto="";
        switch (id)
        {
            case 0:
                texto = "Tienes una pista muy cerca de ti";
                break;
            case 1:
                texto = "Los extraterrestres te detectan";
                break;

            // Casos 2 y 3: Avisos de trials

            case 2:
                texto = "El enemigo se acerca , encuentra la pista antes de que te detecten";
                break;
            case 3:
                texto = "Date prisa, se están acercando demasiado";
                break;
        }
        return texto;
    } 

    public void ActivateAdvise(int id)
    {
        if (adviseIsOn) 
        {
            StopCoroutine("Advise_CRT");
        }
        else
        {
            adviseIsOn = true;
            pastillaAviso.SetBool("state", true);
        }

        pastillaAviso.GetComponentInChildren<TextMeshProUGUI>().text = AvisoCorto(id);
        StartCoroutine("Advise_CRT");

    }
    public void DeactivateAdvise()
    {
        StopAllCoroutines();
        pastillaAviso.GetComponentInChildren<TextMeshProUGUI>().text = "";
        pastillaAviso.SetBool("state",false);
        pastillaAviso.SetTrigger("shut");
        Debug.Log("desactivar");
        adviseIsOn = false;
    }

    private IEnumerator Advise_CRT()
    {

        yield return new WaitForSeconds(ADVISE_TEMP);
        pastillaAviso.SetBool("state", false);
        adviseIsOn = false;
    }

    /*public void QuestPanel()
    {
        if(!activePanel)
        {
            ClosedPanel.gameObject.SetActive(false);
            PanelQuest.gameObject.SetActive(true);
            activePanel = true;

        }
        else
        {
            ClosedPanel.gameObject.SetActive(true);
            PanelQuest.gameObject.SetActive(false);
            activePanel = false;
        }
    }*/
}
