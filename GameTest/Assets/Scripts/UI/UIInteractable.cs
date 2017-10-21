using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInteractable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Vector2 offset = new Vector2(32, 32);
    public Vector2 sendoff = new Vector2(-100, -100);

    private static GameObject draggedItem;
    private static GameObject hoverhud;
    private GameObject bucket;
    private GameObject origin;

    private bool isDragging = false;
    private bool verbose = false;

    public Item item;

    void Awake()
    {
        draggedItem = null;
        hoverhud = GameObject.Find("hoverhud");
        Debug.Log(hoverhud);
        bucket = GameObject.Find("bucket");
        origin = transform.parent.gameObject;
        //hoverhud.SetActive(false);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        hoverhud.transform.position = sendoff;
        draggedItem = gameObject;
        Debug.Log("Pop off");
        transform.SetParent(bucket.transform);
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition;
        }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        draggedItem = null;
        Debug.Log("Revert");
        transform.SetParent(origin.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return;
        verbose = false;
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
        }
        else
        {
            verbose = !verbose;
            hoverhud.transform.GetChild(1).gameObject.SetActive(true);
            hoverhud.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

}

