﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPackage : Body, Interactable {

    public List<Item> Inventory { get { return inventory; } }

    string renderName;
    float loc = 0f;
    GameObject bodyRenderObject;
    Renderer rend;
    SpriteOutline outline;
    SpriteRenderer bodyRender;

    int sortingLayer = 0;

    bool interacting;
    Body interactor;

	// Use this for initialization
	void Start () {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRenderTransform = child; }
        //Set bodyRender equal to the transform of the proper childObject

        harmQuant = 0f;
        harmThreshold = 1f;

        hinderQuant = 0f;
        hinderThreshold = 1f;

        currMoveAct = new HaltAction("Halt", 0, this);
        currAct = new Action("Open", 0, this);
        AddAffecter(new ResistanceAggregate(this, 0f));

        mind = new Inanimate(null, this);
        interacting = false;
        outline = GetComponent<SpriteOutline>();
        inventory = new List<Item> { new Sword(this, 1) };
        CreateItemPackage(inventory, "RChest", 5.5f);
    }

	// Update is called once per frame
	void Update () {
        //mind.Update();
        if (time.clock)
            Tick();
	}

    public void CreateItemPackage(List<Item> _inventory, string _renderName, float _loc, int size = 1) {
        inventory = _inventory;
        renderName = _renderName;
        loc = _loc;

        SetRender();
    }

    void SetRender() {
        //bodyRenderObject = (GameObject)Instantiate(Resources.Load(renderName), new Vector3(0,0,0), Quaternion.identity);
        //bodyRender = bodyRenderObject.GetComponent<SpriteRenderer>();

        bodyRender = GetComponent<SpriteRenderer>();
        bodyRenderObject = bodyRender.gameObject;

        bodyRenderTransform = bodyRenderObject.transform;

        bodyRender.sortingLayerName = size.ToString();
        rend = bodyRenderObject.GetComponent<Renderer>();
        
    }

    public bool CheckInteract(Body player) {
        if (Mathf.Abs(transform.position.x - player.Position.x) <= 3) {
            interacting = true;
            interactor = player;
        }
        else {
            interacting = false;
            interactor = null;
        }
        return interacting;
    }

    public void OnMouseOver() {
        outline.OnEnable();
        /*
        if (interacting) {
            rend.material.shader = highlighted;
        }
        else
            rend.material.shader = normal;
        */
    }

    public void OnMouseExit() {
        outline.OnDisable();
    }

    public void OnMouseDown() {
        Debug.Log("CLICK==========");
        if (interactor != null) {
            Debug.Log("Interactor is not null");
            interactor.TakeItemPackage(this);
            Destroy(transform.parent.gameObject);
        }
    }
}
