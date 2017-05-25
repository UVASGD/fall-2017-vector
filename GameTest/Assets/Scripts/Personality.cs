using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality : MonoBehaviour {

    // enum Moods { Charm, Disgust, Amuse, Anger, Inspire, Intimidate, Happy, Sad, Hinder, Harm, Heal, Expedite };

    public enum Mood {
        Charm, Amuse, Inspire, Happy,
        Disgust, Anger, Intimidate, Sad,
        None
    };

    float[] quants = { 0, 0, 0, 0 };

    float[] thresholds = { 1, 1, 1, 1,
                           1, 1, 1, 1 };

    float[] mods = { 1, 1, 1, 1,
                     1, 1, 1, 1 };

    float[] cooldowns = { 0.1f, 0.1f, 0.1f, 0.1f,
                          0.1f, 0.1f, 0.1f, 0.1f };

    bool[] redMods = { false, false, false, false,
                       false, false, false, false};

    bool[] newTraits = { false, false, false, false,
                         false, false, false, false};

    Mood dominant;  // This is the dominating mood of the personality, which will be visible to the player

    List<PTrait>[] traits = new List<PTrait>[8];  // An array of eight generic lists of Personality Traits, one list for each mood

    public float GetMod(Mood m) {  // Returns the mod of the given mood
        return mods[(int)m];
    }

    public void ChangeMod(Mood m, float delt) {  // Changes the modifier for a given mood by the given delta
        mods[(int)m] += delt;
    }

    public float GetQuant(Mood m) {
        return quants[(int)m % 4];
    }

    public void ChangeQuant(Mood m, float delt) {  // Changes the quantity of the given mood, interpreting the delta as a magnitudue of change for the negative-defined moods.
        if ((int)m < 4)
            quants[(int)m] += delt;
        else
            quants[(int)m % 4] -= delt;
        //quants[(int)m % 4] += delt * (mods[(int)m]);  <- OLD CODE; REPLACED BY ABOVE
    }

    public void ChangeQuant(Mood m, float delt, float sMod) { // Changes the quantity of the given mood, interpreting the delta as a magnitudue of change for the negative-defined moods.
        if ((int)m < 4)
            quants[(int)m] += delt * sMod;
        else
            quants[(int)m % 4] -= delt * sMod;
        // quants[(int)m % 4] += delt * sMod;  <- OLD CODE; REPLACED BY ABOVE
    }

    public void ActivateMood(Mood m, float quant=0f) {  // 
        int redirectSpot = -1;
        // If the mood has new traits that haven't been checked for redirects yet, perform check
        if (newTraits[(int)m])
            redirectSpot = CheckRedirect(m);
        // If a redirect was found, perform the redirect
        if (redirectSpot != -1)
            traits[(int)m][redirectSpot].Check(m, quant);
        // If a new redirect was not found, then activate all traits
        else
            foreach (PTrait trait in traits[(int)m]) {
                trait.Check(m, quant);  // TODO: Consider passing in the mood being activated to the trait to ensure traits can act on the right mood
                ChangeQuant(m, quant, mods[(int)m]);  // Make Change
            }
        /*  SIMON: I noticed that we never actually changed the quant, so I added the below. Then, I thought it may be useful in ChangeQuant as well, and therefore just called that.
        if ((int)m < 4)
            quants[(int)m] += quant;
        else
            quants[(int)m % 4] -= quant;
        */
        DetermineDominant();  // Check if dominant mood has changed.
    }

    public int CheckRedirect(Mood m) {  // Checks whether mood 'm' has a redirect trait
        int spot = -1;  // Index of redirect trait in traits list
        newTraits[(int)m] = false;  // Unsets dirty bit.
        for (int i = 0; i < traits[(int)m].Count; i++) {  // loops through traits list
            if (!redMods[i])
                continue;  // If the mood is already known to have a redirect, no check is made

            PTrait trait = traits[(int)m][i];
            if (trait.GetType().Equals("Redirect")) {
                redMods[i] = true;  // Flag is raised indicating the mood has a redirect
                return i;  // A redirect trait has been found, so the search is ended
                // spot = i;
            }
        }
        return spot;
    }

    public void Cool() {  // Performs cooldowns on emotions (reducing their magnitude, pos or neg)
        for (int i = 0; i < 4; i++) {
            if (quants[i] > 0)  // If the track is a positive emotion, decrease it by the cooldown
                quants[i] = Mathf.Max(quants[i] - cooldowns[i], 0f);  // Decreases emotion without going below 0
            else if (quants[i] < 0)  // If the track is a negative emotion, increase it by the cooldown
                quants[i] = Mathf.Min(quants[i] + cooldowns[i + i], 0f); // Increases emotion without going above 0
        }
    }

    public void DetermineDominant() {
        Mood newDom = Mood.None;  // Default to no mood
        float highestQuant = 0f;  // track maginitude of highest mood quantity
        for (int i = 0; i < 8; i++) {
            float thisQuant = Mathf.Abs(quants[i]);
            // if the current mood has higher magnitude than the previous highest and is above its thrshold, it is dominant
            if (thisQuant > highestQuant && thisQuant > thresholds[i]) {
                newDom = (Mood) i;
                highestQuant = thisQuant;
            }
        }
        dominant = newDom;
    }

    public Mood GetDominant() {
        return dominant;
    }

    public string domStr() {
        string returnVal = "None";
        switch (dominant) {
            case (Personality.Mood.Charm): returnVal = "Charm"; break;
            case (Personality.Mood.Amuse): returnVal = "Amuse"; break;
            case (Personality.Mood.Inspire): returnVal = "Inspire"; break;
            case (Personality.Mood.Happy): returnVal = "Happy"; break;

            case (Personality.Mood.Disgust): returnVal = "Disgust"; break;
            case (Personality.Mood.Anger): returnVal = "Anger"; break;
            case (Personality.Mood.Intimidate): returnVal = "Intimidate"; break;
            case (Personality.Mood.Sad): returnVal = "Sad"; break;
        }
        return returnVal;
    }

    public void AddTrait(Mood m, PTrait trait) {
        traits[(int)m].Add(trait);
        traits[(int)m][traits[(int)m].Count - 1].person = this;
    }
}

