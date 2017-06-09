using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

    TimeManager time; //Reference to time manager

    Transform bodyRender; //Reference to the body-renderer's transform

    float harmQuant; //Harm Variables
    float harmThreshold;

    float hinderQuant; //Hinder Variables
    float hinderThreshold;

    float[] damMods = { 1f, 1f, 1f, 1f, 1f, 1f }; //Mods for sensitivity to each type of damage

    int size; //Self-Explanatory *MUST BE SET
    Direction dir; //Self-Explanatory *MUST BE SET

    Action currMoveAct; //Current movement action
    Action currAct; //Current non-movement action

    List<string> targetTags = new List<string>(); //List of tag-filters *MUST BE SET

    AI mind; //The AI object that will generate actions *MUST BE SET

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRender = child; } //Set bodyRender equal to the transform of the proper childObject

        harmQuant = 0f;
        harmThreshold = 1f;

        hinderQuant = 0f;
        hinderThreshold = 1f;

        currMoveAct = new Action("Open", 0, this);
        currAct = new Action("Open", 0, this);
    }

    void Update() {
        mind.Update();
        if (time.clock) {
            Tick();
        }
    }

    public void BodyConstructor(int _size, Direction _dir, List<string> _targetTags, AI _mind) {
        size = _size;
        dir = _dir;
        targetTags = _targetTags;
        mind = _mind;
        mind.Start();
    }

    //TICK FUNCTION
    void Tick() {
        mind.Tick();
        currMoveAct.Tick();
        currAct.Tick();
    }

    //ABILITY TO GET HURT AND TO BE TARGETED
    public void Harmed(float delt) {
        harmQuant += delt;
    }

    public void Hindered(float delt) {
        hinderQuant += delt;
    }

    public void Damaged(Damage dam) {
        Harmed(dam.quant * damMods[(int)dam.type]);
    }

    public List<string> GetTargetTags() {
        return targetTags;
    }

    //ABILITY TO MOVE
    public void Move(Direction dir) {
        switch (dir) {
            case Direction.Left:
                gameObject.transform.Translate(-1, 0, 0);
                dir = Direction.Left;
                break;
            case Direction.Right:
                gameObject.transform.Translate(1, 0, 0);
                dir = Direction.Right;
                break;
        }
    }

    public void SetCurrMoveAct(Action _currMoveAct) {
        currMoveAct = _currMoveAct;
    }
    public Action GetCurrMoveAct() {
        return currMoveAct;
    }

    //GET/SET DIRECTION
    public void setDir(Direction _dir) {
        dir = _dir;
    }
    public Direction getDir() {
        return dir;
    }

    //SET ACTION
    public void SetCurrAct(Action _currAct) {
        currAct = _currAct;
    }
    public Action GetCurrAct() {
        return currAct;
    }

    //SET/GET MIND
    public void SetMind(AI _mind) {
        mind = _mind;
    }
    public AI GetMind() {
        return mind;
    }

}
