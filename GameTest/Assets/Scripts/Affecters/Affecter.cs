using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate float ReactorDelegate(float val);

public class Affecter {

    protected Body targetBody; //Body to which this affecter is attached
    protected List<Affecter> interactorList = new List<Affecter>(); //The list of affecters with which the affecter interacts
    protected List<Reactor> reactorList = new List<Reactor>(); //
    protected bool combinable; //Whether this affecter will combine with similar affecters
    protected float turnVitality; //The vitality with which other affecters interact
    protected float vitality; //The strength/time left/'life' of the affecter
    protected bool present; //Whether or not this affecter is still 'alive'
    protected bool inAffecter; //Whether or not this affecter should actively tick
    bool inAffecterChanged; //Dirty bit to detect change
    protected float vRate; //The inherent rate at which this affecter is altered
    protected int timer; //The timer that designates when to activate
    protected int freq; //The frequency at which the timer goes off

    public Affecter(Body _targetBody, float _vitality, float _vRate, int _freq = 5) {
        targetBody = _targetBody;
        vitality = _vitality;
        turnVitality = _vitality;
        combinable = false;
        present = true;
        inAffecter = true;
        inAffecterChanged = false;
        vRate = _vRate;
        timer = _freq;
        freq = _freq;
    }

    public virtual void Tick() {
        if (timer <= 0) { //WHEN THE AFFECTER IS READY TO INTERACT WITH OTHER AFFECTERS
            turnVitality = vitality; //THE VITALITY WITH WHICH OTHER AFFECTERS ARE AFFECTED
            for (int i = 0; i < reactorList.Count; i++) {  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
                Reactor reactor = reactorList.ElementAt(i);  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
                reactor.Check();  //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
            }
            if (!Check()) { //IF THE AFFECTER IS DEAD
                if (inAffecter) targetBody.RemoveAffecterFromAffecters(this); //IF IT'S IN THE AFFECTERLIST, DELETE IT THENCE
                else targetBody.RemoveAffecterFromTraits(this); //IF IT'S IN THE TRAITLIST, DELETE IT THENCE
                Deact(); //DELETE THE AFFECTER
                return; //STOP TICKING
            }
            if (inAffecterChanged) { //IF THE PLACE NEEDS TO BE SWAPPED
                inAffecterChanged = false; //RESET THE DIRTY BIT
                if (inAffecter) {//IF IT OUGHT TO BE IN THE AFFECTER LIST
                    targetBody.AddToAffecterList(this); //ADD TO AFFECTER LIST
                    targetBody.RemoveAffecterFromTraits(this);
                }
                else { //IF IT OUGHT TO BE IN THE TRAITLIST
                    targetBody.AddToTraitList(this); //ADD TO TRAITLIST
                    targetBody.RemoveAffecterFromAffecters(this);
                    return; //THE GOYIM KNOW; SHUT IT DOWN; STOP TICKING
                }
            }
            Dewit(); //PERFORM ITS FUNCTION AND INTERACT WITH OTHER AFFECTERS
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
        if (vitality <= 0 || !present) //IF THE AFFECTER IS DEAD
            present = false; //SET IT TO INACTIVE
        return present; //RETURN WHETHER OR NOT IT'S STILL ALIVE
    }

    public virtual void Scan() { //THIS IS THE METHOD THAT SCANS TO SEE IF ANOTHER EFFECT HAS A MATCHING REACTOR
        List<Affecter> allLists = (targetBody.GetAffecterList().Concat(targetBody.GetTraitList())).ToList(); //COMBINES EFFECTLIST AND TRAITLIST
        for (int i = 0; i < allLists.Count; i++) { //FOR EACH AFFECTER IN THE COMBINED LIST
            Affecter affecter = allLists.ElementAt(i); //FOR EACH AFFECTER IN THE COMBINED LIST
            if (combinable && affecter.GetType() == GetType()) //CHECK WHETHER AFFECTERS SHOULD COMBINE
                affecter.Combine(this); //COMBINE AFFECTERS
            else //IF THEY SHOULDN'T COMBINE
                for (int j = 0; j < reactorList.Count; j++) { //FOR EACH REACTOR IN THE REACTOR LIST
                    Reactor reactor = reactorList.ElementAt(j); //FOR EACH REACTOR IN THE REACTOR LIST
                    if (reactor.FindMatches(affecter) && (!interactorList.Contains(affecter))) //IF THE AFFECTER FROM THE COMBINED HAS A MATCHING REACTOR
                        interactorList.Add(affecter); //ADD THAT AFFECTER TO THE INTERACTOR LIST
                }
        }
    }

    public virtual void Enact() { //THIS IS WHAT THE AFFECTER WILL PERFORM WHEN FIRST ADDED
        Scan(); //SCAN TO UPDATE INTERACTOR LIST
    }

    public virtual void Dewit() { //THIS IS WHAT THE AFFECTER WILL PERFORM EVERY TURN, IF ACTIVE
        vitality += vRate; //UPDATE VITALITY
        Debug.Log(vitality);
        for (int i = 0; i < interactorList.Count; i++) { //FOR EACH INTERACTOR IN THE LIST
            Affecter interactor = interactorList.ElementAt(i); //FOR EACH INTERACTOR IN THE LIST
            if (!(interactor).IsPresent()) { //IF THE INTERACTOR ISN'T ALIVE
                interactorList.Remove(interactor);  //DELET THIS
                continue; //IGNORE THE REST OF THE POTENTIAL INTERACTION WITH THIS INTERACTOR
            }
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

    }

    public virtual void Combine(Affecter combiner) { }

    public float GetTurnVitality() {
        return turnVitality;
    }

    public void SetVitality(float _vitality) {
        vitality = _vitality;
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

    public List<Reactor> GetReactorList() {
        return reactorList;
    }

    public void RemoveFromReactorList(Reactor reactor) {
        reactorList.Remove(reactor);
    }

    public void AddToInteractorList(Affecter _affecter) {
        interactorList.Add(_affecter);
    }

    public List<Affecter> GetInteractorList() {
        return interactorList;
    }
}

//Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false

public class Fire : Affecter {
    public Fire(Body _targetBody, float _vitality, float _vRate = -1f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new Burning(this, _vitality, Mathf.Infinity, false, true),
                                          new Drying(this, _vitality, Mathf.Infinity, false, true),
                                          new Heating(this, _vitality, Mathf.Infinity, false, true)};
    }
}

/* 
Reactor goes to 0, nothing changes - Closed wound can reopen, therefore damage reactor remains : linkMod = null, immortal = true, vital = true/false
Reactor goes to 0, delete Reactor - Chilling reactor on water can be heated, will delete self without altering Parent : linkMod = null, immortal = false, vital = false
Reactor goes to 0, delete ParentAffecter - Ties of armor count as fueling. When they get destroyed, the armor falls apart : linkMod = null, immortal = false, vital = true
Reactor changed, Parent changed by some amt, Reactor can die before Parent - Dirt-caked stone armor, reduced dirt would affect affecteriveness : linkMod = +, immortal = false, vital = false
Reactor changed, Reactor has same health as Parent - Heating of fire gets reduced by Chilling; affects fire's vitality : linkMod = Mathf.Infinity, immortal = false, vital = true 
float linkMod - Proportional modifier for linked damage
bool immortal - Can this reactor be destroyed
bool vital - Will this destroy the ParentAffecter
*/
public class Reactor {
    protected Affecter parentAffecter; //Affecter to which this reactor has been attached
    protected Reactor[] reactants;
    public float vitality;
    public float turnVitality;
    float lastParentVitality;
    protected float linkMod;
    protected bool immortal;
    protected bool vital;

    public Reactor(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) {
        parentAffecter = _parentAffecter;
        vitality = _vitality;
        turnVitality = _vitality;
        lastParentVitality = parentAffecter.GetTurnVitality();
        linkMod = _linkMod;
        immortal = _immortal;
        vital = _vital;
    }

    public Reactor() { }

    public virtual void Check() { //THIS WILL ADJUST THE VITALITY OF EACH REACTOR AT THE START OF EACH WAVE OF AFFECTER INTERACTIONS
        if (linkMod != Mathf.NegativeInfinity) {
            if (linkMod == Mathf.Infinity)
                vitality = parentAffecter.GetTurnVitality();
            else vitality += ((parentAffecter.GetTurnVitality() - lastParentVitality)  / lastParentVitality) * linkMod * vitality;
        }
        turnVitality = vitality;
        lastParentVitality = parentAffecter.GetTurnVitality();
    }

    public virtual void Deact() {
        parentAffecter.RemoveFromReactorList(this);
    }

    public virtual bool FindMatches(Affecter _targetAffecter) {
        bool result = false;
        List<Reactor> otherList = _targetAffecter.GetReactorList();
        for (int i = 0; i < otherList.Count; i++) {
            Reactor other = otherList.ElementAt(i);
            for (int j = 0; j < reactants.Length; j++) {
                Reactor own = reactants[j];
                if (other.GetType() == own.GetType()) {
                    React(other);
                    result = true;
                }
            }
        }
        return result;
    }

    protected virtual void React(Reactor reactant) {
    }

    public bool IsImmortal() {
        return immortal;
    }

    public bool IsVital() {
        return vital;
    }

    public float GetLinkMod() {
        return linkMod;
    }

    public virtual void AffectVitality(float _delta) {
        if (!immortal) {
            vitality += _delta;
            if (linkMod != Mathf.NegativeInfinity) {
                if (linkMod == Mathf.Infinity)
                    parentAffecter.AffectVitality(_delta);
                else parentAffecter.AffectVitality(_delta * linkMod);
            }
        }
    }

    public virtual void AffectVRate() {
    }

    public Affecter GetParentAffecter() {
        return parentAffecter;
    }
}

//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public class Dampening : Reactor {
    public Dampening(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) : 
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Dirtying(), new Drying()};
    }

    public Dampening() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Drying))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Oiling : Reactor {
    bool onFire = false;
    bool foundFire = false;

    public Oiling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) : 
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Watering(), new Burning() };
    }

    public Oiling() { }

    public override void Check() {
        base.Check();
        if (foundFire == false)
            onFire = false;
        foundFire = false;
    }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Watering))
            if (!onFire && reactant.turnVitality > (turnVitality / 2))
                AffectVitality(-reactant.turnVitality / 2);
        else if (reactant.GetType() == typeof(Burning)) {
            foundFire = true;
            onFire = true;
        }
    }  
}

