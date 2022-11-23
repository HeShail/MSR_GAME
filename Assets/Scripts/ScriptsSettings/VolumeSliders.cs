using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary> Clase que inicializa los sliders de volumen. </summary>
public class VolumeSliders : MonoBehaviour
{
    [Header ("Variables en Uso")]
    [SerializeField] Slider masterVolume;
    [SerializeField] Slider trackVolume;
    [SerializeField] Slider soundMacroVolume;

    [Header("Variables en Desuso")]
    [SerializeField] Slider environmentVolume;
    [SerializeField] Slider soundfxVolume;
    [SerializeField] Toggle muteToggle;
    [SerializeField] GameObject toggleCheckMark;

    private void Update()
    {
        if (AudioManager.audioManager.IsGameMute()) toggleCheckMark.SetActive(true); else toggleCheckMark.SetActive(false);
    }

    /// <summary> Funcion publica que inicializa el volumen maestro. </summary>
    public void MasterVolumeChange()
    {
        AudioManager.audioManager.SetMasterVolume(masterVolume.value);
    }

    /// <summary> Funcion publica que inicializa el volumen de la musica. </summary>
    public void AudiotrackVolumeChange()
    {
        AudioManager.audioManager.SetTrackVolume(trackVolume.value);

    }

    // WARNING //
    //ONLY USED WHEN THERE´S NO PRESSENCE OF ANY KIND OF ORIENTED VOLUME SLIDERS SUCH AS ENVIRONMENT,SFX...
    /// <summary> Funcion publica que inicializa el volumen del slider de macros de sonido. </summary>
    public void SoundMacrosChange()
    {
        AudioManager.audioManager.SetSFXVolume(soundMacroVolume.value);
        AudioManager.audioManager.SetPlayerVolume(soundMacroVolume.value);
        AudioManager.audioManager.SetEnvironmentVolume(soundMacroVolume.value);
        AudioManager.audioManager.SetSOUNDVolume(soundMacroVolume.value);
    }

    //public void EnvironmentVolumeChange()
    //{
    //    AudioManager.audioManager.SetEnvironmentVolume(environmentVolume.value);
    //    AudioManager.audioManager.SetPlayerVolume(soundfxVolume.value);

    //}
    //public void SoundVolumeChange()
    //{
    //    AudioManager.audioManager.SetSFXVolume(soundfxVolume.value);
    //}

    //EXTENDED VOLUME FUNCTION SETTER

    //public void SetVolumeValues(float master, float track, float environment, float sfx, bool mute_value)
    //{
    //    masterVolume.value = master;
    //    trackVolume.value = track;
    //    environmentVolume.value = environment;
    //    soundfxVolume.value = sfx;
    //    toggleCheckMark.SetActive(mute_value);
    //}

    /// <summary> Funcion publica que inicializa el valor de las variables 
    /// que determinan el volumen asignado a las fuentes de audio. </summary>
    public void SetVolumeValuesII(float track, float sound)
    {
        trackVolume.value = track;
        soundMacroVolume.value = sound;
    }
}
