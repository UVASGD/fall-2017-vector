using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentDroptest : MonoBehaviour, IDropHandler {

    public static Body playerBody;
    public int type;
    //0 = Weapon
    //1 = Armor
    //2 = Other

    void Start()
    {
        if (playerBody == null)
        {
            playerBody = GameObject.Find("Player").GetComponent<Body>();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject moved = UIInteractable.draggedItem;
        if (moved == null) return;
        var from = UIInteractable.origin.GetComponentInParent<UIInventoryAbstract>();
        if (from.GetType() != typeof(InventoryInteraction)) return;
        var fromObject = UIInteractable.origin;
        var to = gameObject;

        // Debug.Log("Equipping: " + moved.GetComponent<UIInteractable>().item.Name);
        Item checkedItem;
        switch (type)
        {
            case 0:
                checkedItem = moved.GetComponent<UIInteractable>().item;
                if(checkedItem.Function == "Weapon")
                {
                    if (playerBody.Weapon.GetType() != typeof(Fists)) //Needs to check for proper default. Not null. 
                    {
                        playerBody.addItem(playerBody.Weapon);
                    }
                    from.removeElement(moved);
                    checkedItem.EquipTo(playerBody);
                    Debug.Log("Is the item in moved null? " + moved.GetComponent<UIInteractable>().item.ToString());

                    UIInteractable.origin = transform.gameObject;
                }
                break;
            case 1:
                checkedItem = moved.GetComponent<UIInteractable>().item;
                if (checkedItem.Function == "Armor")
                {
                    if (playerBody.Armor.Name != "none") //Needs to check for proper default. Not null. 
                    {
                        playerBody.addItem(playerBody.Armor);
                    }
                    from.removeElement(moved);
                    checkedItem.EquipTo(playerBody);
                    // Debug.Log("Is the item in moved null? " + moved.GetComponent<UIInteractable>().item.ToString());

                    UIInteractable.origin = transform.gameObject;
                }
                break;
            case 2:
                Debug.Log("The memes, Jack");
                break;

        }
    }

}
