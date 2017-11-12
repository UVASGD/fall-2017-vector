using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInteractable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Vector2 offset = new Vector2(32, 32);
    public Vector2 sendoff = new Vector2(-100, -100);

    public static GameObject draggedItem;
    public static GameObject origin;
    private static GameObject hoverhud;
    private  static GameObject bucket;

    

    private bool isDragging = false;
    private bool verbose = false;

    public Item item;

    void Awake()
    {
        draggedItem = null;
        if(hoverhud == null) 
            hoverhud = GameObject.Find("hoverhud");
        Debug.Log(hoverhud);
        if(bucket == null)
            bucket = GameObject.Find("bucket");
        //hoverhud.SetActive(false);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        origin = transform.parent.gameObject;
        Debug.Log(origin.GetInstanceID());
        hoverhud.transform.position = sendoff;
        draggedItem = gameObject;
        draggedItem.GetComponent<Image>().raycastTarget = false;
        transform.SetParent(bucket.transform);
        
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
        }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        draggedItem.GetComponent<Image>().raycastTarget = true;
        draggedItem = null;
        transform.SetParent(origin.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        verbose = false;
        hoverhud.transform.GetChild(0).gameObject.SetActive(true);
        hoverhud.transform.GetChild(1).gameObject.SetActive(false);
        hoverhud.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = item.Name;
        // hoverhud.SetActive(true);
        hoverhud.transform.position = (transform.position + (Vector3)offset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        hoverhud.transform.position = sendoff;
        //hoverhud.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hoverhud.activeInHierarchy) return;
        //0 is Simple, 1 is Verbose.
        if(!verbose)
        {
            verbose = !verbose;

            hoverhud.transform.GetChild(0).gameObject.SetActive(true);
            hoverhud.transform.GetChild(1).gameObject.SetActive(false);
            hoverhud.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = item.Name;
        }
        else
        {
            verbose = !verbose;

            hoverhud.transform.GetChild(1).gameObject.SetActive(true);
            hoverhud.transform.GetChild(0).gameObject.SetActive(false);
            hoverhud.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = item.Name;
            hoverhud.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "waluigi";
        }
    }

}

