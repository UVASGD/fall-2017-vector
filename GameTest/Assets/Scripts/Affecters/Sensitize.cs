using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: rethink how target is defined; Must ensure a sensitization both can be unconditioned,
//  and will take effect when the mood the modifier of which it edits is called

public class Sensitize : PTrait {

    float increment;
    bool activated;

    float defLow;
    float defHig;

    float actualDelt;

    // bool leftOver;

    public Sensitize(Personality.Mood trg, float inc, float t, float dLow = 0, float dHig = 2) {
        target = trg;  // target, here, is the mood the sensitization is conditioned by. Can be None
        increment = inc;
        actualDelt = inc;
        threshold = t;
        activated = false;
        // leftOver = false;
        defLow = dLow;
        defHig = dHig;
    }

    public override bool Check(Personality.Mood source, float quant) {
        bool over = base.Check(source, quant);  // Check if the threshold is surpassed
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
