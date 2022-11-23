using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Clase utilizada para desarrollar la mec�nica de vallas: sigilo y ruido. </summary>
public class CollisionDetection : MonoBehaviour
{
    [Header("�Es una valla ca�da?")]
    public bool isFallenFence;
    private bool trigger;
    private Vector3 parentPosition;
    private Quaternion parentRotation;
    private const float RELOCAT_TIME=3f;
    private GameManager gm;
    
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        trigger = false;
        parentPosition = GetComponentInParent<Rigidbody>().position;
        parentRotation = GetComponentInParent<Rigidbody>().rotation;
    }

    /// <summary> Funci�n preparada para detectar la colisi�n del jugador. 
    /// Si el jugador colisiona con el trigger mientras corre, es abducido. </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!trigger && !PlayerRespawn.respawnScript.isReappearing)
            {
                if (isFallenFence)
                {
                    gm.GetComponent<PlayerRespawn>().ChangeDeathMessage(0);
                    gm.GetComponent<PlayerRespawn>().Abduction(true);
                    AudioManager.Fence(false);
                    Invoke("DisableRB", RELOCAT_TIME);
                }
                if (other.GetComponent<StarterAssets.ThirdPersonController>().IsRunning() && !isFallenFence)
                {
                    gm.GetComponent<PlayerRespawn>().ChangeDeathMessage(0);
                    gm.GetComponent<PlayerRespawn>().Abduction(true);
                    GetComponentInParent<Rigidbody>().AddExplosionForce(6000f, transform.position, 2f);
                    AudioManager.Fence(false);
                    Invoke("DisableRB", RELOCAT_TIME);
                    trigger = true;
                }
                
                if (other.GetComponent<StarterAssets.ThirdPersonController>().IsWalking() && !isFallenFence)
                {
                    AudioManager.Fence(true);
                    GetComponentInParent<Rigidbody>().AddExplosionForce(150f, transform.position, 2f);
                    
                }
            }
        }
    }


    /// <summary> Funci�n empleada para desactivar la funcionalidad del colisionador tras el contacto. </summary>
    void EnableRB()
    {
        trigger = false;
    }

    /// <summary> M�todo orientado a la recolocaci�n y/o restablecimiento original del 
    /// ojeto en escena mediante el componente Rigidbody </summary>
    void DisableRB()
    {
        GetComponentInParent<Rigidbody>().position = parentPosition;
        GetComponentInParent<Rigidbody>().rotation = parentRotation;
        trigger = false;
    }
}