public class Watering : Reactor {
    public Watering(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) : 
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Oiling(), new Chilling()};
    }

    public Watering() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Chilling))
            if (reactant.turnVitality > (turnVitality * 1.25f)) { }
                //(Water)parentAffecter.Freeze(); - The water parentAffecter should turn into an iceLayer affecter
    }
}

public class Dirtying : Reactor {
    public Dirtying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning(), new Dampening(), new Winding()};
    }

    public Dirtying() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Dampening))
            AffectVitality(-reactant.turnVitality / 2);
        else if (reactant.GetType() == typeof(Winding))
            if (reactant.turnVitality > (turnVitality / 2))
                AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Winding : Reactor {
    public Winding(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning(), new Dirtying(), new Puny()};
    }

    public Winding() { }
}

public class Puny : Reactor {
    public Puny(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Winding()};
    }

    public Puny() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Winding)) {
            if (reactant.turnVitality > turnVitality) { }
                //(Item)parentAffecter.Drop(); - The small object affecter shouldn't just remove itself from the list, it should drop into the world
        }
    }
}

public class Drying : Reactor {
    bool oilFire;

    public Drying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Dampening()};
        oilFire = false;
    }

    public Drying() { }

    public override void Check() {
        base.Check();
        //Debug.Log("HOWDY DOODY, I'M THE DRIER");
    }

    public void SetOilFire(bool _oilFire) {
        oilFire = _oilFire;
    }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Dampening)) {
            if (!oilFire)
                AffectVitality(-reactant.turnVitality / 2);
            else AffectVitality(reactant.turnVitality / 4);
        }
    }
}

