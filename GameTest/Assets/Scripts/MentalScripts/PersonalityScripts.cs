//Association class
//Mood class
//List mapping associations to other associations / moods
//special character to denote association with this 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Association {
    string name; //The name that will appear in 'dialogue' strings
    public string Name { get { return name; } }

    string id; //The name that will be used to match with other associations
    public string Id { get { return id; } }

    float interest;
    public float Interest { get { return interest; } }

    public Dictionary<string, float[]> associations; //The assocs this is connected to [0] being polarity, [1] being strength, [2] being exhaustion multiplier
    public Dictionary<string, float[]> marks; //The associations to which this association is 'perma' connected to

    public Association(string _name, string _id, Dictionary<string, float[]> _marks = null) {
        name = _name;
        id = _id;
        marks = _marks;
    }


    public void AddMark(string n, float s) {
    }

    public void AddAssoc(string n, float[] interaction) {
        if (associations.ContainsKey(n)) {
            for (int i = 0; i < associations[n].Length - 1; i++)
                break;
        }
    }
}

public class VerbAssoc : Association {
    string gerund;
    public string Gerund { get { return gerund; } }

    float polarity; //Kindness of the interaction
    public float Polarity { get { return polarity; } }

    public VerbAssoc(string _name, string _id, string _gerund, float _polarity, Dictionary<string, float[]> _marks = null) : 
                     base(_name, _id, _marks) {
        gerund = _gerund;
        polarity = _polarity;
    }

    public override string ToString() {
        return gerund;
    }
}

public class MoodAssoc : Association {
    public MoodAssoc(string _name, string _id, Dictionary<string, float[]> _marks = null) : 
                     base(_name, _id, _marks) {
    }
}

public class PersonAssoc : Association {
    public PersonAssoc(string _name, string _id, Dictionary<string, float[]> _marks = null) :
                       base(_name, _id, _marks) {
    }
}

public class PlaceAssoc : Association {
    public PlaceAssoc(string _name, string _id, Dictionary<string, float[]> _marks = null) :
                      base(_name, _id, _marks) {
    }
}

public class ItemAssoc : Association {
    public ItemAssoc(string _name, string _id, Dictionary<string, float[]> _marks = null) :
                     base(_name, _id, _marks) {
    }
}

public class Personality {
    List<Association> associator;
    Dictionary<string, float> seenEvents;
    Identity identity;

    float markThreshold;
    float objMarkThreshold;
    float interestThreshold;

    public void Perceive(string[] info, float[] interaction) {
        float totalInt = 0;
        float objvbInt = 0;
        float vbInt = 0;
        Association subj = null;
        VerbAssoc vb = null;
        Association obj = null;
        string sentence = "";

        for (int i = 0; i < info.Length; i++) {
            string id = info[i];
            if (id == "")
                continue;
            else {
                sentence += id;
                if (i < (info.Length - 1))
                    sentence += ", ";
            }
            foreach (Association a in associator)
                if (a.Id == id) {
                    switch (i) {
                        case 0: subj = a; totalInt += a.Interest; break;
                        case 1: vb = (VerbAssoc)a; totalInt += a.Interest; objvbInt += a.Interest; break;
                        case 2: obj = a; totalInt += a.Interest; objvbInt += a.Interest; vbInt += a.Interest; break;
                    }
                }
        }

        if (seenEvents.ContainsKey(sentence)) {
            seenEvents[sentence] = Mathf.Min(1, seenEvents[sentence] + vbInt);
            if (seenEvents[sentence] - subj.Interest > markThreshold)
                subj.AddMark(sentence, (seenEvents[sentence] - subj.Interest) / 2);
            if (obj != null) subj.AddAssoc(obj.Name, interaction);
            Feel(sentence, 4);
        }
        else if (totalInt < interestThreshold) {
            seenEvents.Add(sentence, totalInt);
            Feel(sentence, 2);
        }
        else {
            if (objvbInt > markThreshold)
                subj.AddMark(sentence, objvbInt / 2);

        }


    }

    public void Feel(string info, int div, bool opine = false) { }

    public void Feel(string identity) { }

}
/*
// enum Moods { Charm, Disgust, Amuse, Anger, Inspire, Intimidate, Happy, Sad, Hinder, Harm, Heal, Expedite };

    public enum Mood {
        Charm, Amuse, Inspire, Happy,
        Disgust, Anger, Intimidate, Sad,
        None
    };

    float[] quants = { 0, 0, 0, 0 };

    float[] thresholds = {  1,  1,  1,  1,
                           -1, -1, -1, -1 };

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

    public bool IsOverThreshold(Mood m) {
        int moodInt = (int)m;
        float quant = quants[moodInt % 4];
        if (moodInt < 4)
            return quant >= thresholds[moodInt];
        else
            return quant <= thresholds[moodInt];
    }
    }
    */

