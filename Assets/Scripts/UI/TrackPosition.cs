using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase publica que monitorea la posicion del objetivo actual. </summary>
public class TrackPosition : MonoBehaviour
{
    public Transform parent;

    private void Update()
    {
        transform.position = new Vector3(parent.transform.position.x, transform.position.y, parent.transform.position.z);
    }
}
