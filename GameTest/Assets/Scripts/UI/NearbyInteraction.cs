using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NearbyInteraction : UIInventoryAbstract{

    [HideInInspector]
    public ItemPackage currentInventory;



    public override void UpdateUI()
    {
        wipeList();
        if (currentInventory == null) return;
        foreach (Item i in currentInventory.Inventory)
        {
            Debug.Log(i.Name);
            GameObject insert = Instantiate(insertable, transform.GetChild(0));
            insert.GetComponent<Image>().color = Random.ColorHSV() + new Color(0.1f, 0.1f, 0.1f, 1);
            insert.GetComponent<UIInteractable>().item = i;
            inventoryUISide.Add(insert);
        }
    }

    public override void removeElement(GameObject g)
    {
        var item = g.GetComponent<UIInteractable>().item;
        currentInventory.RemoveItem(item);
        base.removeElement(g);
    }

    public override void addElement(GameObject g)
    {
        if (currentInventory == null)
        {
            Destroy(g);
            return;
        }
        var item = g.GetComponent<UIInteractable>().item;
        currentInventory.AddItem(item);
        base.addElement(g);
    }

}
