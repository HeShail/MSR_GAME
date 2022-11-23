using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase encargada de activar el mensaje final del juego al intentar entrar al anexo </summary>
/// <param name="isActive"> Booleano que gestiona al activacion del cursor del raton </param>
public class InteriorEntry : MonoBehaviour
{
    public GameObject endingMenu, creditos;
    public Animator creditsAnimator;
    public StarterAssets.ThirdPersonController player;
    
    [SerializeField] private bool isActive;

    // Start is called before the first frame update
    private void Start()
    {
        endingMenu.SetActive(false);
        isActive = false;        
    }
     
    private void Update()
    {
        if(isActive) Cursor.lockState = CursorLockMode.None;
    }

    /// <summary> Comprueba la colision del jugador con el trigger que activa el mensaje final </summary>   
    private void OnTriggerEnter(Collider other)
    {
        endingMenu.SetActive(true);
        isActive = true;
        player.MoveBind();
        Credits();
    }
    
    public void Credits()
    {
        creditsAnimator.SetTrigger("ActivateCredits");
    }

    public void CloseCredits()
    {
        GameManager.gm.ChangeScene(1);
    }

}
