using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirect : PTrait {

    public Redirect(Personality.Mood trg, float m, float t) {
        target = trg;
        mod = m;
        threshold = t;
    }

    public override bool Check(Personality.Mood source, float q) {
        if (base.Check(source, q)) {
            person.ChangeQuant(target, q * mod);
        }
        return true;
    }
}

