using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Body : MonoBehaviour {

    public int brawn = 5;
    public int Brawn { get { return brawn; } }

    public int coordination = 5;
    public int Coordination { get { return coordination; } }

    public int Athletics { get { return (brawn + coordination) / 2; } }

    ImpedimentLevel impediment = ImpedimentLevel.unimpeded;
    public ImpedimentLevel Impediment { get { return impediment; } set { impediment = value; } }

    TimeManager time; //Reference to time manager

    Transform bodyRender; //Reference to the body-renderer's transform

    int bodyCollisions = 0;

    public float harmQuant; //Harm Variables
    float harmThreshold;

    float hinderQuant; //Hinder Variables
    float hinderThreshold;

    int size; //Self-Explanatory *MUST BE SET
    public Direction face; //Self-Explanatory *MUST BE SET

    Action currMoveAct; //Current movement action
    Action currAct; //Current non-movement action

    List<string> targetTags = new List<string>(); //List of tag-filters *MUST BE SET
    public List<string> TargetTags { get { return targetTags;  } }

    AI mind; //The AI object that will generate actions *MUST BE SET
    public AI Mind { get { return mind; } }

    List<Affecter> affecterList = new List<Affecter>(); //List of Affecters
    List<Affecter> traitList = new List<Affecter>(); //List of Traits 

    List<Affecter> spreadList = new List<Affecter>();
    List<Affecter> layerList = new List<Affecter>();

    Item weapon;
    public Item Weapon { get { return weapon; } set { weapon = value; } }

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRender = child; } 
        //Set bodyRender equal to the transform of the proper childObject

        harmQuant = 0f;
        harmThreshold = 1f;

        hinderQuant = 0f;
        hinderThreshold = 1f;

        currMoveAct = new HaltAction("Halt", 0, this);
        currAct = new Action("Open", 0, this);
        AddAffecter(new ResistanceAggregate(this, 0f));
    }

    void Update() {
        mind.Update();
        if (time.clock) {
            Tick();
        }
    }

    public void BodyConstructor(int _size, Direction _dir, List<string> _targetTags, AI _mind) {
        size = _size;
        face = _dir;
        targetTags = _targetTags;
        mind = _mind;
        mind.Start();
    }

    //TICK FUNCTION
    void Tick() {
        if (mind.GetType() == typeof(PlayerAI)) {
            float mousePos = (Camera.main.ScreenToViewportPoint(Input.mousePosition).x - 0.5f) * 38;
            face = (Direction)Mathf.Sign(mousePos - (gameObject.transform.position.x));
        }
        for (int i = 0; i < affecterList.Count; i++) {
            Affecter affecter = affecterList.ElementAt(i);
            affecter.Tick();
        }
        mind.Tick();
        currMoveAct.Tick();
        currAct.Tick();
        if (harmQuant > harmThreshold) {
            DieIdiot();
        }
    }

    public void DieIdiot() {
        Destroy(gameObject);
    }

    //ABILITY TO GET HURT AND TO BE TARGETED
    public void ChangeHarm(float delt) {
        harmQuant += delt;
    }

    public void Hinder(float delt) {
        hinderQuant += delt;
    }

    //ABILITY TO MOVE
    public void Move(Direction _dir) {
        switch (_dir) {
            case Direction.Left:
                gameObject.transform.Translate(-1, 0, 0);
                //face = Direction.Left;
                break;
            case Direction.Right:
                gameObject.transform.Translate(1, 0, 0);
                //face = Direction.Right;
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
    public void SetFace(Direction _dir) {
        face = _dir;
    }
    public Direction GetFace() {
        return face;
    }

    //COLLISION
    private void OnTriggerEnter2D(Collider2D other) {
        Body otherBody = other.gameObject.GetComponent<Body>();
        if (otherBody != null) {
            Spread(otherBody);
            bodyCollisions++;
        }
    }

    void Spread(Body otherBody) {
        foreach (Affecter otherAffecter in otherBody.GetSpreadList()) {
            bool skip = false;
            if (otherAffecter.IsLayered())
                if (otherBody.GetLayerVal(otherAffecter) > 2)
                    continue;
            foreach (Affecter ownAffecter in spreadList) {
                if (ownAffecter.IsLayered())
                    if (GetLayerVal(ownAffecter) > 2) {
                        skip = true;
                        break;
                    }
                if (ownAffecter.GetType() == otherAffecter.GetType()) {
                    skip = true;
                    break;
                }
            }
            if (skip)
                continue;
            AddAffecter(otherAffecter.GetAffecterClone(otherAffecter, true));
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Body otherBody = other.gameObject.GetComponent<Body>();
        if (otherBody != null) {
            bodyCollisions--;
            bodyRender.position = gameObject.transform.position;
        }
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

    //GET PERSONALITY
    public Personality GetPersonality() {
        return mind.GetPersonality();
    }

    //ADD/REMOVE TO/FROM APPROPRIATE AFFECTER LIST
    public void AddAffecter(Affecter _affecter) {
        if (_affecter.Enact(this))
            affecterList.Add(_affecter);
    }

    public void RemoveFromAffecterList(Affecter _affecter) {
        affecterList.Remove(_affecter);
    }

    public void RemoveFromTraitList(Affecter _affecter) {
        traitList.Remove(_affecter);
    }

    //GET/SET EFFECTLIST AND TRAITLIST
    public List<Affecter> GetAffecterList() {
        return affecterList;
    }

    public void AddToAffecterList(Affecter _affecter) {
        affecterList.Add(_affecter);
    }

    public List<Affecter> GetTraitList() {
        return traitList;
    }

    public void AddToTraitList(Affecter _affecter) {
        traitList.Add(_affecter);
    }

    //GET/SET SPREADLIST
    public void AddToSpreadList(Affecter _affecter) {
        spreadList.Add(_affecter);
    }

    public List<Affecter> GetSpreadList() {
        return spreadList;
    }

    public void RemoveFromSpreadList(Affecter _affecter) {
        spreadList.Remove(_affecter);
    }

    //GET/SET LAYERLIST
    public void AddToLayerList(Affecter _affecter, int ind = 0) {
        layerList.Insert(ind, _affecter);
    }

    public void RemoveFromLayerList(Affecter _affecter) {
        layerList.Remove(_affecter);
    }

    public List<Affecter> GetLayerList() {
        return layerList;
    }

    public int GetLayerVal(Affecter _affecter) {
        return layerList.IndexOf(_affecter);
    }


    public int GetMoveSpeed() {
        return 15 - coordination;
    }

    public int GetDashSpeed() {
        return 11 - coordination;
    }
}
