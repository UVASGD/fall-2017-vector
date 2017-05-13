using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTrait : MoodAffecter {

    public Personality person;

    protected Personality.Mood target;

    protected float mod;

    protected float threshold;

    public virtual void Instance(Personality.Mood trg, float m, float t) {
        target = trg;
        threshold = t;
        mod = m;
    }

    public virtual bool Check(float quant) {

        return Mathf.Abs(quant) > Mathf.Abs(threshold);
    }

    public virtual void Enact() { }
}
