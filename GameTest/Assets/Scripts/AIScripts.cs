using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    protected Personality personality;
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

    float heavyTime = 1.5f;
    float button1Timer = 0;
    float button2Timer = 0;

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
        if (body.Impediment == ImpedimentLevel.noMove)
            return;
        if (Input.anyKeyDown) {
            //DASH CONTROLS
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (dashTimer > 0) {
                //Debug.Log(body.GetDir());
                //if (body.GetCurrMoveAct().name.Equals("MoveLeft") && moveKey == -1) {
                if (moveKey == -1 && body.GetFace() == Direction.Left) {
                    body.SetCurrMoveAct(new MoveAction("DashLeft", body.GetDashSpeed(), body, Direction.Left, 3));  // body.GetDashSpeed used to be 1
                    dashTimer = 0f;
                }
                //else if (body.GetCurrMoveAct().name.Equals("MoveRight") && moveKey == 1) {
                else if (moveKey == 1 && body.GetFace() == Direction.Right) {
                    body.SetCurrMoveAct(new MoveAction("DashRight", body.GetDashSpeed(), body, Direction.Right, 3));
                    dashTimer = 0f;
                }
            }
        }
        if (Input.anyKey) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (body.GetCurrMoveAct().name == "Halt") {
                if (moveKey == -1) {
                    body.SetCurrMoveAct(new MoveAction("MoveLeft", body.GetMoveSpeed(), body, Direction.Left, 0)); // body.GetMoveSpeed used to be 4
                    dashTimer = dashTime;
                }
                else if (moveKey == 1) {
                    body.SetCurrMoveAct(new MoveAction("MoveRight", body.GetMoveSpeed(), body, Direction.Right, 0));
                    dashTimer = dashTime;
                }
            }
        }
        if (dashTimer > 0) {
            dashTimer -= Time.deltaTime;
        }
    }

    void GetAttackInput() {

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("PREPARE PRIMARY");
            button1Timer = 1;
        }
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("PREPARE SECONDARY");
            button2Timer = 1;
            ((ICloseMelee)body.Weapon).CloseMeleeBlockEnact();
        }

        if (Input.GetMouseButtonUp(0)) {
            if (button1Timer > heavyTime) {
                if (button2Timer > heavyTime) {
                    Debug.Log("THROW");
                }
                else {
                    Debug.Log("HEAVY ATTACK");
                    if (body.Weapon is ICloseMelee)
                        ((ICloseMelee)body.Weapon).CloseMeleeHeavyAttack();
                    //body.SetCurrAct(new AttackAction("SampleAttack", 10 - body.Athletics, body, (GameObject)Resources.Load("Chest")));
                }
            }
            else if (button1Timer > 0) {
                if (button2Timer > heavyTime) {
                    Debug.Log("PARRY");
                    if (body.Weapon is ICloseMelee)
                        ((ICloseMelee)body.Weapon).CloseMeleeParry();
                }
                else {
                    Debug.Log("LIGHT ATTACK");
                    if (body.Weapon is ICloseMelee)
                        ((ICloseMelee)body.Weapon).CloseMeleeLightAttack();
                    //body.SetCurrAct(new AttackAction("SampleAttack", 10 - body.Athletics, body, (GameObject)Resources.Load("Attack")));
                }
            }

            button1Timer = 0;
            button2Timer = 0;
            body.Impediment = ImpedimentLevel.unimpeded;
        }
        if (Input.GetMouseButtonUp(1)) {
            if (button2Timer > heavyTime && button1Timer > heavyTime) {
                Debug.Log("THROW");
            }
            else if (button2Timer > 0) {
                Debug.Log("SHOVE");
            }

            Debug.Log("END BLOCK");
            ((ICloseMelee)body.Weapon).CloseMeleeBlockDeact();

            button1Timer = 0;
            button2Timer = 0;
            body.Impediment = ImpedimentLevel.unimpeded;
        }

        if (Input.GetMouseButton(0)) {
            if (button1Timer > 0.5 && button1Timer < 10) {
                button1Timer += Time.deltaTime;
            }
            if (button1Timer > heavyTime)
                body.Impediment = ImpedimentLevel.noMove;
        }
        if (Input.GetMouseButton(1)) {
            if (button2Timer > 0.5f && button2Timer < 10) {
                button2Timer += Time.deltaTime;
                if (button2Timer > heavyTime) {
                    button2Timer = 20;
                }
            }
        }



        if (Input.GetKeyDown("space")) {
        }
    }

}

public class TurretAI : AI {

    int coolTime = 20;
    int coolCount = 20;

    public TurretAI(Personality _personality, Body _body) : base(_personality, _body) {
    }

    public override void Start() {
        Debug.Log("ree");
    }

    public override void Update() {
    }

    public override void Tick() {
        coolCount--;

        if (coolCount == 0) {
            ReleaseAttack();
            coolCount = coolTime;
        }
    }

    public void ReleaseAttack() {
        ((ICloseMelee)body.Weapon).CloseMeleeLightAttack();
    }
}