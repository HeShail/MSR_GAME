using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary> Clase publica que responde al comportamiento de los objetos del juego. </summary>
public class Item : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private int itemType;
    [SerializeField] private int _id=0;
    public Sprite textura;
    private bool active;
    public TimeTrial timeTrial;
    public SceneryViewer SV;
    
    void Start()
    {
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "Pulsa E para recoger " + _name;
            other.transform.GetComponent<StarterAssets.ThirdPersonController>().SendPickeable(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && active)
        {
            if (Input.GetKey(KeyCode.E))
            {
                //GetComponentInChildren<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (GameManager.gm.GetComponent<ItemSystem>().ContainsItem(_id, itemType)) Debug.Log("YA SE ENCUENTRA GUARDADO");
                else GameManager.gm.GetComponent<ItemSystem>().AddItem(_id, _name, itemType);
                GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";

                if (itemType == 0) TrackPapers();

                Destroy(gameObject, 2f);
                active = false;
            }
        }
        
    }

    /// <summary> Metodo oculto que ejecuta la obtencion de las claves del anexo y repercusiones. </summary>
    private void TrackPapers()
    {
        if (GameManager.gm.GetComponent<ItemSystem>().GetInventory().Count == 7) AudioManager.audioManager.TotalSuccess(); else AudioManager.audioManager.Success();
        if (GameManager.gm.GetComponent<ItemSystem>().GetInventory().Count == 4) GameObject.Find("GameManager").GetComponent<PlayerRespawn>().ChangeRespawn();
        if (textura != null) GameManager.gm.GetComponent<ItemSystem>().SetTexture(textura);
        GameManager.gm.GetComponentInChildren<QuestManager>().UpdateQuest();
        timeTrial.CompleteTrial();
        SV.DeactivateQuest();
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject.Find("GameManager").GetComponent<ItemSystem>().SpitItems();
        GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
        other.GetComponent<StarterAssets.ThirdPersonController>().SendPickeable(false);

    }


}
