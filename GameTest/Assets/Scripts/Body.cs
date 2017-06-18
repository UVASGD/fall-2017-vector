using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Body : MonoBehaviour {

    public int brawn = 5;
    public int coordination = 5;

    TimeManager time; //Reference to time manager

    Transform bodyRender; //Reference to the body-renderer's transform

    float harmQuant; //Harm Variables
    float harmThreshold;

    float hinderQuant; //Hinder Variables
    float hinderThreshold;

    float[] damMods = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; //Mods for sensitivity to each type of damage

    int size; //Self-Explanatory *MUST BE SET
    public Direction face; //Self-Explanatory *MUST BE SET

    Action currMoveAct; //Current movement action
    Action currAct; //Current non-movement action

    List<string> targetTags = new List<string>(); //List of tag-filters *MUST BE SET

    AI mind; //The AI object that will generate actions *MUST BE SET

    List<Affecter> affecterList = new List<Affecter>(); //List of Affecters
    List<Affecter> traitList = new List<Affecter>(); //List of Traits 

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager)); //Set Time manager
        foreach (Transform child in transform) if (child.CompareTag("Renderer")) { bodyRender = child; } //Set bodyRender equal to the transform of the proper childObject

        harmQuant = 0f;
        harmThreshold = 1f;

        hinderQuant = 0f;
        hinderThreshold = 1f;

        currMoveAct = new HaltAction("Halt", 0, this);
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
        face = _dir;
        targetTags = _targetTags;
        mind = _mind;
        mind.Start();
    }

    //TICK FUNCTION
    void Tick() {
        for (int i = 0; i < affecterList.Count; i++) {
            Affecter affecter = affecterList.ElementAt(i);
            //Debug.Log("EEEEE");
            affecter.Tick();
        }
        mind.Tick();
        currMoveAct.Tick();
        currAct.Tick();
    }

    //ABILITY TO GET HURT AND TO BE TARGETED
    public void Harm(float delt) {
        harmQuant += delt;
    }

    public void Hinder(float delt) {
        hinderQuant += delt;
    }

    public void Damage(DamageType _damType, float _damQuant) {
        Harm(_damQuant * damMods[(int)_damType]);
    }

    public List<string> GetTargetTags() {
        return targetTags;
    }

    //ABILITY TO MOVE
    public void Move(Direction _dir) {
        switch (_dir) {
            case Direction.Left:
                gameObject.transform.Translate(-1, 0, 0);
                face = Direction.Left;
                break;
            case Direction.Right:
                gameObject.transform.Translate(1, 0, 0);
                face = Direction.Right;
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
        affecterList.Add(_affecter);
        _affecter.Enact();
    }
    public void RemoveAffecterFromAffecters(Affecter _affecter) {
        affecterList.Remove(_affecter);
        _affecter.Deact();
    }

    public void RemoveAffecterFromTraits(Affecter _affecter) {
        traitList.Remove(_affecter);
        _affecter.Deact();
    }

    //GET/SET EFFECTLIST AND TRAITLIST
    public List<Affecter> GetAffecterList() {
        return affecterList;
    }

    public void AddToAffecterList(Affecter newAffecter) {
        affecterList.Add(newAffecter);
    }

    public List<Affecter> GetTraitList() {
        return traitList;
    }

    public void AddToTraitList(Affecter newAffecter) {
        traitList.Add(newAffecter);
    }

    public int GetMoveSpeed() {
        return 15 - coordination;
    }

    public int GetDashSpeed() {
        return 11 - coordination;
    }
}
