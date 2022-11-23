using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary> Clase encargada de gestionar los reproductores de video y cinematicas </summary>
/// <param name="playing"> Booleano que comprueba si se esta reproduciendo o no un video </param>
public class CinematicPlayer : MonoBehaviour
{
    public GameObject player;
    public AdviserManager advisor;
    public GameObject skipText;

    public bool playing = true;

    [Header("Videos")]
    [Tooltip ("Reproductores de video de la escena de juego, 0:Cinematicas y 1:Menu_Inteligencia")]
     public VideoPlayer [] video;
    [Tooltip ("Lista de videos de los desafios del juego")]
    public VideoClip[] videos;
    [Tooltip ("Contador encargado de gestionar que video debe reproducirse segun el desafio")]
    public int videoCount = 0;

    [Header("VideosExtra")]
    [Tooltip ("Reproductor de video del menu inicial")]
    public VideoPlayer InitialVideo;
    [Tooltip ("Reproductor de video de logo inicial")]
    public VideoPlayer IntroVideo;
    [Tooltip ("¿Es el menu de inicio?")]
    public bool isInitialMenu;
    [Tooltip ("¿Es la pantalla inicial del logo?")]
    public bool isLogoVideo;

    [Header("Iconos de Videos")]
    [Tooltip ("Iconos de los videos del menu de inteligencia")]
    public GameObject[] PauseMenuVideos;

    [Header("Galeria de Videos")]
    [Tooltip ("Videos del menu de inteligencia")]
    public VideoClip[] GalleryVideos;
    [Tooltip ("Video al resolver la contraseña de la entrada al anexo")]
    public VideoClip FinalVideo;
    

    private void Awake() 
    {
        if (isLogoVideo)
        {
            Debug.Log("intro");
            IntroVideo.loopPointReached += CheckOver;
        }
        else if(!isInitialMenu)
        {
            video[0].GetComponent<RawImage>().enabled = false;
            video[1].GetComponent<RawImage>().enabled = false;
            PauseMenuVideos[videoCount].gameObject.SetActive(true);
            
            video[0].loopPointReached += CheckOver;
            video[1].loopPointReached += CheckOver;
            videoCount++;
            video[0].clip = videos[videoCount];
        }
        else
        {
            InitialVideo.GetComponent<CinematicPlayer>().enabled = true;
            InitialVideo.loopPointReached += CheckOver;
            GameObject.Find("Cinematic").GetComponent<RawImage>().enabled = false;
        } 
    }

    // Start is called before the first frame update
    private void Start()
    {
        AudioManager.audioManager.LoadSoundtrack();
    }

    private void Update() 
    {
       if((playing == true) && (isInitialMenu))
       {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SkipVideo();
        }
       }
    }
    
    /// <summary> Funcion encargada de comprobar cuando acaba la reproduccion de un video</summary>
    public void CheckOver(VideoPlayer vp)
    {
        if(isInitialMenu) GameManager.gm.ChangeScene(2);
        else if(isLogoVideo) GameManager.gm.ChangeScene(1);
        else
        {
            Time.timeScale = 1;
            player.GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
            video[0].Stop();
            video[0].clip = null;
            video[1].clip = null;
            Invoke("Playing",1f);
            AudioManager.audioManager.LoadSoundtrack();
            if(isInitialMenu) GameManager.gm.ChangeScene(1);
            if(videoCount == 1) 
            {
                advisor.LaunchAdvisor(0);
                skipText.SetActive(false);
            } 
            else if (videoCount == 2) advisor.LaunchAdvisor(1);
            if(videoCount == 9) video[0].clip = FinalVideo;
            else if (videoCount < videos.Length) 
            {
                video[0].clip = videos[videoCount];
                video[0].Prepare();
            }
            video[0].GetComponent<RawImage>().enabled = false;
            video[1].GetComponent<RawImage>().enabled = false;
        }
    }

    /// <summary> Funcion encargada de reproducir los videos de cinematicas</summary>
    public void PlayCinematic()
    {  
        AudioManager.audioManager.trackAudioSource.Stop();
        if (videoCount < PauseMenuVideos.Length) 
        {
            PauseMenuVideos[videoCount].gameObject.SetActive(true);
        }    
      
        video[0].GetComponent<RawImage>().enabled = true;
        video[0].Play();
        
        playing = true; 
        videoCount++;
        Invoke("TimeStop",1.7f);
    }

    /// <summary> Funcion que pausa el juego durante la reproduccionde un video</summary>
    public void TimeStop()
    {
        Time.timeScale = 0;
    }

    /// <summary> Funcion que reproduce la cienmtica inicial al pulsar el boton de jugar </summary>
    public void PlayInitialCinematic()
    {
        AudioManager.audioManager.trackAudioSource.Stop();
        GameObject.Find("Cinematic").GetComponent<RawImage>().enabled = true;
        InitialVideo.Play();
        
    }

    /// <summary> Funcion que gestiona la opcion de saltar la cinematica </summary>
    public void SkipVideo()
    {   
        if(isLogoVideo) GameManager.gm.ChangeScene(1);
        if(isInitialMenu) GameManager.gm.ChangeScene(2);
        else
        {
            Time.timeScale = 1;
            video[0].GetComponent<RawImage>().enabled = false;
            video[0].Stop();
            video[1].GetComponent<RawImage>().enabled = false;    
            player.GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
            video[0].clip = null;
            Invoke("Playing",1f);
            AudioManager.audioManager.LoadSoundtrack();
        
            if(videoCount == 1) 
            {
                advisor.LaunchAdvisor(0); 
                skipText.SetActive(false);
            }
            else if (videoCount == 2) advisor.LaunchAdvisor(1);
            if(videoCount == 9) video[0].clip = FinalVideo;
            else if (videoCount < videos.Length) 
            {
                video[0].clip = videos[videoCount];
                video[0].Prepare();
            }
        }    
    }

    /// <summary> Funcion activa la reproduccion de los video desde el menu de inteligencia </summary>
    public void Replay(int videoSelect)
    {  
        AudioManager.audioManager.trackAudioSource.Stop(); 
        video[1].clip = GalleryVideos[videoSelect];
        video[1].GetComponent<RawImage>().enabled = true;
        video[1].Play();
        playing = true;
    }

    /// <summary> Funcion que avisa que ha acabado un video </summary>
    public void Playing()
    {
        playing = false;  
    }   
}
