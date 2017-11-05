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
        bool different;
        if (transform.childCount > 0) { different = UIInteractable.origin.GetInstanceID() != transform.GetChild(0).gameObject.GetInstanceID(); }
        else { different = UIInteractable.origin.GetInstanceID() != gameObject.GetInstanceID(); }

        if (different)
        {

            var from = UIInteractable.origin.GetComponentInParent<UIInventoryAbstract>();
            var to = gameObject.GetComponent<UIInventoryAbstract>();
            bool checkEquip = (UIInteractable.origin.tag == "EquipmentUI" || gameObject.tag == "EquipmentUI");

            if (to == null)
            {
                to = gameObject.GetComponentInParent<UIInventoryAbstract>();
            }
            if (checkEquip)
            {
                var newto = gameObject;
                var newfrom = UIInteractable.origin;
                if (newto.tag == "EquipmentUI") //Equipping
                {
                    Debug.Log("Equip " + moved.GetComponent<UIInteractable>().item);
                    from.removeElement(moved);
                    if (playerBody.Weapon != null)
                    {
                        playerBody.addItem(playerBody.Weapon);
                        from.UpdateUI();
                    }
                    playerBody.Weapon = moved.GetComponent<UIInteractable>().item;
                    Debug.Log(moved);

                }
                else if (newfrom.tag == "EquipmentUI") //Unequipping
                {
                    playerBody.addItem(playerBody.Weapon);
                    playerBody.Weapon = null; //TODO: Change
                    Debug.Log("Unequip");
                }


                UIInteractable.origin = transform.gameObject;
                return;
            }


            from.removeElement(UIInteractable.draggedItem);
            to.addElement(UIInteractable.draggedItem);
            UIInteractable.origin = transform.GetChild(0).gameObject;


        }
    }
}
