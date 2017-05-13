using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodLink : PTrait {

    float sharedMod;

    public MoodLink(Personality.Mood trg, float m, float sMod, float t) {
        target = trg;
        mod = m;
        sharedMod = sMod;
        threshold = t;
    }

    public override bool Check(float q) {
        if (base.Check(q)) {
            person.ChangeQuant(target, q * mod, sharedMod);
        }
        return true;
    }

}
