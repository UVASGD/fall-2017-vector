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

    public virtual void Enact() {
        foreach (Affecter interactor in interactorList)
            foreach (Reactor reactor in reactorList)
                reactor.FindMatches(interactor);
    }

    public virtual void Deact() {
    }

    public virtual void Combine(Effect combiner) { }

    public List<Reactor> GetReactorList() {
        return reactorList;
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
    protected float vitality; //The strength/time left/'life' of the effect
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

            Enact();
            timer = freq;
        }

        timer--;
    }

    public override void Enact() {
        base.Enact();
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

    public float GetVitality() {
        return vitality;
    }

    public void SetVitality(float _vitality) {
        vitality = _vitality;
    }

    public float GetVRate() {
        return vRate;
    }

    public void SetVRate(float _vRate) {
        vRate = _vRate;
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
    protected Reactor[] reactants;
    public int vitality;

    public Reactor(Affecter _parentAffecter) {
        parentAffecter = _parentAffecter;
        vitality = 1;
    }

    public Reactor() { }

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
        //THIS WILL FIND THE PROPER METHODS TO RUN ACCORDING TO THE REACTANT E.G. FOR BURNING: reactant=fueling, AffectVitality(parentAffecter, someValue)
    } 

    public virtual void AffectVitality() {

    }

    public virtual void AffectVRate() {

    }
}

//public enum Reaction { Dampening, Oiling, Watering, Dirtying, Drying, Burning, Fueling, Hindering, Freeing, Harming, Healing, Crushing, Slashing, Piercing};

public class Dampening : Reactor {
    public Dampening(Affecter _parentAffecter) : base(_parentAffecter) {
        reactants = new Reactor[] { new Dirtying(), new Drying()};
    }

    public Dampening() { }
}

public class Oiling : Dampening {
    public Oiling(Affecter _parentAffecter) : base(_parentAffecter) {
        reactants = new Reactor[] { new Watering(), new Burning() };
    }

    public Oiling() { }
}

public class Watering : Dampening {
    public Watering(Affecter _parentAffecter) : base(_parentAffecter) {
        reactants = new Reactor[] { new Oiling()};
    }

    public Watering() { }
}

public class Dirtying : Reactor {
    public Dirtying(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Dirtying() { }
}

public class Drying : Reactor {
    public Drying(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Drying() { }
}

public class Heating : Reactor {
    public Heating(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Heating() { }
}

public class Chilling : Reactor {
    public Chilling(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Chilling() { }
}

public class Freezing : Reactor {
    public Freezing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Freezing() { }
}

public class Burning : Reactor {
    public Burning(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Burning() { }
}

public class Fueling : Reactor {
    public Fueling(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Fueling() { }
}

public class Hindering : Reactor {
    public Hindering(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Hindering() { }
}

public class Freeing : Reactor {
    public Freeing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Freeing() { }
}

public class Harming : Reactor {
    public Harming(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Harming() { }
}

public class Healing : Reactor {
    public Healing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Healing() { }
}

public class Crushing : Reactor {
    public Crushing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Crushing() { }
}

public class Slashing : Reactor {
    public Slashing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Slashing() { }
}

public class Piercing : Reactor {
    public Piercing(Affecter _parentAffecter) : base(_parentAffecter) {
    }

    public Piercing() { }
}


