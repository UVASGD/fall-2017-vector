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

    MouseManager mouse;

    public PlayerAI(Personality _personality, Body _body) : base(_personality, _body) {
        
    }

    public override void Start() {
        //dashTime = 0.05f;
        mouse = (MouseManager)MonoBehaviour.FindObjectOfType(typeof(MouseManager));
    }

    public override void Update() {
        GetMoveInput();
        // Debug.Log(mouse);
        if (body != null && body.Weapon != null && mouse != null) {
            body.Weapon.PlayerInput(mouse.State);
        }
    }

    public override void Tick() {
        float mousePos = (Camera.main.ScreenToViewportPoint(Input.mousePosition).x - 0.5f) * 55.2765f;
        body.face = (Direction)Mathf.Sign(mousePos - (body.gameObject.transform.position.x));
        InteractableSearch();
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

    void InteractableSearch() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in objs) {
            //Interactable inter = obj.GetComponent<Interactable>();
            ItemPackage inter = obj.GetComponent<ItemPackage>();
            if (inter != null)
                inter.CheckInteract(body);
        }
    }

}

public class TurretAI : AI {

    int coolTime = 20;
    int coolCount = 20;

    public TurretAI(Personality _personality, Body _body) : base(_personality, _body) {
    }

    public override void Start() { }

    public override void Update() { }

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

public class Inanimate : AI {
    public Inanimate(Personality _personality, Body _body) : base(_personality, _body) { }
}