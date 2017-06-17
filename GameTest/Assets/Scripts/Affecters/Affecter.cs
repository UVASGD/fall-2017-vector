using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate float ReactorDelegate(float val);

public class Affecter {

    protected Body targetBody;
    protected List<Affecter> interactorList = new List<Affecter>();
    protected List<Reactor> reactorList = new List<Reactor>();
    protected bool combinable;
    protected float vitality; //The strength/time left/'life' of the effect

    public virtual bool Check() {
        return true;
    }

    public virtual void Scan() { //THIS IS THE METHOD THAT SCANS TO SEE IF ANOTHER EFFECT HAS A MATCHING REACTOR
        List<Effect> allLists = (List<Effect>)targetBody.GetEffectList().Concat(targetBody.GetTraitList());
        foreach (Effect effect in allLists)
            if (combinable && effect.GetType() == this.GetType())
                effect.Combine((Effect)this);
            else 
                foreach (Reactor reactor in reactorList)
                    if (reactor.FindMatches(effect))
                        interactorList.Add(effect);
    }

    public virtual void Dewit() {
        foreach (Affecter interactor in interactorList) {
            if (interactor.GetType() == typeof(Effect) && !((Effect)interactor).IsPresent()) {
                interactorList.Remove(interactor);
                continue;
            }
            foreach (Reactor reactor in reactorList) {
                if (!reactor.IsImmortal() && reactor.vitality <= 0)
                    reactor.Deact();
                else
                    reactor.FindMatches(interactor);
            }
        }
    }

    public virtual void Deact() {
    }

    public virtual void Combine(Effect combiner) { }

    public float GetVitality() {
        return vitality;
    }

    public void SetVitality(float _vitality) {
        vitality = _vitality;
    }

    public void AffectVitality(float _mod) {
        vitality += _mod;
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

public class Effect : Affecter {
    protected bool present; //Whether or not this effect is still 'alive'
    protected bool inEffect; //Whether or not this effect should actively tick
    bool inEffectChanged; //Dirty bit to detect change
    protected float vRate; //The inherent rate at which this effect is altered
    protected int timer;
    protected int freq;

    public virtual void Tick() {
        if (timer <= 0) {
            if (!Check()) {
                if (inEffect) targetBody.RemoveAffecterFromEffects(this);
                else targetBody.RemoveAffecterFromTraits(this);
                return;
            }

            if (inEffectChanged) {
                if (inEffect)
                    targetBody.AddToEffectList(this);
                else {
                    targetBody.AddToTraitList(this);
                    return;
                }
            }

            Dewit();
            timer = freq;
        }

        timer--;
    }

    public override void Dewit() {
        base.Dewit();
        vitality += vRate;
    }

    public override bool Check() {
        if (vRate != 0 || interactorList.Count > 0) {
            if (!inEffect) { inEffectChanged = true; }
            inEffect = true;
        }
        else {
            if (inEffect) { inEffectChanged = true; }
            inEffect = false;
        }

        if (vitality <= 0)
            present = false;
        else present = true;

        return present;
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
}

public class Instant : Affecter { }

public class Damage : Instant {
    DamageType damType;
    float damQuant;

    public Damage(DamageType _damType, float _damQuant) {
        damType = _damType;
        damQuant = _damQuant;
    }
}


public class Reactor {
    protected Affecter parentAffecter; //Affecter to which this reactor has been attached
    //Reactors are paired with up methods that they call on the parentAffecter
    /* 
    Reactor goes to 0, nothing changes - Closed wound can reopen, therefore damage reactor remains : linkMod = null, immortal = true, vital = true/false
    Reactor goes to 0, delete Reactor - Chilling reactor on water can be heated, will delete self without altering Parent : linkMod = null, immortal = false, vital = false
    Reactor goes to 0, delete ParentAffecter - Ties of armor count as fueling. When they get destroyed, the armor falls apart : linkMod = null, immortal = false, vital = true
    Reactor changed, Parent changed by some amt, Reactor can die before Parent - Dirt-caked stone armor, reduced dirt would affect effectiveness : linkMod = +, immortal = false, vital = false
    Reactor changed, Reactor has same health as Parent - Heating of fire gets reduced by Chilling; affects fire's vitality : linkMod = Mathf.Infinity, immortal = false, vital = true 
    float linkMod - Proportional modifier for linked damage
    bool immortal - Can this reactor be destroyed
    bool vital - Will this destroy the ParentAffecter
    */
    protected Reactor[] reactants;
    public float vitality;
    protected float linkMod;
    protected bool immortal;
    protected bool vital;

    public Reactor(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) {
        parentAffecter = _parentAffecter;
        vitality = _vitality;
        linkMod = _linkMod;
        immortal = _immortal;
        vital = _vital;
    }

    public Reactor() { }

    public virtual void Deact() {
        parentAffecter.RemoveFromReactorList(this);
    }

    public bool FindMatches(Affecter _targetAffecter) {
        bool result = false;
        foreach (Reactor other in _targetAffecter.GetReactorList()) {
            foreach (Reactor own in reactants) {
                if (other.GetType() == own.GetType()) {
                    React(other);
                    result = true;
                }
            }
        }
        return result;
    }

    protected virtual void React(Reactor reactant) {
        //THIS WILL FIND THE PROPER METHODS TO RUN ACCORDING TO THE REACTANT E.G. FOR BURNING: reactant=fueling, AffectVitality(parentAffecter, bool _lifeLinksomeValue)
    }

    public bool IsImmortal() {
        return immortal;
    }

    public virtual void AffectVitality(float _delta) {
        if (linkMod == Mathf.Infinity)
            parentAffecter.AffectVitality(_delta);
    }

    public virtual void AffectVRate() {

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
            AffectVitality(-reactant.vitality);
    }

    protected void DryingReact(float reactantVit) {

    }
}

public class Oiling : Dampening {
    public Oiling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) : 
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Watering(), new Burning() };
    }

