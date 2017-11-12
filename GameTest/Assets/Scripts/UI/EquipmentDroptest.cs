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
            Debug.Log(GameObject.Find("Player"));
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

        if (playerBody.Weapon != null) //Needs to check for proper default. Not null. 
        {
            playerBody.addItem(playerBody.Weapon);
            Debug.Log(UIInteractable.draggedItem);
        }
        playerBody.Weapon = moved.GetComponent<UIInteractable>().item;
        Debug.Log(moved);

        Debug.Log(UIInteractable.draggedItem);
        UIInteractable.origin = transform.gameObject;
    }

}
