using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

public class AI {
    protected Personality personality;
    public Personality GetPersonality { get { return personality; } }
    protected Body body;
    public Body GetBody { get { return body; } }

    protected bool dashing = false;
    public bool Dashing { get { return dashing; } }

    protected Place place;
    public virtual Place Place { get { return place; } set { place = value; } }

    protected QuestPicker questMind;
    public QuestPicker QuestMind {get { return questMind; } }

    protected List<WatchZone> zones = new List<WatchZone>();

    protected int personalityChecker = 0;

    public AI(Personality _personality, Body _body) {
        questMind = new QuestPicker(_body);
        personality = _personality;
        body = _body;
    }

    public virtual void Start() {
    }

    public virtual void Update() {
    }

    public virtual void Tick() {
        if (personality != null && personalityChecker++ > 200) {
            personality.Tick();
            personalityChecker = Random.Range(0, 50);
        }
        foreach (WatchZone zone in zones) {
            Debug.Log("zone: " + zone.ToString());
        }
    }
}

public class PlayerAI : AI {
    float dashTime = 0.2f;
    float dashTimer = 0f;

    float heavyTime = 1.5f;
    float button1Timer = 0;
    float button2Timer = 0;

    MouseManager mouse;
    PlayMusic music;

    private bool changedPlace = false;
    public override Place Place { get { return place; } set { place = value; changedPlace = true; } }

    public bool talkReady = false;
    public bool inventoryReady = false;
    public bool talking = false;

    public PlayerAI(Personality _personality, Body _body) : base(_personality, _body) {
        mouse = (MouseManager)MonoBehaviour.FindObjectOfType(typeof(MouseManager));
    }

    public override void Start() {
        //dashTime = 0.05f;
    }

    public override void Update() {
        GetMoveInput();
        if (body != null && body.Weapon != null && mouse != null) {
            if (talkReady == false && inventoryReady == false) {
                body.Weapon.PlayerInput(mouse.State);
            }
            else if (talkReady && !talking) {
                Talk();
            }
        }

        if (changedPlace) {   //  TODO: make this actually choose the right music based on the place
            MusicChoice new_music = MusicChoice.Credits;
            music.ChangePlace(new_music);
            changedPlace = false;
        }
    }

    public override void Tick() {
        //base.Tick();
        SpriteFace();
        InteractableSearch();
    }

    private void SpriteFace() {
        Vector3 worldPoint = new Vector3();
        Camera c = Camera.main;

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.

        worldPoint = c.ScreenToWorldPoint(Input.mousePosition);

        body.face = (Direction)Mathf.Sign(worldPoint.x - (body.gameObject.transform.position.x));
        body.transform.localScale = new Vector2((int)body.face, 1);
    }

    public void Talk() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.back);
            if (hit.collider != null) {
                Body bodhit = hit.collider.gameObject.GetComponent<Body>();
                if (bodhit == null || bodhit.GetPersonality() == null)
                    return;
                Personality otherPersonality = bodhit.GetPersonality();
                talking = true;
                body.SetCurrAct(new TalkAction("Talk", 1, body, otherPersonality));
            }
        }
    }

    void GetMoveInput() {
        if (body.Impediment == ImpedimentLevel.noMove)
            return;
        if (Input.anyKeyDown) {
            if (Input.GetKeyDown("k")) {
                if (!talkReady) {
                    talkReady = true;
                    inventoryReady = false;
                }
                else {
                    if (talking) {
                        talking = false;
                        body.SetCurrAct(new EndTalkAction("EndTalk", 1, body));
                    }
                    talkReady = false;
                }
            }
            if (Input.GetKeyDown("q") && !talking) {
                if (!inventoryReady) {
                    inventoryReady = true;
                    talkReady = false;
                }
                else inventoryReady = false;
            }
            //DASH CONTROLS
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (dashTimer > 0) {
                //Debug.Log(body.GetDir());
                //if (body.GetCurrMoveAct().name.Equals("MoveLeft") && moveKey == -1) {
                if (moveKey == -1 && body.GetFace() == Direction.Left) {
                    body.SetCurrMoveAct(new MoveAction("DashLeft", body.GetDashSpeed(), body, Direction.Left, 5));  // body.GetDashSpeed used to be 1
                    dashTimer = 0f;
                }
                //else if (body.GetCurrMoveAct().name.Equals("MoveRight") && moveKey == 1) {
                else if (moveKey == 1 && body.GetFace() == Direction.Right) {
                    body.SetCurrMoveAct(new MoveAction("DashRight", body.GetDashSpeed(), body, Direction.Right, 5));
                    dashTimer = 0f;
                }
                dashing = true;
            }
            else { dashing = false; }
        }
        if (Input.anyKey) {
            int moveKey = (int)Input.GetAxisRaw("Horizontal");
            if (body.CurrMoveAct.Name.Equals("Open") || body.CurrMoveAct.Name.Equals("Halt")) {
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

public class PersonAI : AI {


    public PersonAI(Personality _personality, Body _body) : base(_personality, _body) { }

    protected void Ding() { }

    public override void Tick() {
        if (body.CurrAct.Name.Equals("Open")) {
            Action nextAction = body.CurrSubQuest.GetAction();
            if (nextAction.Name.Equals("Open")) {
                if (!body.SetNextSubQuest())
                    Ding();
            }
        }
        base.Tick();
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

public class DogAI : AI {
    int coolTime = 20;
    int coolCount = 20;

    Direction desiredDirect;
    Direction playerDirect;

    GameObject player;

    public DogAI(Personality _personality, Body _body) : base(_personality, _body) {
    }

    public override void Start() {
        List<GameObject> results = MultiTag.FindGameObjectsWithTags(Tags.Player);
        player = results[0];
    }

    public override void Update() {
        if (player != null) {
            FindPlayer();
            DetermineDirection();
            body.SetFace(playerDirect);
        }
        else
            desiredDirect = Direction.None;


    }

    public override void Tick() {
        coolCount--;

        if (desiredDirect == Direction.None && coolCount == 0) {
            ReleaseAttack();
            coolCount = coolTime;
        }
        else if (desiredDirect != Direction.None && coolCount < 17) {
            body.Move(desiredDirect);
            coolCount = coolTime;
        }
    }

    public void ReleaseAttack() {
        ((ICloseMelee)body.Weapon).CloseMeleeLightAttack();
    }

    public void FindPlayer() {
        if (player.transform.position.x > body.transform.position.x)
            playerDirect = Direction.Right;
        else
            playerDirect = Direction.Left;
    }

    public void DetermineDirection() {
        if (Mathf.Abs((player.transform.position.x - body.transform.position.x)-(player.GetComponent<Body>().Size + body.Size)) <= 1)
            desiredDirect = Direction.None;
        else
            desiredDirect = playerDirect;
    }
}

public class Inanimate : AI {
    public Inanimate(Personality _personality, Body _body) : base(_personality, _body) { }
}