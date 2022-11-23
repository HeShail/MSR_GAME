using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Clase publica que testea la operatividad de los controles del juego. </summary>
public class ControlsTest : MonoBehaviour
{
    public Image esc_key;
    public Image shift_key;
    public Image spacebar_key;
    public Image mevement_keys;
    public Image interaction_key;
    public Image control_key;
    public Image map_key;
    public Color default_color;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) esc_key.color = Color.green; else esc_key.color = default_color;
        if (Input.GetKey(KeyCode.LeftShift)) shift_key.color = Color.green; else shift_key.color = default_color;
        if (Input.GetKey(KeyCode.Space)) spacebar_key.color = Color.green; else spacebar_key.color = default_color;
        if (Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W)) mevement_keys.color = Color.green; else mevement_keys.color = default_color;
        if (Input.GetKey(KeyCode.E)) interaction_key.color = Color.green; else interaction_key.color = default_color;
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse X") != 0) control_key.color = Color.green; else control_key.color = default_color;
        if(Input.GetKey(KeyCode.M)) map_key.color = Color.green; else map_key.color = default_color;
    }
}
