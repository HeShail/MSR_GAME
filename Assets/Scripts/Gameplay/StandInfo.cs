using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StandInfo : MonoBehaviour
{
    [Header("COMPONENTS")]
    public GameObject Pause;
    public CinematicPlayer CP;
    public GameObject message;

    [Header("COUNTER")]
    public int index;

    [Header("UI")]
    public Image DescriptionViewer;
    public Sprite TextViewer1;

    void Start()
    {
        index = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player"))
        {
            message.SetActive(true);
            DescriptionViewer = GameObject.Find("Descriptor_ArtistValley").GetComponent<Image>();
            DescriptionViewer.sprite = TextViewer1;
        } 
    }

    private void OnTriggerStay (Collider other)
    {
        if(Input.GetKey(KeyCode.Return))
        {
            InfoMessage();             
        }
    }

    public void InfoMessage()
    {
        
        message.SetActive(false);
        Pause.GetComponent<MapManager>().MinimapStatus(true);
        Pause.GetComponent<MapManager>().EnableMap();
        GameObject.Find("QuestManager").GetComponent<QuestManager>().VisoresQuest[0].gameObject.SetActive(true);
        Invoke("PlayVideo",1f);
        Invoke("DestroyPanel",1f);
    }

    public void PlayVideo()
    {
        CP.PlayCinematic();
    }
    private void DestroyPanel()
    {
        Destroy(gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        message.SetActive(false);
        
    }
}
