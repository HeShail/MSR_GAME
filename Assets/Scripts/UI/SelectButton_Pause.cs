using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary> Clase publica que expande la funcionalidad basica de Button para un caso concreto. </summary>
public class SelectButton_Pause : MonoBehaviour, IPointerEnterHandler
{
	public AudioSource buttonPoint;
	public AudioClip sfx_Clip;
	private Button button;

	void Start()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(OnClick);
	}

	
	public void OnPointerEnter(PointerEventData eventData)
	{
		buttonPoint.pitch = 2;
		buttonPoint.PlayOneShot(sfx_Clip);
	}

	private void OnClick()
    {
		buttonPoint.pitch = 1;
	}

}
