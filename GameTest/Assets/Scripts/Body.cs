using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Body : MonoBehaviour {

    protected int brawn = 5;
    public int Brawn { get { return brawn; } }

    protected int coordination = 5;
    public int Coordination { get { return coordination; } }

    public int Athletics { get { return (brawn + coordination) / 2; } }

    protected ImpedimentLevel impediment = ImpedimentLevel.unimpeded;
    public ImpedimentLevel Impediment { get { return impediment; } set { impediment = value; } }

    protected TimeManager time; //Reference to time manager

    protected Transform bodyRenderTransform; //Reference to the body-renderer's transform

    int bodyCollisions = 0;

    public float harmQuant; //Harm Variables
    protected float harmThreshold;

    protected float hinderQuant; //Hinder Variables
    protected float hinderThreshold;

    protected int size; //Self-Explanatory *MUST BE SET
    public Direction face; //Self-Explanatory *MUST BE SET

    protected Action currMoveAct; //Current movement action
    protected Action currAct; //Current non-movement action

    protected List<string> targetTags = new List<string>(); //List of tag-filters *MUST BE SET
    public List<string> TargetTags { get { return targetTags;  } }

    protected AI mind; //The AI object that will generate actions *MUST BE SET
    public AI Mind { get { return mind; } }

    protected List<Affecter> affecterList = new List<Affecter>(); //List of Affecters
    protected List<Affecter> traitList = new List<Affecter>(); //List of Traits 

    protected List<Affecter> spreadList = new List<Affecter>();
    protected List<Affecter> layerList = new List<Affecter>();

    protected List<Item> inventory = new List<Item>();

    Item weapon;
    public Item Weapon { get { return weapon; } set { weapon = value; } }

    public Vector3 Position { get { return transform.position; } }

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRenderTransform = child; } 
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
    protected void Tick() {
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
            bodyRenderTransform.position = gameObject.transform.position;
        }
    }

    public void TakeItemPackage(ItemPackage package) {
        Debug.Log("Imma take dat");
        Debug.Log("Player inventory is: {");
        foreach (Item item in package.Inventory) {
            inventory.Add(item);
            Debug.Log(string.Format("{0},", item.Name));
        }
        Debug.Log("}");
        
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
