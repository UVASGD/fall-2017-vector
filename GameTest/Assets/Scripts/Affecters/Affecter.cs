using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affecter {

    protected Body targetBody;


    public virtual void Enact() {
    }

    public virtual void Deact() {
    }

    public virtual bool Check() {
        return true;
    }
}

public interface IIgnitor { }

public interface IIgnitable {
    bool Ignited(float intensity);
    float GetFlammability();
    float GetFlameResistance();
    void SetIgnitor(IIgnitor ignitor);
}

public interface Dampener {
    void Dampen();
}

public interface IElectrifier {
    void Electrify();
}

public interface IConductor {
    bool Conduct(float intensity);
}

//INSTANT AFFECTERS
public class Instant : Affecter {
    public override void Enact() { }
}

//public enum DamageType { Crushing, Piercing, Slashing, Burning, Freezing, Electric, Hindering, Magic};

public class Damage : Instant {

    DamageType type;
    float quant;

    public Damage(DamageType _type, float _quant) {
        type = _type;
        quant = _quant;
    }

    public override void Enact() {
        targetBody.Damage(type, quant);
    }

    public void SetQuant(float _quant) {
        quant = _quant;
    }
    public float GetQuant() {
        return quant;
    }
}
//INSTANT AFFECTERS


//LASTING AFFECTERS
public class Effect : Affecter {
    protected int timer;

    public virtual void Tick() {
        timer--;
        if (timer >= 0)
            Dewit();
        else
            Deact();
    }

    public override void Enact() { }

    public override  void Deact() { }

    public virtual void Dewit() { }
}

public class Burning : Effect, IIgnitor {
    Damage dam;
    int freq;
    bool fuelChange = false;
    List<IIgnitable> fuelList = new List<IIgnitable>();

    public Burning() {
        freq = 5;
    }

    public override void Tick() {
        timer--;
        if (timer == 0 && fuelList.Count > 0) {
            Dewit();
            timer = freq;
        }
        else if (fuelList.Count == 0)
            Deact();
    }

    public override bool Check() {
        foreach (Effect eff in targetBody.GetEffectList())
            if (eff.GetType() == typeof(IIgnitable))
                AddFuel((IIgnitable)eff);
        foreach (Trait tra in targetBody.GetTraitList())
            if (tra.GetType() == typeof(IIgnitable))
                AddFuel((IIgnitable)tra);
        return fuelList.Count > 0;
    }

    public override void Enact() {
        DetIntensity();
    }

    public override void Dewit() {
        if (fuelChange) {
            DetIntensity();
            fuelChange = false;
        }
        foreach (IIgnitable fuel in fuelList) {
            bool shouldKeep = fuel.Ignited(dam.GetQuant());
            if (!shouldKeep)
                RemoveFuel(fuel);
        }
        targetBody.AddAffecter(dam);
    }

    void DetIntensity() {
        float newIntensity = 0f;
        foreach (IIgnitable fuel in fuelList)
            if (fuel.GetFlammability() > newIntensity)
                newIntensity = fuel.GetFlammability();
        dam.SetQuant(newIntensity);
    }

    public void AddFuel(IIgnitable fuel) {
        fuelList.Add(fuel);
        fuelChange = true;
        fuel.SetIgnitor(this);
    }
    public void RemoveFuel(IIgnitable fuel) {
        fuelList.Remove(fuel);
        fuelChange = true;
        fuel.SetIgnitor(null);
    }
}

public class Wet : Effect { }
//LASTING AFFECTERS


//TRAIT AFFECTERS
public class Trait : Affecter {
    public override void Enact() { }

    public override void Deact() { }
}
//TRAIT AFFECTERS
