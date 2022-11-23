using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Clase que maneja el comportamiento del panel de acceso al anexo. </summary>
public class PasswordPanelBehaviour : MonoBehaviour
{
    [Header("Parámetros")]
    [SerializeField] private bool isActive;
    private int[] correctPasswd;
    private int[] passwdEntry = new int[7];

    [Header("GameObjects")]
    public GameObject triggerPanel;
    public GameObject escudoPrueba;
    public GameObject passwordPanel;
    public List<GameObject> passwd;

    [Header ("Audios")]
    public AudioSource sfxAudioSource;
    public AudioClip errorSound;

    [Header("Partículas")]
    public ParticleSystem ElectricParticle;

    void Start()
    {  
        correctPasswd = new int[7] {2, 8, 7, 3, 4, 1, 5};
        step = 0;
        isActive = false;
        SetPanel(false);
    }

    [Tooltip ("Entero que señala la posicion de la contraseña en la que te encuentras al pulsar")]
    private int step;
    /// <summary> Funcion publica que lee e interpreta las teclas pulsadas del panel </summary>
    /// <param name="id"> Identificador entero que corresponde a la tecla pulsada </param>
    public void ButtonIDPressed(int id)
    {
        if (isActive)
        {
            if (step < correctPasswd.Length)
            {
                if (correctPasswd[step].Equals(id))
                {
                    passwd[step].transform.GetChild(0).gameObject.SetActive(true);
                    step++;

                }
                else
                {
                    for (int i = 0; i < correctPasswd.Length; i++)
                    {
                        passwd[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    GetComponent<PauseMenuManager>().gameMachine.ChangeState(new LiveState(GetComponent<PauseMenuManager>()));
                    Electrochute();
                    sfxAudioSource.PlayOneShot(errorSound);
                    step = 0;
                }

            }
            if (step == correctPasswd.Length)
            {
                SetPanel(false);
                escudoPrueba.SetActive(false);
                step = 0;
                GameObject.Find("CinematicManager").GetComponent<CinematicPlayer>().videoCount = 10;
                GameObject.Find("CinematicManager").GetComponent<CinematicPlayer>().PlayCinematic();
                Invoke("DestroyTrigger",2f);
            }
        }

        
    }

    /// <summary> Método privado que suprime el trigger. </summary>
    private void DestroyTrigger()
    {
        Destroy(triggerPanel);
    }

    /// <summary> ¿Esta el panel de contraseña activado actualmente? </summary>
    public bool IsPanelActivated()
    {
        return isActive;
    }

    public void SetPanel(bool state)
    {
        isActive = state;
        if (state)
        {
            step = 0;
            passwordPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            passwordPanel.SetActive(false);
        }
    }

    public void SetActive(bool status)
    {
        isActive = status;
    }

    #region Electrochute_System
    public void Electrochute()
    {
        ElectricParticle.Play();
        AudioManager.audioManager.Electrocute();
        Invoke("RespawnCall",0.5f);
        Invoke("RespawnPlayer", 1f);
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
    public void RespawnCall()
    {
        GetComponent<PlayerRespawn>().Abduction(false);
    }
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
#endregion
}
