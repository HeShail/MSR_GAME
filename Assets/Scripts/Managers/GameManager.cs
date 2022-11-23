using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Clase encargada de gestionar el estado del juego entre escenas. </summary>
public class GameManager : MonoBehaviour
{
    /// <summary> Singleton de la clase GameManager. </summary>
    public static GameManager gm { get; private set; }

    private void Awake()
    {
        if (gm != null && gm != this)
        {
            Destroy(this);
        }
        else
        {
            gm = this;
        }
        
    }

    void Start()
    {

        //Cargamos los ajustes graficos
        if (SaveSystem.SettingsHavePath())
        {
            ScreenData data = SaveSystem.LoadQualitySettings();
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            QualitySettings.vSyncCount = data.qualityLevel;
            if (data.qualityLevel == 0) Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen); else Screen.SetResolution(1280, 720, FullScreenMode.ExclusiveFullScreen);
            
            
        }
        Time.timeScale = 1;
    }

    /// <summary> Metodo que cierra el proceso del juego. </summary>
    public void LeaveGame()
    {
        Application.Quit();
    }

    /// <summary> Funcion encargada de cambiar de escena. </summary>
    /// <param name="scene"> Identificador entero referente a la escena de destino. </param>
    public void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
