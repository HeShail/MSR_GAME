using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary> Clase publica que almacena los ajustes de pantalla. </summary>
[System.Serializable]
public class ScreenData 
{
    public int qualityLevel;
    public bool fullscreen;
    public bool vsync;

    /// <summary> Constructor publico de la clase ScreenData. Encargado de almacenar ajustes graficos. </summary>
    public ScreenData (GameQualitySettings gameQ)
    {
        qualityLevel = gameQ.graphicValue;
        vsync = gameQ.vsyncValue;
    }
}
