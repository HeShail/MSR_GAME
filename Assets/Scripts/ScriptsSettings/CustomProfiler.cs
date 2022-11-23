using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary> Clase publica que monitorea y muestra la tasa de refresco de la pantalla en tiempo real. </summary>
public class CustomProfiler : MonoBehaviour
{
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    private bool showFPS;
    public const float m_refreshTime = 0.5f;

    private void Start()
    {
        if (this.gameObject.GetComponent<TextMeshProUGUI>()) showFPS = true;
        else showFPS = false;
    }

    void Update()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }

        if (showFPS) GetComponent<TextMeshProUGUI>().text = "" + (int)m_lastFramerate;

    }

    /// <summary> Metodo publico que retorna los frames por segundo del juego.  </summary>
    public int GetFPS()
    {
        return (int)m_lastFramerate;
    }

    
}
