using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MouseState {
    public bool rightUp;
    public bool rightDown;

    public bool leftUp;
    public bool leftDown;

    public float rightHold;
    public float leftHold;
}

public class MouseManager : MonoBehaviour {

    MouseState current;
    public MouseState State { get { return current; } }

    bool sendit = false;
    public bool ShouldSend { get { return sendit; } }

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
		if (Input.GetMouseButtonUp(1)) {
            current.leftUp = true;
            current.leftHold = 0f;
            sendit = true;
        }
        else if (Input.GetMouseButtonDown(1)) {
            current.leftDown = true;
            sendit = true;
        }
        else if (Input.GetMouseButton(1)) {
            current.leftHold += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(2)) {
            current.rightUp = true;
            current.rightHold = 0f;
            sendit = true;
        }
        else if (Input.GetMouseButtonDown(2)) {
            current.rightDown = true;
            sendit = true;
        }
        else if (Input.GetMouseButton(2)) {
            current.rightHold += Time.deltaTime;
        }
	}

    void Clear() {
        sendit = false;

        current.rightUp = false;
        current.rightDown = false;

        current.leftUp = false;
        current.leftDown = false;
    }
}