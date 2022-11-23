using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Clase encargada de fijar el elemento de la UI del minimapa a su marco en caso de distanciarse en exceso del jugador. </summary>
public class MinimapClamp : MonoBehaviour
{
    [SerializeField] public Transform minimapCam;
    private const float  MINIMAP_SIZE = 18;
    private Vector3 tempV3;
    public bool isNorth;

    // Update is called once per frame
    void Update()
    {
        tempV3 = transform.parent.transform.position;
        tempV3.y = transform.position.y;
        transform.position = tempV3;
    }

    private void LateUpdate()
    {
        if (isNorth)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minimapCam.position.x - MINIMAP_SIZE +6, MINIMAP_SIZE + minimapCam.position.x), transform.position.y,
            Mathf.Clamp(transform.position.z, minimapCam.position.z - MINIMAP_SIZE, MINIMAP_SIZE + minimapCam.position.z));
        
        }
        else
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minimapCam.position.x - MINIMAP_SIZE, MINIMAP_SIZE + minimapCam.position.x), transform.position.y,
            Mathf.Clamp(transform.position.z, minimapCam.position.z - MINIMAP_SIZE, MINIMAP_SIZE + minimapCam.position.z));
        }
    }
}
