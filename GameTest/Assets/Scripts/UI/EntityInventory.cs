using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInventory : UIInventoryAbstract {

    [HideInInspector]
    public Body currentInventory;

    public override void UpdateUI()
    {
        wipeList();
        if (currentInventory == null) return;
        foreach (Item i in currentInventory.Inventory)
        {
            Debug.Log(i.Name);
            GameObject insert = Instantiate(insertable, transform.GetChild(0));
            insert.GetComponent<Image>().color = i.itemColor;
            insert.GetComponent<UIInteractable>().item = i;
            inventoryUISide.Add(insert);
        }
    }

    public override void removeElement(GameObject g)
    {
        if (currentInventory == null) return;
        var item = g.GetComponent<UIInteractable>().item;
        base.removeElement(g);
        currentInventory.Inventory.Remove(item);
    }

    public override void addElement(GameObject g)
    {
        if (currentInventory == null)
            return;

        var item = g.GetComponent<UIInteractable>().item;
        item.SwitchHolder(currentInventory);
        currentInventory.Inventory.Add(item);
        base.addElement(g);
    }


}
