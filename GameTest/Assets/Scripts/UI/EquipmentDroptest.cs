using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentDroptest : MonoBehaviour, IDropHandler {

    public static Body playerBody;

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
        var from = UIInteractable.origin.GetComponentInParent<UIInventoryAbstract>();
        var fromObject = UIInteractable.origin;
        var to = gameObject;

        Debug.Log("Equipping: " + moved.GetComponent<UIInteractable>().item.Name);
        from.removeElement(moved);

        if (playerBody.Weapon.GetType() != typeof(Fists)) //Needs to check for proper default. Not null. 
        {
            playerBody.addItem(playerBody.Weapon);
        }
        Item i = moved.GetComponent<UIInteractable>().item;
        // playerBody.Weapon = i;
        i.EquipTo(playerBody);
        Debug.Log("Is the item in moved null? " + moved.GetComponent<UIInteractable>().item.ToString());

        UIInteractable.origin = transform.gameObject;
    }

}
