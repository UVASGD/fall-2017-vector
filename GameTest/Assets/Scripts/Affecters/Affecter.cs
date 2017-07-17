using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Affecter {

    protected Body targetBody; //Body to which this affecter is attached
    protected List<Affecter> interactorList = new List<Affecter>(); //The list of affecters with which the affecter interacts
    protected List<Reactor> reactorList = new List<Reactor>(); //

    protected bool combinable; //Whether this affecter will combine with similar affecters

    protected bool spreadable; //Whether this affecter can be spread to other objects by contact
    protected float spreadMod; //The ratio of how much vitality will be shared with an object to which the affecter has been spread

    protected bool layered; //Whether this affecter can be layered over/under another layered affecter

    protected float turnVitality; //The vitality with which other affecters interact
    protected float vitality; //The strength/time left/'life' of the affecter
    protected float fullVitality;
    protected float naturalVitality;

    protected float vRate; //The inherent rate at which this affecter is altered
    protected float natVrate;

    protected bool present; //Whether or not this affecter is still 'alive'
    public bool Present { set { present = value; } }
    protected bool immortal;

    protected bool inAffecter; //Whether or not this affecter should actively tick
    bool inAffecterChanged; //Dirty bit to detect change

    protected int timer; //The timer that designates when to activate
    protected int freq; //The frequency at which the timer goes off

    public Affecter(Body _targetBody, float _vitality, float _vRate, float _spreadMod = 1f, bool _immortal = false, int _freq = 5) {
        //targetBody = _targetBody;
        vitality = _vitality;
        fullVitality = 100f;
        naturalVitality = _vitality;
        turnVitality = _vitality;
        combinable = false;
        spreadable = false;
        spreadMod = _spreadMod;
        layered = false;
        present = true;
        immortal = _immortal;
        inAffecter = true;
        inAffecterChanged = false;
        vRate = _vRate;
        natVrate = _vRate;
        timer = _freq;
        freq = _freq;
    }

    public virtual void Tick() {
        if (timer <= 0) { //WHEN THE AFFECTER IS READY TO INTERACT WITH OTHER AFFECTERS
            for (int i = 0; i < reactorList.Count; i++) {  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
                Reactor reactor = reactorList.ElementAt(i);  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
                reactor.Check();  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
            }
            if (!Check()) { //IF THE AFFECTER IS DEAD
                if (inAffecter) targetBody.RemoveFromAffecterList(this); //IF IT'S IN THE AFFECTERLIST, DELETE IT THENCE
                else targetBody.RemoveFromTraitList(this); //IF IT'S IN THE TRAITLIST, DELETE IT THENCE
                if (spreadable) targetBody.RemoveFromSpreadList(this); //IF IT'S IN THE SPREAD LIST, DELETE IT THENCE
                if (layered) targetBody.RemoveFromLayerList(this); //IF IT SERVES AS A LAYER, DELETE IT THENCE
                return; //STOP TICKING
            }
            if (inAffecterChanged) { //IF THE PLACE NEEDS TO BE SWAPPED
                inAffecterChanged = false; //RESET THE DIRTY BIT
                if (inAffecter) {//IF IT OUGHT TO BE IN THE AFFECTER LIST
                    targetBody.AddToAffecterList(this); //ADD TO AFFECTER LIST
                    targetBody.RemoveFromTraitList(this); //REMOVE FROM TRAIT LIST
                }
                else { //IF IT OUGHT TO BE IN THE TRAITLIST
                    targetBody.AddToTraitList(this); //ADD TO TRAITLIST
                    targetBody.RemoveFromAffecterList(this); //ADD TO AFFECTER LIST 
                    return; //THE GOYIM KNOW; SHUT IT DOWN; STOP TICKING
                }
            }
            Dewit(); //PERFORM ITS FUNCTION AND INTERACT WITH OTHER AFFECTERS
            turnVitality = vitality; //THE VITALITY WITH WHICH OTHER AFFECTERS ARE AFFECTED
            timer = freq; //RESET TIMER 
        }
        timer--; //DECREASE TIMER
    }

    public virtual bool Check() { //CHECKS TO SEE IF THE AFFECTER IS STILL ALIVE/PUTS IT IN THE PROPER LIST
        if (vRate != 0 || interactorList.Count > 0) { //IF THE AFFECTER INHERENTLY CHANGES OR NEEDS TO INTERACT WITH AFFECTERS IN THE INTERACTOR LIST
            if (!inAffecter) { inAffecterChanged = true; } //IF THE AFFECTER ISN'T ALREADY BEING ACTIVELY TICKED
            inAffecter = true; //BETTER FIX THAT
        }
        else { //IF THE AFFECTER IS NOT INHERENTLY CHANGING AND THE INTERACTOR LIST IS EMPTY
            if (inAffecter) { inAffecterChanged = true; } //IF THE AFFECTER IS ACTIVELY BEING TICKED
            inAffecter = false; //OOFA FIX THAT
        }
        if ((vitality <= 0 || !present) && !immortal) {//IF THE AFFECTER IS DEAD 
            present = false; //SET IT TO INACTIVE
            Deact();
        }
        return present; //RETURN WHETHER OR NOT IT'S STILL ALIVE
    }

    public virtual bool Scan() { //THIS IS THE METHOD THAT SCANS TO SEE IF ANOTHER EFFECT HAS A MATCHING REACTOR
        List<Affecter> allLists = (targetBody.GetAffecterList().Concat(targetBody.GetTraitList())).ToList(); //COMBINES EFFECTLIST AND TRAITLIST
        for (int i = 0; i < allLists.Count; i++) { //FOR EACH AFFECTER IN THE COMBINED LIST
            Affecter affecter = allLists.ElementAt(i); //FOR EACH AFFECTER IN THE COMBINED LIST
            if (combinable && affecter.GetType() == GetType()) {//CHECK WHETHER AFFECTERS SHOULD COMBINE
                Combine(affecter); //COMBINE AFFECTERS
                return false;
            }
            else //IF THEY SHOULDN'T COMBINE
                for (int j = 0; j < reactorList.Count; j++) { //FOR EACH REACTOR IN THE REACTOR LIST
                    Reactor reactor = reactorList.ElementAt(j); //FOR EACH REACTOR IN THE REACTOR LIST
                    if (reactor.FindMatches(affecter) && (!interactorList.Contains(affecter))) {//IF THE AFFECTER FROM THE COMBINED HAS A MATCHING REACTOR
                        interactorList.Add(affecter); //ADD THAT AFFECTER TO THE INTERACTOR LIST
                        affecter.AddToInteractorList(this);
                    }
                }
        }
        return true;
    }

    public virtual bool Enact(Body _targetBody) { //THIS IS WHAT THE AFFECTER WILL PERFORM WHEN FIRST ADDED
        targetBody = _targetBody;
        if (Scan()) { //SCAN TO UPDATE INTERACTOR LIST
            if (spreadable) //IF THIS AFFECTER CAN BE SPREAD BY CONTACT
                targetBody.AddToSpreadList(this); //ADD IT TO THE SPREAD LIST
            if (layered && !targetBody.GetLayerList().Contains(this)) //IF THIS AFFECTER SERVES AS A LAYER
                targetBody.AddToLayerList(this); //ADD IT TO THE LAYER LIST
            return true;
        }
        return false;
    }

    public virtual void Dewit() { //THIS IS WHAT THE AFFECTER WILL PERFORM EVERY TURN, IF ACTIVE
        vitality += vRate; //UPDATE VITALITY
        for (int i = 0; i < interactorList.Count; i++) { //FOR EACH INTERACTOR IN THE LIST
            Affecter interactor = interactorList.ElementAt(i); //FOR EACH INTERACTOR IN THE LIST
            if (!(interactor).IsPresent()) { //IF THE INTERACTOR ISN'T ALIVE
                interactorList.Remove(interactor);  //DELET THIS
                continue; //IGNORE THE REST OF THE POTENTIAL INTERACTION WITH THIS INTERACTOR
            }
            if (layered && interactor.IsLayered()) //IF BOTH AFFECTERS ACT AS LAYERS
                if (Mathf.Abs(targetBody.GetLayerVal(interactor) - targetBody.GetLayerVal(this)) > 1) //IF THE INTERACTOR ISN'T ONE LAYER BENEATH OR ABOVE
                    continue; //CEASE THE INTERACTION
            for (int j = 0; j < reactorList.Count; j++) { //FOR EACH REACTOR IN THE REACTOR LIST
                Reactor reactor = reactorList.ElementAt(j); //FOR EACH REACTOR IN THE REACTOR LIST
                if (reactor.vitality <= 0) //IF THE REACTOR IS DEAD
                    if (!reactor.IsImmortal()) { //LIKE... DEAD
                        reactor.Deact(); //DELETE REACTOR
                        if (reactor.IsVital()) {//IF THE REACTOR IS VITAL
                            present = false; //DESTROY THE AFFECTER TO WHICH THE REACTOR IS ATTACHED
                            return;
                        }
                    }
                    else //IF THE REACTOR IS STILL ALIVE
                        reactor.FindMatches(interactor); //HANDLE ANY INTERACTIONS IT MIGHT HAVE WITH THE INTERACTOR
            }
        }
    }

    public virtual void Deact() {
        for (int i = 0; i < reactorList.Count; i++) {
            Reactor r = reactorList[i];
            r.Deact();
        }
    }

    public virtual void Combine(Affecter combiner) {
        combiner.AffectVitality(vitality);
        for (int i = 0; i < reactorList.Count; i++) {
            Reactor ownReactor = reactorList.ElementAt(i);
            for (int j = 0; j < combiner.reactorList.Count; j++) {
                Reactor otherReactor = combiner.reactorList.ElementAt(j);
                if (ownReactor.GetType() != otherReactor.GetType())
                    combiner.reactorList.Add(ownReactor.CloneReactor(ownReactor, combiner));
            }
        }
        //Deact(); //DELETE THE AFFECTER
        present = false;
    }

    public float GetTurnVitality() {
        return turnVitality;
    }

    public float GetSpreadVitality() {
        return turnVitality * spreadMod;
    }

    public Affecter GetAffecterClone<T>(T _this, bool spread = false) where T : Affecter {
        T affecterClone = (T)_this.MemberwiseClone();
        affecterClone.present = true;
        affecterClone.inAffecter = true;
        affecterClone.inAffecterChanged = false;
        affecterClone.interactorList = new List<Affecter>();
        affecterClone.reactorList = new List<Reactor>();
        for (int i = 0; i < _this.reactorList.Count; i++) {
            Reactor reactor = _this.reactorList.ElementAt(i);
            affecterClone.reactorList.Add(reactor.CloneReactor(reactor, affecterClone));
        }
        if (spread) {
            affecterClone.vitality *= spreadMod;
            affecterClone.turnVitality = affecterClone.vitality;
        }
        else { affecterClone.vitality = naturalVitality; affecterClone.turnVitality = affecterClone.vitality; }
        return affecterClone;
    }

    public void SetVitality(float _vitality) {
        vitality = _vitality;
    }

    public void ResetVitality() {
        vitality = naturalVitality;
        turnVitality = vitality;
        vRate = natVrate;
    }

    public void AffectVitality(float _mod) {
        vitality += _mod;
    }

    public float GetVRate() {
        return vRate;
    }

    public void SetVRate(float _vRate) {
        vRate = _vRate;
    }

    public bool IsPresent() {
        return present;
    }

    public bool IsLayered() {
        if (vitality < (fullVitality / 2))
            return false;
        return layered;
    }

    public List<Reactor> GetReactorList() {
        return reactorList;
    }

    public void RemoveFromReactorList(Reactor reactor) {
        reactorList.Remove(reactor);
    }

    public void AddToInteractorList(Affecter _affecter) {
        inAffecter = true;
        inAffecterChanged = true;
        timer = 0;
        Tick();
        interactorList.Add(_affecter);
    }

    public List<Affecter> GetInteractorList() {
        return interactorList;
    }

    public void Kill() {
        present = false;
    }
}

//Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false
/*
    protected List<Reactor> reactorList = new List<Reactor>(); //
    protected bool combinable; //Whether this affecter will combine with similar affecters
    protected bool spreadable; //Whether this affecter can be spread to other objects by contact
    protected float spreadMod; //The ratio of how much vitality will be shared with an object to which the affecter has been spread
    protected bool layered; //Whether this affecter can be layered over/under another layered affecter
    protected float vitality; //The strength/time left/'life' of the affecter
    protected float fullVitality;
    protected float vRate; //The inherent rate at which this affecter is altered
    protected int freq; //The frequency at which the timer goes off
 */


