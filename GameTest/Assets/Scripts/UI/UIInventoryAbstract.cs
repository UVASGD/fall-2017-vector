using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryAbstract : MonoBehaviour{

    protected static GameObject insertable;

    protected List<GameObject> inventoryUISide;

    protected static Body Player;

    void Awake()
    {
        if(insertable == null) 
            insertable = Resources.Load("ItemSlot") as GameObject;
        inventoryUISide = new List<GameObject>();
    }
    void Start()
    {
        if(Player == null) 
            Player = GameObject.Find("Player").GetComponent<Body>();
        Debug.Log(Player);
    }

    public virtual void UpdateUI()
    {
        wipeList();
        foreach (Item i in Player.Inventory)
        {
            Debug.Log(i.Name);
            GameObject insert = Instantiate(insertable, transform);
            insert.GetComponent<Image>().color = Random.ColorHSV() + new Color(0.5f,0.5f,0.5f,1);
            insert.GetComponent<UIInteractable>().item = i;
            inventoryUISide.Add(insert);
        }
    }

    public virtual void removeElement(GameObject g)
    {
        //inventoryUISide.Find(x => x.GetInstanceID() == g.GetInstanceID());
        inventoryUISide.Remove(g);
    }

    public virtual void addElement(GameObject g)
    {
        inventoryUISide.Add(g);
    }

    public void testPrint()
    {
        int x = 0;
        foreach (GameObject g in inventoryUISide)
        {
            Debug.Log(g + " " + x);
            x++;
        }
    }

    public void wipeList()
    {
        foreach (GameObject g in inventoryUISide)
            Destroy(g);
        inventoryUISide.Clear();
    }

}
