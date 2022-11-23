using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase que almacena y ejecuta todas las llamadas de sonido del sistema. </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager {get; private set;}

    [SerializeField] private bool game_muted;

    [Header("Audio Sources")]
    public AudioSource environmentAudioSource;
    public AudioSource playerAudioSource;
    public AudioSource trackAudioSource;
    public AudioSource sfxAudioSource;
    private float environmentVolume;
    private float playerVolume;
    private float trackVolume;
    private float sfxVolume;
    private float masterVolume;
    private float soundVolumeII;

    [Header("SOUNDTRACKS")]
    public AudioClip mainSoundtrack;

    [Header("SFX")]
    public AudioClip fencePush;
    public AudioClip fenceTouch;
    public AudioClip electrocute;
    public AudioClip drum;
    public AudioClip advisor_call;
    public AudioClip heater_fall;
    public AudioClip explosion;
    public AudioClip invalidSelection;

    [Header ("Clips MUERTE")]
    public AudioClip banishment;

    [Header ("Cronometro")]
    public AudioClip [] crono;

    [Header ("Exito")]
    public AudioClip paperSound;
    public AudioClip paperSound2;

    #region LOADING VOLUMES

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (audioManager != null && audioManager != this)
        {
            Destroy(this);
        }
        else
        {
            audioManager = this;
        }
    }

    private void Start()
    {
        LoadPlayerPrefSounds();
        
    }

    /// <summary>Función encargada de extraer la selección de volúmenes del jugador. 
    /// Si no existiera tal configuración, se ajustaría al valor inicial (máximo) </summary>
    public void LoadPlayerPrefSounds()
    {
        if (!PlayerPrefs.HasKey("environmentVolume")) PlayerPrefs.SetFloat("environmentVolume", 1f);
        if (!PlayerPrefs.HasKey("masterVolume")) PlayerPrefs.SetFloat("masterVolume", 1f);
        if (!PlayerPrefs.HasKey("playerVolume")) PlayerPrefs.SetFloat("playerVolume", 1f);
        if (!PlayerPrefs.HasKey("trackVolume")) PlayerPrefs.SetFloat("trackVolume", 1f);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 1f);
        if (!PlayerPrefs.HasKey("soundVolumeII")) PlayerPrefs.SetFloat("soundVolumeII", 1f);
        if (!PlayerPrefs.HasKey("gameMuted")) PlayerPrefs.SetInt("gameMuted", 0);

        VolumeLoad();
        SetAudioSourcesVolume();
    }


    /// <summary>Funcion publica que carga los ajustes de sonido de la configuración anterior. </summary>
    public void VolumeLoad()
    {

        environmentVolume = PlayerPrefs.GetFloat("soundVolumeII");
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        playerVolume = PlayerPrefs.GetFloat("soundVolumeII");
        trackVolume = PlayerPrefs.GetFloat("trackVolume");
        sfxVolume = PlayerPrefs.GetFloat("soundVolumeII");
        soundVolumeII = PlayerPrefs.GetFloat("soundVolumeII");

        if (PlayerPrefs.GetInt("gameMuted") != 0) game_muted = true; else game_muted = false;
        SetSlidersValues();
    }

    /// <summary> Metodo que inicializa el valor de los sliders. </summary>
    public void SetSlidersValues()
    {
        //OLD EXTENDED VOLUME
        //GetComponent<VolumeSliders>().SetVolumeValues(masterVolume, trackVolume, environmentVolume, sfxVolume, game_muted);
        
        GetComponent<VolumeSliders>().SetVolumeValuesII(trackVolume, soundVolumeII);

    }

    /// <summary> Funcion publica que asigna a los Audio Sources los valores de volumen almacenados. </summary>
    public void SetAudioSourcesVolume()
    {
        AudioListener.volume = masterVolume;
        environmentAudioSource.volume = environmentVolume;
        playerAudioSource.volume = playerVolume;
        trackAudioSource.volume = trackVolume;
        sfxAudioSource.volume = sfxVolume;


        trackAudioSource.mute = game_muted;
        playerAudioSource.mute = game_muted;
        sfxAudioSource.mute = game_muted;
        environmentAudioSource.mute = game_muted;
    }


    /// <summary> Funcion que almacena los ajustes de sonido actuales a la base de datos de PlayerPrefs. </summary>
    public void SaveSoundSettings()
    {
        #region OLD ONE
        /*int mute_value;
        if (game_muted) mute_value = 1; else mute_value = 0;
        PlayerPrefs.SetFloat("environmentVolume", environmentVolume);
        PlayerPrefs.SetFloat("playerVolume", playerVolume);
        PlayerPrefs.SetFloat("trackVolume", trackVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetInt("gameMuted", mute_value);
        */
        #endregion

        PlayerPrefs.SetFloat("soundVolumeII", soundVolumeII);
        PlayerPrefs.SetFloat("environmentVolume", environmentVolume);
        PlayerPrefs.SetFloat("playerVolume", playerVolume);
        PlayerPrefs.SetFloat("trackVolume", trackVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
    }

    /// <summary> Funcion publica que activa y desactiva el silencio en el juego. </summary>
    public void MuteGame()
    {
        game_muted = !game_muted;
        trackAudioSource.mute = game_muted;
        playerAudioSource.mute = game_muted;
        sfxAudioSource.mute = game_muted;
        environmentAudioSource.mute = game_muted;
    }

    public bool IsGameMute()
    {
        return game_muted;
    }

    /// <summary> Metodo que inicializa el valor de sonidos de la variable que manipula multiples audio sources. </summary>
    public void SetSOUNDVolume(float value)
    {
        soundVolumeII = value;
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;

    }

    public void SetEnvironmentVolume(float value)
    {
        environmentVolume = value;
    }

    public void SetPlayerVolume(float value)
    {
        playerVolume = value;
    }

    public void SetTrackVolume(float value)
    {
        trackVolume = value;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;

    }

    #endregion


    public void LoadSoundtrack()
    {
        trackAudioSource.PlayOneShot(mainSoundtrack);
        trackAudioSource.loop = true;
    }

    public static void Fence(bool state)
    {
        audioManager.FenceSounds(state);
    }

    private void FenceSounds(bool state)
    {
        if (state) environmentAudioSource.PlayOneShot(fenceTouch); else environmentAudioSource.PlayOneShot(fencePush);


    }
    public void Drum()
    {
        environmentAudioSource.PlayOneShot(drum);

    }
    public void Pum()
    {
        environmentAudioSource.PlayOneShot(explosion);
    }

    /// <summary> Funcion publica que ejecuta el sonido de la abduccion y reinicia el soundtrack tras hacer respawn. </summary>
    public void Abduction()
    {
        trackAudioSource.loop = false;
        trackAudioSource.Stop();
        trackAudioSource.PlayOneShot(banishment);
        StartCoroutine("Soundtrack_CRT");
    }

    public void InvalidSelection()
    {
        sfxAudioSource.PlayOneShot(invalidSelection);

    }

    public void Electrocute()
    {
        environmentAudioSource.PlayOneShot(electrocute);
    }
    public void Cronometer(int index)
    {
        sfxAudioSource.PlayOneShot(crono[index]);
    }
    public void StopCrono()
    {
        sfxAudioSource.Stop();
    }

    public void AdvisorCall()
    {
        sfxAudioSource.PlayOneShot(advisor_call);
    }
    public void Heater()
    {
        sfxAudioSource.PlayOneShot(heater_fall);
    }

    public void Success()
    {
        trackAudioSource.loop = false;
        trackAudioSource.PlayOneShot(paperSound);
    }
     public void TotalSuccess()
    {
        trackAudioSource.loop = false;
        trackAudioSource.PlayOneShot(paperSound2);
        StartCoroutine("Soundtrack_CRT");
    }
    IEnumerator Soundtrack_CRT()
    {

        yield return new WaitForSeconds(4f);
        trackAudioSource.PlayOneShot(mainSoundtrack);
        trackAudioSource.loop = true;
    }
    
}
