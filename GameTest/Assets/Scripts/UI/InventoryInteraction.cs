using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InventoryInteraction : UIInventoryAbstract{

    private Item weapon;

    public override void UpdateUI()
    {
        wipeList();
        foreach (Item i in Player.Inventory)
        {
            Debug.Log(i.Name);
            GameObject insert = Instantiate(insertable, transform.GetChild(0).GetChild(0));
            insert.GetComponent<Image>().color = Random.ColorHSV() + new Color(0.1f, 0.1f, 0.1f, 1);
            insert.GetComponent<UIInteractable>().item = i;
            inventoryUISide.Add(insert);
        }
    }

    public override void removeElement(GameObject g)
    {
        Player.Inventory.Remove(g.GetComponent<UIInteractable>().item);
        base.removeElement(g);
    }

    public override void addElement(GameObject g)
    {
        Player.Inventory.Add(g.GetComponent<UIInteractable>().item);
        g.GetComponent<UIInteractable>().item.SwitchHolder(Player);
        base.addElement(g);
    }

}