public class Heating : Reactor {
    public Heating(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Chilling(), new Freezing()};
    }

    public Heating() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Chilling))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Chilling : Reactor {
    public Chilling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Heating(), new Watering()};
    }

    public Chilling() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Heating))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Freezing : Reactor {
    public Freezing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Heating()};
    }

    public Freezing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Heating))
            if (reactant.turnVitality > (turnVitality * 2))
                AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Burning : Reactor {
    bool oilFire;
    bool foundOil;

    public Burning(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Fueling(), new Oiling(), new Winding(), new Dirtying()};
        oilFire = false;
        foundOil = false;
    }

    public Burning() { }

    public override void Check() {
        base.Check();
        if (foundOil == false) 
            oilFire = false;
        foundOil = false;
        SetOilFire();
    }

    public void SetOilFire() {
        foreach (Reactor reactor in parentAffecter.GetReactorList())
            if (reactor.GetType() == typeof(Drying))
                ((Drying)reactor).SetOilFire(oilFire);
    }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Fueling))
            if (turnVitality > (reactant.turnVitality / reactant.GetLinkMod()) / 2)
                AffectVitality((reactant.turnVitality * reactant.GetLinkMod()));
        else if (reactant.GetType() == typeof(Oiling)) {
            foundOil = true;
            oilFire = true;
        }
        else if (reactant.GetType() == typeof(Winding))
            AffectVitality(reactant.turnVitality / 4);
        else if (reactant.GetType() == typeof(Dirtying))
            AffectVitality(reactant.turnVitality / 2);
    }
}

public class Fueling : Reactor {
    public Fueling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning()};
    }

    public Fueling() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Burning))
            if (reactant.turnVitality > (turnVitality / linkMod) / 2)
                AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Hindering : Reactor {
    public Hindering(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Freeing()};
    }

    public Hindering() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Freeing))
            AffectVitality(-reactant.turnVitality);
    }
}

public class Freeing : Reactor {
    public Freeing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Hindering()};
    }

    public Freeing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Freeing))
            AffectVitality(-reactant.turnVitality);
    }
}

public class Harming : Reactor {
    public Harming(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Healing()};
    }

    public Harming() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Winding))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Healing : Reactor {
    public Healing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Harming()};
    }

    public Healing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Winding))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Crushing : Reactor {
    public Crushing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new CrushingBlock()};
    }

    public Crushing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(CrushingBlock))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class CrushingBlock : Reactor {
    public CrushingBlock(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Crushing()};
    }

    public CrushingBlock() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Crushing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Slashing : Reactor {
    public Slashing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new SlashingBlock()};
    }

    public Slashing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(SlashingBlock))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class SlashingBlock : Reactor {
    public SlashingBlock(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Slashing()};
    }

    public SlashingBlock() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Slashing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Piercing : Reactor {
    public Piercing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new PiercingBlock()};
    }

    public Piercing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(PiercingBlock))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class PiercingBlock : Reactor {
    public PiercingBlock(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Piercing()};
    }

    public PiercingBlock() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Piercing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}


