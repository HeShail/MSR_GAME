using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase publica que monitorea la direccion del norte. </summary>
public class PlayerCompass : MonoBehaviour
{

    public Vector3 northDirection;
    public RectTransform northLayer;

    private void Update()
    {
        ChangeNorthDirection();
    }

    /// <summary> Metodo oculto que actualiza la direccion del norte. </summary>
    private void ChangeNorthDirection()
    {
        northDirection.z = transform.eulerAngles.y;
        northLayer.localEulerAngles = northDirection;
    }
}
