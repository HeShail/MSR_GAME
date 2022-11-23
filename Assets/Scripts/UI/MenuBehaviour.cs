using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase publica orientada a la gestion del menu de Inicio. </summary>
public class MenuBehaviour : MonoBehaviour
{
    public GameObject[] optionsOBJs;
    public GameObject optionsWarning;
    public GameObject mainMenuButtons;

    /// <summary> Funcion publica que invoca la corutina Options_Coroutine(). </summary>
    public void ActivateOptions()
    {
        StartCoroutine("Options_Coroutine");
    }

    /// <summary> Corutina que organiza la muestra de los botones de opciones. </summary>
    IEnumerator Options_Coroutine()
    {
        optionsOBJs[0].SetActive(true);
        optionsOBJs[1].SetActive(true);

        yield return new WaitForSeconds(1.1f);
        for(int i = 0; i < optionsOBJs.Length; i++)
        {
            optionsOBJs[i].SetActive(true);
        }
    }

    /// <summary> Funcion publica que activa todos los objetos de opciones. </summary>
    public void DeactivateOptions()
    {
        for (int i = 0; i < optionsOBJs.Length; i++)
        {
            optionsOBJs[i].SetActive(false);
        }
    }

    /// <summary> Funcion publica que esconde el titulo de opciones y desactiva el resto de objetos. </summary>
    public void HideOptions()
    {
        optionsOBJs[1].GetComponent<Animator>().SetTrigger("hide");
        for (int i = 2; i < optionsOBJs.Length; i++)
        {
            optionsOBJs[i].SetActive(false);
        }
        Time.timeScale = 1;
    }

    /// <summary> Funcion publica que muestra el titulo de opciones y activa el resto de objetos. </summary>
    public void UnhideOptions()
    { 
        for (int i = 2; i < optionsOBJs.Length; i++)
        {
            optionsOBJs[i].SetActive(true);
        }
        optionsOBJs[1].GetComponent<Animator>().SetTrigger("hide");
        Time.timeScale = 1;
    }

    /// <summary> Funcion publica que activa todos los objetos de opciones. </summary>
    public void EnableOptions()
    {
        for (int i = 0; i < optionsOBJs.Length; i++)
        {
            optionsOBJs[i].SetActive(true);
        }
    }
}
