using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/// <summary> Clase que verifica la prueba de juego. </summary>
public class DateRestrictorManager : MonoBehaviour
{
    public GameObject authenticatorErrorPanel;
    public CinematicPlayer CMP;
    private DateTime dateLimit;
    private DateTime actualDate;

    void Start()
    {
        dateLimit= new DateTime(2022, 12, 01);
        dateLimit.AddHours(1);

    }

    /// <summary> Comprobamos que la versión está en el periodo de prueba si no, te impide acceder al gameplay. </summary>
    public void IsTrialOperative()
    {
        actualDate = DateTime.UtcNow;
        actualDate.AddHours(1);
        if (DateTime.Compare(dateLimit, actualDate) <= 0) 
        {
            authenticatorErrorPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Valid authentication: " + actualDate.ToString("MMM dd, yyyy") + " is under trial limit of " + dateLimit.ToString("MMM dd, yyyy"));
            CMP.PlayInitialCinematic();
        }
    }

}
