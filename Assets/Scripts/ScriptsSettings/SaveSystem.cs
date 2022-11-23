using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary> Clase publica que encripta, almacena y rescata datos del juego. </summary>
public static class SaveSystem 
{
    /// <summary> Funcion estatica que guarda los ajustes graficos del juego. </summary>
    public static void SaveGraphicSettings(GameQualitySettings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/screen.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        ScreenData data = new ScreenData(settings);
        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Guardado de ajustes");
    }

    /// <summary> Funcion estatica que carga los ajustes graficos del juego. </summary>
    public static ScreenData LoadQualitySettings()
    {
        string path = Application.persistentDataPath + "/screen.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ScreenData data = formatter.Deserialize(stream) as ScreenData;
            stream.Close();
            return data;

        }
        else
        {
            Debug.Log("No se ha encontrado archivo en " + path);
            return null;
        }
    }

    /// <summary> Funcion estatica que detecta si encuentra archivos guardados en la ruta asignada. </summary>
    public static bool SettingsHavePath()
    {
        string path = Application.persistentDataPath + "/screen.data";
        if (File.Exists(path)) return true; else return false;
    }

    /// <summary> Funcion estatica que almacena y encripta por primera vez los ajustes graficos. </summary>
    public static void InitializeValues(GameQualitySettings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path2 = Application.persistentDataPath + "/screen.data";
        FileStream stream = new FileStream(path2, FileMode.Create);

        ScreenData data = new ScreenData(settings);
        formatter.Serialize(stream, data);
        stream.Close();
    }
}
