using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Clase publica que elabora un sistema para organizar los objetos del juego. </summary>
public class ItemSystem : MonoBehaviour
{
    private List<ItemStruct> inventory;
    public List<GameObject> clavesAnexo;
    public List<GameObject> clavesPanelAnexo;
    public List<Material> hoja_personajeMat;

    /// <summary> Struct que alberga un constructor con tres parámetros:
    /// Identificador [entero], nombre[string] y tipo de objeto[entero] </summary>
    public struct ItemStruct
    {
        public int _id;
        public string _name;
        public int itemType; 

        public ItemStruct(int number, string itemName, int type)
        {
            _id = number;
            _name = itemName;
            itemType = type;  // 0 para papelotes de s�mbolos
        }
    }
    private void Start()
    {
        inventory = new List<ItemStruct>();
        hojaPersonaje.SetActive(false);
    }

    /// <summary> Función que añade un objeto al inventario. </summary>
    public void AddItem(int id, string nombre, int type)
    {
        inventory.Add(new ItemStruct(id, nombre, type));
        if (type == 0) AddPaperKey(id);
    }

    [Tooltip ("Textura del papel recogido")]
    private Sprite tempTexture;

    /// <summary> Metodo que añade los papeles clave al inventario del jugador. </summary>
    /// <param name="id"> Identificador entero del papel encontrado. </param>
    private void AddPaperKey(int id)
    {
        clavesAnexo[id].GetComponent<Image>().color = Color.white;
        clavesPanelAnexo[id].GetComponent<Image>().color = Color.white;
        hojaPersonaje.GetComponent<Renderer>().material = hoja_personajeMat[id];
        switch (id)
        {
            case 0:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 1:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 2:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 3:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 4:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 5:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;

            case 6:
                clavesAnexo[id].GetComponent<Image>().sprite = tempTexture;
                clavesPanelAnexo[id].GetComponent<Image>().sprite = tempTexture;
                break;
        }
        Invoke("PlayerPaper", 1f);
    }

    /// <summary> Metodo que añade los papeles clave al inventario del jugador. </summary>
    private void PlayerPaper()
    {
        StartCoroutine("ShowPaper");
    }

    [Tooltip("Papel que porta la protagonista en la mano")]
    public GameObject hojaPersonaje;

    /// <summary> Funcion que renderiza el papel del personaje </summary>
    IEnumerator ShowPaper()
    {
        hojaPersonaje.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        hojaPersonaje.SetActive(false);

    }

    /// <summary> Metodo que muestra todos los objetos del inventario. </summary>
    public void SpitItems()
    {
        if (inventory!= null)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                Debug.Log("Item ID: " + inventory[i]._id + ". Descripci�n: " + inventory[i]._name + ". tipo de objeto:" + inventory[i].itemType);
            }
        }
        
    }

    /// <summary> Funcion que devuelve un booleano indicando si existe un objeto en el inventario. </summary>
    /// <param name="index"> Identificador entero del objeto/item en cuestión. </param>
    /// <param name="type"> Identificador entero del tipo de objeto. </param>
    public bool ContainsItem(int index, int type)
    {
        if (inventory != null)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i]._id == index && inventory[i].itemType==type) return true;
            }
        }
        return false;
    }

    /// <summary> Procedimiento que dibuja una textura en la variable temporal tempTexture </summary>
    public void SetTexture(Sprite image)
    {
        tempTexture = image;
    }

    /// <summary> Funcion publica que devuelve el inventario. </summary>
    public List<ItemStruct> GetInventory()
    {
        return inventory;
    }
}
