using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary> Clase publica que expande la funcionalidad basica de Button. </summary>
public class SelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public AudioSource buttonPoint;
	private Button button;

	void Start()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(OnClick);
	}

	/// <summary> Funcion publica que ejecuta un sonido al posicionar el curso sobre el boton. </summary>
	/// <remarks> Activa los elementos anidados del boton.</remarks>
	public void OnPointerEnter(PointerEventData eventData)
	{
		buttonPoint.pitch = 2;
		buttonPoint.Play();
		if (transform.childCount > 1) this.transform.GetChild(1).gameObject.SetActive(true);
		if (transform.childCount > 2) this.transform.GetChild(2).gameObject.SetActive(true);
	}

	/// <summary> Funcion publica que ejecuta un sonido al desplazar el cursor fuera del boton. </summary>
	/// <remarks> Desactiva los elementos anidados del boton.</remarks>
	public void OnPointerExit(PointerEventData eventData2)
	{
		if (transform.childCount > 1) this.transform.GetChild(1).gameObject.SetActive(false);
		if (transform.childCount > 2) this.transform.GetChild(2).gameObject.SetActive(false);

	}

	/// <summary> Funcion publica que ejecuta un sonido al presionar un boton. </summary>
	/// <remarks> Desactiva los elementos anidados del boton.</remarks>
	private void OnClick()
    {
		buttonPoint.pitch = 1;
		if (transform.childCount > 1) this.transform.GetChild(1).gameObject.SetActive(false);
		if (transform.childCount > 2) this.transform.GetChild(2).gameObject.SetActive(false);
	}

	/// <summary> Metodo oculto que libera la seleccion actual. </summary>
	private void DeSelectButton()
    {
		if (TryGetComponent(out Animator anim))
		{

			anim.SetTrigger("Normal");
		}
	}
}
