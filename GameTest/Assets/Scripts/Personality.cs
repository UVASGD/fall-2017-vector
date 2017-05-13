using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour {

    // enum Moods { Charm, Disgust, Amuse, Anger, Inspire, Intimidate, Happy, Sad, Hinder, Harm, Heal, Expedite };

    int numMoods = 4;

    public enum Mood {
        Charm, Amuse, Inspire, Happy,
        Disgust, Anger, Intimidate, Sad
    };

    float[] quants = { 0, 0, 0, 0 };

    float[] thresholds = { 1,  1,  1,  1,
                          -1, -1, -1, -1 };

    float[] mods = { 1, 1, 1, 1,
                     1, 1, 1, 1 };

    bool[] redMods = { false, false, false, false,
                       false, false, false, false};

    bool[] newTraits = { false, false, false, false,
                         false, false, false, false};

    List<PTrait>[] traits = new List<PTrait>[8];

    public float GetMod(Mood m) {
        return mods[(int)m];
    }

    public void ChangeMod(Mood m, float delt) {
        mods[(int)m] += delt;
    }

    public void ChangeQuant(Mood m, float delt) {
        quants[(int)m] += delt * (mods[(int)m]);
    }

    public void ChangeQuant(Mood m, float delt, float sMod) {
        quants[(int)m] += delt * sMod;
    }

    public void ActivateMood(Mood m, float quant) {
        int redirectSpot = -1;
        if (newTraits[(int)m])
            redirectSpot = CheckRedirect(m);
        if (redirectSpot != -1)
            traits[(int)m][redirectSpot].Check(quant);
        else
            foreach (PTrait trait in traits[(int)m])
                trait.Check(quant);
    }

    public int CheckRedirect(Mood m) {
        int spot = -1;
        newTraits[(int)m] = false;
        for (int i = 0; i < traits[(int)m].Count; i++) {
            if (!redMods[i]) {
                PTrait trait = traits[(int)m][i];
                if (trait.GetType().Equals("Redirect")) {
                    redMods[i] = true;
                    spot = i;
                }
            }
        }
        return spot;
    }

}

