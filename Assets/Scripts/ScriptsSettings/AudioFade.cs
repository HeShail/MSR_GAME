using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase en desuso que silencia progresivamente el volumen de la musica del juego. </summary>
public class AudioFade : MonoBehaviour
{
    public AudioSource soundtrack;
    public float finalVolume;
    public float tempTrans;

    public void FadeST() 
    {
        StartCoroutine("StartFade");
    }
    public IEnumerator StartFade()
    {
        float currentTime = 0;
        float start = soundtrack.volume;
        while (currentTime < tempTrans)
        {
            currentTime += Time.deltaTime;
            soundtrack.volume = Mathf.Lerp(start, finalVolume, currentTime / tempTrans);
            yield return null;
        }
        yield break;
    }
}
