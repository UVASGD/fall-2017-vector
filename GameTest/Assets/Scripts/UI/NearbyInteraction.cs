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
        base.removeElement(g);
        currentInventory.RemoveItem(item);
    }

    public override void addElement(GameObject g)
    {
        if (currentInventory == null)
        {
            GameObject packObj = Instantiate(itemPackageRes, new Vector2(GameManager.instance.thePlayer.transform.position.x,-2.75f), Quaternion.identity) as GameObject;
            Item itemq = g.GetComponent<UIInteractable>().item;
            ItemPackage pack = packObj.transform.GetChild(0).GetComponent<ItemPackage>();
            pack.CreateItemPackage(new List<Item>(), "Chest", GameManager.instance.thePlayer.transform.position.x);
            pack.AddItem(itemq);
            currentInventory = pack;
            base.addElement(g);
            return;
        }
        var item = g.GetComponent<UIInteractable>().item;
        currentInventory.AddItem(item);
        base.addElement(g);
    }

}
