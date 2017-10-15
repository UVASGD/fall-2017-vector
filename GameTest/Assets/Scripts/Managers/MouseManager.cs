using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MouseState {
    public bool rightUp;
    public bool rightDown;
    public float rightHold;

    public bool leftUp;
    public bool leftDown;
    public float leftHold;
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
	}
	
	// Update is called once per frame
	void Update () {
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
	}

    void Clear() {
        sendit = false;

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