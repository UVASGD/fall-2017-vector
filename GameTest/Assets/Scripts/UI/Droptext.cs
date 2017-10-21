using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Droptext : MonoBehaviour, IDropHandler {

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


    void LateUpdate()
    {
        Debug.Log(gameObject.transform.GetChild(0).gameObject.GetInstanceID());
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject moved = UIInteractable.draggedItem;
        
        if (UIInteractable.origin.GetInstanceID() != transform.GetChild(0).gameObject.GetInstanceID())
        {

            var from = UIInteractable.origin.GetComponentInParent<UIInventoryAbstract>();
            var to = gameObject.GetComponent<UIInventoryAbstract>();

            if(to == null)
            {
                to = gameObject.GetComponentInParent<UIInventoryAbstract>();
            }
            from.removeElement(UIInteractable.draggedItem);
            to.addElement(UIInteractable.draggedItem);
            UIInteractable.origin = transform.GetChild(0).gameObject;
            
        }
    }
}
