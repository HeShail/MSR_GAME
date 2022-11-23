using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary> Clase encargada de gestionar el trigger del panel de contrase�a. </summary>
public class PanelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "Pulsa E para interactuar con un panel";

        }

    }

    /// <summary> Cambia la m�quina de estados al nodo PasswdState, disparando inmediatamente la ejecuci�n de la mec�nica de contrase�as. </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
                PauseMenuManager _manager = GameManager.gm.GetComponent<PauseMenuManager>();
                _manager.gameMachine.ChangeState(new PasswdState(_manager));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) GameObject.FindWithTag("Message").GetComponent<TextMeshProUGUI>().text = "";
    }

    public void ExitPanel()
    {
        PauseMenuManager _manager = GameManager.gm.GetComponent<PauseMenuManager>();
        _manager.gameMachine.ChangeState(new PasswdState(_manager));
    }
}
