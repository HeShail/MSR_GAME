using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary> Clase que opera el consejero tutorial del marco izquierdo </summary>
public class AdviserManager : MonoBehaviour
{
    [Header("Panel del Consejero")]
    public GameObject panel;

    private string message;
    private string secondpart_message;
    private static bool is_rerun = false;

    private void Start()
    {
        panel.SetActive(false);
    }

    /// <summary> Método encargado de seleccionar el mensaje del consejero respecto al parámetro de entrada. </summary> 
    /// <param name="id"> Identificador entero que señala el tipo de mensaje. </param>
    public void LaunchAdvisor(int id)
    {
        switch (id)
        {
            case 0:

                if (!is_rerun)
                {
                    message = "Los alienigenas han situado vigilantes para cazar a más personas - Si haces ruido te detectarán y abducirán";
                    Invoke("Advisor", 3f);
                }
                else
                {
                    message = "Cuando mueres, pierdes el progreso y comienzas desde el principio";
                    Invoke("Advisor", 3f);
                }
                break;

            case 1:
                message = "Los vigilantes pueden verte y oirte si te acercas - ¡Estudia sus movimientos!";
                Invoke("Advisor", 3f);
                break;
            case 2:
                message = "Cuando te abducen, vuelves a la entrada del Salón - Inténtalo de nuevo";
                Invoke("Advisor",3f);
                break;
        }
    }


    /// <summary> Función que modifica el estado de la UI y el componente Text de TMPro para emitir el mensaje. 
    /// También programa su desactivación. </summary>
    private void Advisor()
    {
        
        panel.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        panel.GetComponentInChildren<TextMeshProUGUI>().text = message;
        panel.SetActive(true);
        AudioManager.audioManager.AdvisorCall();
        Invoke("DeactivateAdvisor", 8f);
    }


    /// <summary> Función muleta para ejecutar la corutina anidada con un temporizador Invoke. </summary>
    private void ComplexAdvisor()
    {
        StartCoroutine("ComplexAdvisor_CRT");

    }

    /// <summary> Corutina que prepara el estado de la UI y el componente text de TMPro para dos llamadas de mensaje. </summary>
    IEnumerator ComplexAdvisor_CRT()
    {
        panel.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
        panel.GetComponentInChildren<TextMeshProUGUI>().text = message;
        panel.SetActive(true);
        AudioManager.audioManager.AdvisorCall();
        yield return new WaitForSeconds(4f);
        panel.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        panel.GetComponentInChildren<TextMeshProUGUI>().text = secondpart_message;
        Invoke("DeactivateAdvisor", 5f);

    }


    /// <summary> Corutina que comunica la abducción del personaje jugador. </summary>
    IEnumerator AbductionDelay_CRT()
    {
        yield return new WaitForSeconds(7f);
        LaunchAdvisor(2);
    }


    /// <summary> Método privado que desactiva el consejero. </summary>
    private void DeactivateAdvisor()
    {
        panel.SetActive(false);
    }


    /// <summary>Método público que se ejecuta al inicio y que nos advierte de que esta es una escena recargada. </summary>
    /// <remarks> Imprescindible para disparar el mensaje del consejero referido a la muerte del jugador. </remarks>

    public void SetRerun()
    {
        is_rerun = true;
    }
}
