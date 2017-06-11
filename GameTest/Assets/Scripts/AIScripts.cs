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

    public Personality GetPersonality() {
        return personality;
    }

}

public class PlayerAI : AI {
    float dashTime = 0.2f;
    float dashTimer = 0f;

    public PlayerAI(Personality _personality, Body _body) : base(_personality, _body) {
    }

    public override void Start() {
        //dashTime = 0.05f;
    }

    public override void Update() {
        GetMoveInput();
        GetAttackInput();
    }

    void GetMoveInput() {
        if (Input.anyKeyDown) {
            //DASH CONTROLS
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (dashTimer > 0) {
                //Debug.Log(body.GetDir());
                //if (body.GetCurrMoveAct().name.Equals("MoveLeft") && moveKey == -1) {
                if (moveKey == -1 && body.GetFace() == Direction.Left) {
                    body.SetCurrMoveAct(new MoveAction("DashLeft", 1, body, Direction.Left, 3));
                    dashTimer = 0f;
                }
                //else if (body.GetCurrMoveAct().name.Equals("MoveRight") && moveKey == 1) {
                else if (moveKey == 1 && body.GetFace() == Direction.Right) {
                    body.SetCurrMoveAct(new MoveAction("DashRight", 1, body, Direction.Right, 3));
                    dashTimer = 0f;
                }
            }
        }
        if (Input.anyKey) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (body.GetCurrMoveAct().name == "Halt") {
                if (moveKey == -1) {
                    body.SetCurrMoveAct(new MoveAction("MoveLeft", 4, body, Direction.Left, 0));
                    dashTimer = dashTime;
                }
                else if (moveKey == 1) {
                    body.SetCurrMoveAct(new MoveAction("MoveRight", 4, body, Direction.Right, 0));
                    dashTimer = dashTime;
                }
            }
        }
        if (dashTimer > 0) {
            dashTimer -= Time.deltaTime;
        }
    }

    void GetAttackInput() {
        if (Input.GetKeyDown("space")) {
            body.SetCurrAct(new AttackAction("SampleAttack", 2, body, (GameObject) Resources.Load("Attack")));
        }
    }

}