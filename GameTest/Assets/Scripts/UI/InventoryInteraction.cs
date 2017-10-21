using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryInteraction : MonoBehaviour, IDropHandler {

    private static GameObject insertable;

    private List<GameObject> inventoryUISide;
    private Item weapon;

    private Body Player;

    void Awake()
    {
        insertable = Resources.Load("ItemSlot") as GameObject;
        inventoryUISide = new List<GameObject>(); 
    }
    void Start()
    {
        Player = GameObject.Find("Player").GetComponent<Body>();
        Debug.Log(Player);
    }

    public void UpdateUI()
    {
        wipeList();
        Debug.Log(">>> Inventory Size " + Player.Inventory.Count + " <<< ");
        foreach (Item i in Player.Inventory)
        {
            Debug.Log(i.Name);
            GameObject insert = Instantiate(insertable, transform.GetChild(0).GetChild(0));
            insert.GetComponent<Image>().color = Random.ColorHSV() + Color.black;
            insert.GetComponent<UIInteractable>().item = i;
            inventoryUISide.Add(insert);
        }
    }

    public void wipeList()
    {
        foreach(GameObject g in inventoryUISide)
            Destroy(g);
        inventoryUISide.Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Put in Inventory");
    }
}
