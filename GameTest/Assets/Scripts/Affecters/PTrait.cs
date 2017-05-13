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

    public virtual bool Check(Personality.Mood source, float quant) {
        if ((int)source < 4)
            return person.GetQuant(source) + quant > threshold;
        else
            return person.GetQuant(source) - quant < - threshold;
        //return Mathf.Abs(quant) > Mathf.Abs(threshold); <- OLD CODE;
        // I changed this because it was returning whether the amount being
        // Added to the source mood was over the threshold, and not whether
        // the source mood would be over the threshold after the addition.
        // Also, the check needs to differentiate between the positive and
        // negative states of each mood, such that e.g. a sensitization
        // that triggers due to great sadness doesn't end up triggering if
        // somebody who was really happy is shown something sad.
        // (the absolute value of the extreme happiness even after the
        // subtraction due to the new sadness may still be greater than
        // the sadness threshold for the sensitization, which would be
        // checked because the sadness mood was activated. In this case
        // adding sadness to a very happy person would trigger a sadness-
        // conditioned trait in the old system)
    }

    public virtual void Enact() { }
}
