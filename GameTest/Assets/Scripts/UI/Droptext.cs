﻿using System.Collections;
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
                    playerBody.addItem(moved.GetComponent<UIInteractable>().item);
                    Fists fist = new Fists(playerBody, 1);
                    fist.EquipTo(playerBody);
                }
            }

            if (to == null)
            {
                to = gameObject.GetComponentInParent<UIInventoryAbstract>();
            }

            from.removeElement(UIInteractable.draggedItem);
            to.addElement(UIInteractable.draggedItem);
            UIInteractable.origin = transform.GetChild(0).gameObject;


        }
    }
}
