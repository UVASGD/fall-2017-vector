using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensitize : PTrait {

    float increment;
    bool activated;

    float defLow;
    float defHig;

    float actualDelt;

    bool leftOver;

    public Sensitize(Personality.Mood trg, float inc, float t, float dLow = 0, float dHig = 2) {
        target = trg;
        increment = inc;
        actualDelt = inc;
        threshold = t;
        activated = false;
        leftOver = false;
        defLow = dLow;
        defHig = dHig;
    }

    public override bool Check(float quant) {
        bool over = base.Check(quant);
        if (!activated && over) {
            activated = Enact();
        }
        else if (!over && activated) {
            Deact();
            activated = false;
        }
        return true;
    }

    public void Deact() {
        if (actualDelt != increment) {
            person.ChangeMod(target, -actualDelt);
        }
        else {
            person.ChangeMod(target, -increment);
        }
    }

    new public bool Enact() {
        float personMod = person.GetMod(target);
        if (personMod > defHig || personMod < defLow) {
            return false;
        }

        float total = personMod + increment;
        if (total > defHig) {
            actualDelt = defHig - personMod;
        }
        else if (total < defLow) {
            actualDelt = defLow - personMod;
        }
        else { actualDelt = increment; } 
        person.ChangeMod(target, actualDelt);
        return true;
    }
}
