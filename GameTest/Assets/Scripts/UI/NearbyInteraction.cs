using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NearbyInteraction : UIInventoryAbstract{

    [HideInInspector]
    public ItemPackage currentInventory;

    private static GameObject itemPackageRes;

    void OnEnable()
    {
        if (itemPackageRes == null)
            itemPackageRes = Resources.Load("ItemPackage") as GameObject;
    }

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
            GameObject packObj = Instantiate(itemPackageRes, new Vector2(GameManager.instance.thePlayer.transform.position.x,-2.5f), Quaternion.identity) as GameObject;
            Item itemq = g.GetComponent<UIInteractable>().item;
            Debug.Log("Is itemq null?: " + itemq.ToString());
            Debug.Log("What about player instance stuff?: " + GameManager.instance.thePlayer);
            ItemPackage pack = packObj.GetComponent<ItemPackage>();
            Debug.Log("What about pack?: " + pack.ToString());
            pack.CreateItemPackage(new List<Item>() { itemq }, "Chest", GameManager.instance.thePlayer.transform.position.x);
            base.addElement(g);
            return;
        }
        var item = g.GetComponent<UIInteractable>().item;
        currentInventory.AddItem(item);
        base.addElement(g);
    }

}
