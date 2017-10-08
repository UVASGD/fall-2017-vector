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
		
	}
}