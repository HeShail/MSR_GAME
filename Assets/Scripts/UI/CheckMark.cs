using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary> [En desuso] Clase publica que controla los toogles de pantalla completa y vsync. </summary>
public class CheckMark : MonoBehaviour
{
    public GameQualitySettings sett;
    public bool value;
    public bool screen;

    void Update()
    {
        if (screen)
        {
            //value = GameAspect.GetFullscreenState();
            value = sett.screenValue;
        }
        else
        {
            //value = GameAspect.GetVsyncState();
            value = sett.vsyncValue;

        }
        transform.GetChild(0).gameObject.SetActive(value);
    }

    //public void Marcar()
    //{
    //    if (value)
    //    {
    //        value = false;
    //        transform.GetChild(0).gameObject.SetActive(value);
    //    }
    //    else 
    //    {
    //        value = true;
    //        transform.GetChild(0).gameObject.SetActive(value);

    //    }
    //    if (screen) sett.ChangeScreen(); else sett.ChangeVsync();
    //}

}
