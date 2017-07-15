using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* 
Reactor goes to 0, nothing changes - Closed wound can reopen, therefore damage reactor remains : linkMod = null, immortal = true, vital = true/false
Reactor goes to 0, delete Reactor - Chilling reactor on water can be heated, will delete self without altering Parent : linkMod = null, immortal = false, vital = false
Reactor goes to 0, delete ParentAffecter - Ties of armor count as fueling. When they get destroyed, the armor falls apart : linkMod = null, immortal = false, vital = true
Reactor changed, Parent changed by some amt, Reactor can die before Parent - Dirt-caked armor, reduced dirt reduces effectiveness : linkMod = +, immortal = false, vital = false
Reactor changed, Reactor has same health as Parent - Heating of fire gets reduced by Chilling; affects fire's vitality : linkMod = Mathf.Infinity, immortal = false, vital = true 
float linkMod - Proportional modifier for linked damage
bool immortal - Can this reactor be destroyed
bool vital - Will this destroy the ParentAffecter
*/
public class Reactor {
    protected Affecter parentAffecter;
    protected Reactor[] reactants;
    public float vitality;
    public float turnVitality;
    protected float fullVitality;
    float lastParentVitality;
    protected float linkMod;
    protected bool immortal;
    protected bool vital;

    public Reactor(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) {
        parentAffecter = _parentAffecter;
        vitality = _vitality;
        turnVitality = _vitality;
        fullVitality = _vitality;
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
            else vitality += ((parentAffecter.GetTurnVitality() - lastParentVitality) / lastParentVitality) * linkMod * vitality;
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
            if (Mathf.Sign(_delta) == -1 && Mathf.Abs(_delta) > vitality)
                _delta = -vitality;
            else if ((_delta + vitality) > fullVitality)
                _delta = fullVitality - vitality;
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

    public Reactor CloneReactor<T>(T _this, Affecter _affecter) where T : Reactor {
        T reactorClone = (T)_this.MemberwiseClone();
        reactorClone.parentAffecter = _affecter;
        lastParentVitality = parentAffecter.GetTurnVitality();
        return reactorClone;
    }
}

//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public class Dampening : Reactor {
    public Dampening(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Dirtying(), new Drying() };
    }

    public Dampening() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Drying))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Acid : Reactor {
    public Acid(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Watering() };
    }

    public Acid() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Watering))
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
        reactants = new Reactor[] { new Oiling(), new Chilling(), new Acid() };
    }

    public Watering() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Chilling))
            if (reactant.turnVitality > (turnVitality * 1.25f))
                ((Water)parentAffecter).Freeze();
    }
}

public class Dirtying : Reactor {
    public Dirtying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning(), new Dampening(), new Winding() };
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
        reactants = new Reactor[] { new Burning(), new Dirtying(), new Puny() };
    }

    public Winding() { }
}

public class Puny : Reactor {
    public Puny(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Winding() };
    }

    public Puny() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Winding)) {
            if (reactant.turnVitality > turnVitality) {
                //(Item)parentAffecter.Drop(); - The small object affecter shouldn't just remove itself from the list, it should drop into the world
            }
        }
    }
}

public class Drying : Reactor {
    bool oilFire;

    public Drying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Dampening() };
        oilFire = false;
    }

    public Drying() { }

    public override void Check() {
        base.Check();
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
        reactants = new Reactor[] { new Chilling(), new Freezing() };
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
        reactants = new Reactor[] { new Heating(), new Watering() };
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
        reactants = new Reactor[] { new Heating() };
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
        reactants = new Reactor[] { new Fueling(), new Oiling(), new Winding(), new Dirtying() };
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
        reactants = new Reactor[] { new Burning() };
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
        reactants = new Reactor[] { new Freeing() };
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
        reactants = new Reactor[] { new Hindering() };
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
        reactants = new Reactor[] { new Healing() };
    }

    public Harming() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Healing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Healing : Reactor {
    public Healing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Harming() };
    }

    public Healing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Harming))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Crushing : Reactor {
    public Crushing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new CrushingResist() };
    }

    public Crushing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(CrushingResist))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class CrushingResist : Reactor {
    public CrushingResist(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Crushing() };
    }

    public CrushingResist() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Crushing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Slashing : Reactor {
    public Slashing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new SlashingResist() };
    }

    public Slashing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(SlashingResist))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class SlashingResist : Reactor {
    public SlashingResist(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Slashing() };
    }

    public SlashingResist() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Slashing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class Piercing : Reactor {
    public Piercing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new PiercingResist() };
    }

    public Piercing() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(PiercingResist))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class PiercingResist : Reactor {
    public PiercingResist(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Piercing() };
    }

    public PiercingResist() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Piercing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}

public class DamageResist : Reactor {
    public DamageResist(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Harming() };
    }

    public DamageResist() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Harming)) {
            if (vitality < 1f)
                reactant.AffectVitality(-reactant.vitality * turnVitality);
            else
                reactant.AffectVitality(-reactant.vitality);
        }
        Debug.Log(string.Format("Reactant vitality: {0}", reactant.vitality));
    }
}

public class ResistanceAdder : Reactor {
    Reactor aggrigate;

    public ResistanceAdder(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.Infinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new DamageResist() };
    }

    public ResistanceAdder() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(DamageResist)) {
            aggrigate = reactant;
            aggrigate.AffectVitality(vitality);
            Debug.Log(string.Format("aggrigate vitality: {0}", aggrigate.vitality));
        }
    }

    public override void Deact() {
        aggrigate.AffectVitality(-vitality);
    }

    public override void AffectVitality(float _delta) {
        if (vitality - _delta < 0) {
            aggrigate.AffectVitality(-vitality);
        }
        else {
            aggrigate.AffectVitality(_delta);
        }
        base.AffectVitality(_delta);
    }
}


// Ignore this one; not used yet
public class DamageReduce : Reactor {
    public DamageReduce(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Harming() };
    }

    public DamageReduce() { }

    protected override void React(Reactor reactant) {
        if (reactant.GetType() == typeof(Piercing))
            AffectVitality(-reactant.turnVitality / 2);
    }
}
