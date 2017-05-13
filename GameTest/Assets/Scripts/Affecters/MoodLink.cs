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

    public override bool Check(Personality.Mood source, float q) {
        if (base.Check(source, q)) {
            person.ChangeQuant(target, q * mod, sharedMod);
        }
        return true;
    }

}
