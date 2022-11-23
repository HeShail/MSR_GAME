using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase encargada de comprobar la colision con objetos del personaje jugador dentro de la carpa de conciertos </summary>
public class Drum : MonoBehaviour
{
    private GameManager gm;

    // Start is called before the first frame update
    public void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary> Comprueba la colision del jugador con el obstaculo  </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")&& (!PlayerRespawn.respawnScript.isReappearing))
        {
                other.GetComponent<StarterAssets.ThirdPersonController>().MoveBind();
                AbductionDissolve();
                AudioManager.audioManager.Drum();
                gm.GetComponent<PlayerRespawn>().ChangeDeathMessage(0);   
                Invoke("RespawnPlayer", PlayerRespawn.RESPAWN_TIME);
        }
    }

    /// <summary> Funcion encargada de activar el sistema de reaparecer  </summary>
    private void RespawnPlayer()
    {
        gm.GetComponent<PlayerRespawn>().DeathRespawn();
    }

    /// <summary> Funcion encargada de activar el sistema de abduccion  </summary>
    /// <param name="hasAnim"> Booleano que señala si debe ejecutarse o no la animación de personaje. </param>
    private void AbductionDissolve()
    {
        gm.GetComponent<PlayerRespawn>().Abduction(true);
    }
}
