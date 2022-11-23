using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;

/// <summary> Clase encargada de gestionar los gráficos establecidos. </summary>
public class GameQualitySettings : MonoBehaviour
{
    [Header("Referencias")]
    public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown dropdown;

    [Header ("Valores de seguimiento")]
    public int graphicValue;
    public bool vsyncValue;
    public bool screenValue;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.value = QualitySettings.GetQualityLevel();
        vsyncValue = true;
        screenValue = true;

        if (!SaveSystem.SettingsHavePath()) SaveSystem.InitializeValues(this);
        else LoadSettings();
    }

    /// <summary> Funcion publica que asigna un valor gráfico al juego respecto al valor de un scrollbar. </summary>
    public void ChangeGraphics()
    {
        graphicValue = dropdown.value;
    }

    /// <summary> Funcion pública encargada de inicializar el scrollbar de graficos. </summary>
    public void LoadDropdownValue()
    {
        dropdown.value = graphicValue;
    }

    /// <summary> Funcion pública que modifica los valores gráficos. </summary>
    /// <remarks> Aplica inmediatamente despues los nuevos ajustes y guarda la modificacion.</remarks>
    public void ChangeSettings()
    {
        if (graphicValue == 0)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            QualitySettings.vSyncCount = 1;
        }
        if (graphicValue == 1)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.ExclusiveFullScreen);
            QualitySettings.vSyncCount = 1;

        }
        if (graphicValue == 2)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.ExclusiveFullScreen);
            QualitySettings.vSyncCount = 0;

        }
        QualitySettings.SetQualityLevel(graphicValue);
        QualitySettings.renderPipeline = qualityLevels[graphicValue];
        LoadDropdownValue();
        SaveSystem.SaveGraphicSettings(this);
    }

    /// <summary> Funcion pública que compara los ajustes. </summary>
    /// <remarks> En caso de ser distintos dispara un mensaje de aviso.</remarks>
    public void CompareSettings()
    {
        ScreenData data = SaveSystem.LoadQualitySettings();
        if ((vsyncValue == data.vsync) && (graphicValue == data.qualityLevel)) 
        {
            GetComponent<MenuBehaviour>().mainMenuButtons.SetActive(true);
            GetComponent<MenuBehaviour>().mainMenuButtons.GetComponent<Animator>().SetTrigger("show");

        }
        else
        {
            LoadSettings();
            GetComponent<MenuBehaviour>().HideOptions();
            GetComponent<MenuBehaviour>().mainMenuButtons.SetActive(false);
            GetComponent<MenuBehaviour>().optionsWarning.SetActive(true);

        }
    }

    /// <summary> Funcion pública que carga los valores graficos almacenados. </summary>
    public void LoadSettings()
    {
        ScreenData data = SaveSystem.LoadQualitySettings();
        vsyncValue = data.vsync;
        graphicValue = data.qualityLevel;
        LoadDropdownValue();
    }
}
