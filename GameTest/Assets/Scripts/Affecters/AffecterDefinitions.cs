using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : Affecter {
    public Fire(Body _targetBody, float _vitality, float _vRate = -1f, float _spreadMod = 0.5f) : base(_targetBody, _vitality, _vRate, _spreadMod) {
        reactorList = new List<Reactor> { new Burning(this, _vitality, Mathf.Infinity, false, true),
                                          new Drying(this, _vitality, Mathf.Infinity, false, true),
                                          new Heating(this, _vitality, Mathf.Infinity, false, true)};

        combinable = true;
        spreadable = true;
        layered = true;
    }

    public override void Dewit() {
        Debug.Log("Burn, baby, burn: " + vitality);
        base.Dewit();
    }

    public override void Deact() {
        base.Deact();
    }
}

public class Water : Affecter {
    public Water(Body _targetBody, float _vitality, float _vRate = -1f, float _spreadMod = 0.25f) : base(_targetBody, _vitality, _vRate, _spreadMod) {
        reactorList = new List<Reactor> { new Dampening(this, _vitality, Mathf.Infinity, false, true),
                                          new Watering(this, _vitality, Mathf.Infinity, false, true)};
        combinable = true;
        spreadable = true;
        layered = true;
    }

    public void Freeze() {
        present = false;
        Ice ice = new Ice(targetBody, vitality);
        int index = targetBody.GetLayerVal(this);
        targetBody.AddAffecter(ice);
        targetBody.AddToLayerList(ice, index);
    }
}

public class Ice : Affecter {
    public Ice(Body _targetBody, float _vitality, float _vRate = -1f, float _spreadMod = 0.25f) : base(_targetBody, _vitality, _vRate, _spreadMod) {
        reactorList = new List<Reactor> { new Dampening(this, _vitality, Mathf.Infinity, false, true),
                                          new Freezing(this, _vitality, Mathf.Infinity, false, true),
                                          new Chilling(this, _vitality, Mathf.Infinity, false, true)
        };
        combinable = true;
        spreadable = false;
        layered = true;
    }


    public override void Dewit() {

        base.Dewit();
    }
}

public class Oil : Affecter {
    public Oil(Body _targetBody, float _vitality, float _vRate = -1f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new Dampening(this, _vitality, Mathf.Infinity, false, true),
                                          new Oiling(this, _vitality, Mathf.Infinity, false, true),
                                          new Fueling(this, _vitality, Mathf.Infinity, false, true)};
        combinable = true;
        spreadable = true;
        layered = true;
    }
}

public class Wound : Affecter {
    protected float threshold = 10f;

    public Wound(Body _targetBody, float _vitality, float _vRate = -1f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new Harming(this, _vitality, Mathf.Infinity, false, true) };

        combinable = false;
        spreadable = false;
        layered = false;
    }

    public override bool Enact(Body _targetBody) {
        bool result = base.Enact(_targetBody);
        if (result) {
            targetBody.ChangeHarm(vitality);
        }
        return result;
    }

    public override void Dewit() {
        if (vitality > threshold)
            vRate = 0;
        else
            vRate = (vitality / threshold) - 1f;
        float delt = vitality - turnVitality;
        targetBody.ChangeHarm(delt);
        base.Dewit();
    }
}

public class ResistanceAggregate : Affecter {
    public ResistanceAggregate(Body _targetBody, float _vitality, float _vRate = 0f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new DamageResist(this, 0, Mathf.NegativeInfinity, true, false) };

        combinable = false;
        spreadable = false;
        layered = false;
        immortal = true;

    }
}

public class Resistance : Affecter {
    public Resistance(Body _targetBody, float _vitality, float _vRate = 0f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new ResistanceAdder(this, _vitality, Mathf.Infinity, false, true) };

        combinable = false;
        spreadable = false;
        layered = false;
    }

    public override void Deact() {
        base.Deact();
    }
}

public class Block : Resistance {
    public Block(Body _targetBody, float _vitality, float _vRate = 0f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new ResistanceAdder(this, _vitality, Mathf.Infinity, false, true) };

        combinable = false;
        spreadable = false;
        layered = false;
    }

    public override bool Enact(Body _targetBody) {
        return base.Enact(_targetBody);
    }
}

public class Reduction : Affecter {  //  ToDo: think clearly about these decisions and reconsider them
    public Reduction(Body _targetBody, float _vitality, float _vRate = 0f) : base(_targetBody, _vitality, _vRate) {
        reactorList = new List<Reactor> { new DamageReduce(this, _vitality, Mathf.Infinity, false, true) };

        combinable = false;
        spreadable = false;
        layered = false;
    }
}