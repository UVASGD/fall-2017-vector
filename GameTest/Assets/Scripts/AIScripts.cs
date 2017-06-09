using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    Personality personality;
    protected Body body;

    public AI(Personality _personality, Body _body) {
        personality = _personality;
        body = _body;
    }

    public virtual void Start() {
    }

    public virtual void Update() {
    }

    public virtual void Tick() {
    }

}

public class PlayerAI : AI {
    float dashTime = 0.5f;

    public PlayerAI (Personality _personality, Body _body) : base(_personality, _body) {
    }

    public override void Start() {
        dashTime = 0.5f;
    }

    public override void Update() {
        getMoveInput();
    }

    void getMoveInput() {
        if (Input.anyKeyDown) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (dashTime > 0) {
                if (body.GetCurrMoveAct().name.Equals("MoveLeft") && moveKey == -1) {
                    body.SetCurrMoveAct(new MoveAction("DashLeft", 1, body, Direction.Left, 3));
                    dashTime = 0f;
                }
                else if (body.GetCurrMoveAct().name.Equals("MoveRight") && moveKey == 1) {
                    body.SetCurrMoveAct(new MoveAction("DashRight", 1, body, Direction.Right, 3));
                    dashTime = 0f;
                }
            }
        }

        if (Input.anyKey) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (body.GetCurrMoveAct().name == "Open") {
                if (moveKey == -1) {
                    body.SetCurrMoveAct(new MoveAction("MoveLeft", 4, body, Direction.Left, 0));
                    dashTime = 0.5f;
                }
                else if (moveKey == 1) {
                    body.SetCurrMoveAct(new MoveAction("MoveRight", 4, body, Direction.Right, 0));
                    dashTime = 0.5f;
                }
            }
        }

        if (dashTime > 0) {
            dashTime -= Time.deltaTime;
        }
    }

}