    public Oiling() { }
}

public class Watering : Dampening {
    public Watering(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) : 
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Oiling()};
    }

    public Watering() { }
}

public class Dirtying : Reactor {
    public Dirtying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning(), new Dampening(), new Winding()};
    }

    public Dirtying() { }
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
}

public class Drying : Reactor {
    public Drying(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Dampening()};
    }

    public Drying() { }
}

public class Heating : Reactor {
    public Heating(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Chilling(), new Freezing()};
    }

    public Heating() { }
}

public class Chilling : Reactor {
    public Chilling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Heating(), new Watering()};
    }

    public Chilling() { }
}

public class Freezing : Reactor {
    public Freezing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Heating()};
    }

    public Freezing() { }
}

public class Burning : Reactor {
    public Burning(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Fueling(), new Oiling(), new Winding(), new Dirtying()};
    }

    public Burning() { }
}

public class Fueling : Reactor {
    public Fueling(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Burning()};
    }

    public Fueling() { }
}

public class Hindering : Reactor {
    public Hindering(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Freeing()};
    }

    public Hindering() { }
}

public class Freeing : Reactor {
    public Freeing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Hindering()};
    }

    public Freeing() { }
}

public class Harming : Reactor {
    public Harming(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Healing()};
    }

    public Harming() { }
}

public class Healing : Reactor {
    public Healing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { new Harming()};
    }

    public Healing() { }
}

public class Crushing : Reactor {
    public Crushing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { };
    }

    public Crushing() { }
}

public class Slashing : Reactor {
    public Slashing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { };
    }

    public Slashing() { }
}

public class Piercing : Reactor {
    public Piercing(Affecter _parentAffecter, float _vitality = 1f, float _linkMod = Mathf.NegativeInfinity, bool _immortal = false, bool _vital = false) :
        base(_parentAffecter, _vitality, _linkMod, _immortal, _vital) {
        reactants = new Reactor[] { };
    }

    public Piercing() { }
}


