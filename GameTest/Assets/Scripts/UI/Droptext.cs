using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Droptext : MonoBehaviour, IDropHandler {

    public static Body playerBody;

    void OnEnable()
    {
        if(playerBody == null)
        {
            playerBody = GameObject.Find("Player").GetComponent<Body>();
        }
    }


    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject moved = UIInteractable.draggedItem;
        if (moved == null) return;
        bool different = UIInteractable.origin.GetInstanceID() != transform.GetChild(0).gameObject.GetInstanceID();
        if (different)
        {
            var from = UIInteractable.origin.GetComponentInParent<UIInventoryAbstract>();
            var to = gameObject.GetComponent<UIInventoryAbstract>();
            Debug.Log(GetGameObjectPath(from.gameObject));
            if(UIInteractable.origin.tag == "EquipmentUI") //Assume that it's coming from inventory.
            {
                var newFrom = UIInteractable.origin.GetComponent<EquipmentDroptest>();
                Debug.Log("Unequipping");
                if (newFrom != null)
                {
                    Item item = moved.GetComponent<UIInteractable>().item;
                    playerBody.addItem(item);
                    if (item.GetType() == typeof(Armor)) {
                        Armor none = new Armor(playerBody, 0, "none");
                        none.EquipTo(playerBody);
                    }
                    else {
                        Fists fist = new Fists(playerBody, 1);
                        fist.EquipTo(playerBody);
                    }
                }
            }

            if (to == null)
            {
                to = gameObject.GetComponentInParent<UIInventoryAbstract>();
            }

            if(to.GetType() == typeof(EntityInventory)) //Taking from entities.
            {
                EntityInventory toE = to as EntityInventory;
                if (toE.currentInventory == null) return;

                //Do entity item checking here.
                from.removeElement(UIInteractable.draggedItem);
                to.addElement(UIInteractable.draggedItem);
                UIInteractable.origin = transform.GetChild(0).gameObject;
            }
            else if(from.GetType() == typeof(EntityInventory)) //Giving to entities.
            {
                from.removeElement(UIInteractable.draggedItem);
                to.addElement(UIInteractable.draggedItem);
                UIInteractable.origin = transform.GetChild(0).gameObject;
            }
            else
            {
                from.removeElement(UIInteractable.draggedItem);
                to.addElement(UIInteractable.draggedItem);
                UIInteractable.origin = transform.GetChild(0).gameObject;
            }

        }
    }
}
