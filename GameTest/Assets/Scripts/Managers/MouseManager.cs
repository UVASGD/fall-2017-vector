using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct MouseState {
    public bool rightUp;
    public bool rightDown;
    public float rightHold;

    public bool leftUp;
    public bool leftDown;
    public float leftHold;

    public bool cancel;
}

public class MouseManager : MonoBehaviour {

    MouseState current;
    public MouseState State { get { return current; } }

    bool sendit = false;
    public bool ShouldSend { get { return sendit; } }

    bool clearLeft = false;
    bool clearRight = false;

	// Use this for initialization
	void Start () {
        current.rightUp = false;
        current.rightDown = false;
        current.rightHold = 0f;


        current.leftUp = false;
        current.leftDown = false;
        current.leftHold = 0f;

        current.cancel = false;
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.mousePosition.y > 280 && Input.mousePosition.y < 640)

        // Check if the mouse was clicked over a UI element
        GameMode();
        if (EventSystem.current.IsPointerOverGameObject()) {
            current.cancel = true;
        }
        else
            current.cancel = false;
    }

    void OnGUI() {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        GUILayout.EndArea();
    }

    void UIMode() {

    }

    void GameMode() {
        Clear();
        if (Input.GetMouseButtonUp(0)) {
            current.leftUp = true;
            sendit = true;
            clearLeft = true;
        }
        else if (Input.GetMouseButtonDown(0)) {
            current.leftDown = true;
            sendit = true;
        }
        else if (Input.GetMouseButton(0)) {
            current.leftHold += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(1)) {
            current.rightUp = true;
            sendit = true;
            clearRight = true;
        }
        else if (Input.GetMouseButtonDown(1)) {
            current.rightDown = true;
            sendit = true;
        }
        else if (Input.GetMouseButton(1)) {
            current.rightHold += Time.deltaTime;
        }
        // Debug.Log(Input.mousePosition.ToString());  640 to 280
    }

    void Clear() {
        sendit = false;

        current.cancel = false;

        current.rightUp = false;
        current.rightDown = false;

        current.leftUp = false;
        current.leftDown = false;

        if (clearLeft) {
            current.leftHold = 0f;
            clearLeft = false;
        }
        if (clearRight) {
            current.rightHold = 0f;
            clearRight = false;
        }
    }
}