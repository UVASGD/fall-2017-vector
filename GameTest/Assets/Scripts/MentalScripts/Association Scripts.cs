﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Association {
    string name; //The name that will appear in 'dialogue' strings
    public string Name { get { return name; } }

    string id; //The name that will be used to match with other associations
    public string Id { get { return id; } }

    float interest;
    public float Interest { get { return interest; } }

    float accesses;
    public float Accesses { set { accesses = value; } get { return accesses; } }

    bool permanent;
    public bool Permanent { get { return permanent; } }

    public short deletable;

    float checks;
    public float Checks { get { return checks; } set { checks = value; } }

    public Dictionary<Association, Interaction> associations; //The assocs this is connected to [0] being polarity, [1] being strength
    public Dictionary<string, Interaction> addToMarks; //This is a temporary list of strings to association values
    public Dictionary<Association, Interaction> marks; //The associations to which this association is 'perma' connected to, [2] being exhaustion multiplier

    public Association(string _name, string _id, Dictionary<string, Interaction> _marks = null, bool _perm = false, short _delet = 0) {
        name = _name;
        id = _id;
        addToMarks = _marks;
        permanent = _perm;
        deletable = _delet;
        accesses = 0;
        checks = 0;
    }

    public Interaction CheckAssoc(Association obj, Interaction interaction) {
        float[] thresholds = new float[] { 0.3f, 0.5f, 0.7f, 0.9f };
        Interaction retInt = new Interaction(interaction.Polarity, 0);
        for (int i = thresholds.Length; i > 0; i--) {
            bool pastMark = associations[obj].Strength <= thresholds[i];
            float tempAdd = Mathf.Clamp(associations[obj].Strength + interaction.Strength, 0, 1);
            if (pastMark && tempAdd > thresholds[i]) {
                retInt.Apply(strengthDelt: (tempAdd / 4));
                break;
            }
        }
        associations[obj].Apply(interaction.Polarity, interaction.Strength);
        return retInt;
    }

    public void GetMood(Dictionary<MoodAssoc, float> feels, List<Association> branch, float percent, float div, int acc) {
        accesses += div;
        acc++;
        branch.Add(this);
        foreach (Association mark in marks.Keys) {
            if (branch.Contains(mark) || accesses > marks[mark].Capacity)
                continue;
            if (mark.GetType() == typeof(MoodAssoc)) {
                if (!feels.ContainsKey((MoodAssoc)mark))
                    feels.Add((MoodAssoc)mark, percent * marks[mark].Strength * Mathf.Sign(marks[mark].Polarity));
                else { feels[(MoodAssoc)mark] += percent * Mathf.Sign(marks[mark].Polarity); }
            }
            if (acc < 5 && marks[mark].Strength > (acc * 5)) {
                float newStr = marks[mark].CalibrateStrength();
                newStr *= Mathf.Sign(marks[mark].Polarity);
                mark.GetMood(feels, branch, newStr * percent, div, acc);
            }
        }
        acc--;
        branch.RemoveAt(acc);
    }

    public void GetMarks(Association subj, Association obj, List<Association> branch, Interaction markInteract, int acc = 0) {
        acc++;
        branch.Add(this);
        foreach (Association mark in obj.marks.Keys) {
            Interaction interact = obj.marks[mark];
            if (mark.GetType() == typeof(MoodAssoc) || mark.GetType() == typeof(ConceptAssoc))
                subj.AddMark(subj, mark, markInteract.Polarity, interact.Strength * markInteract.Strength);
            GetMarks(subj, mark, branch, subj.marks[mark], acc);
        }
        acc--;
        branch.RemoveAt(acc);
    }

    public void AddMark(Association subj, Association newMark, float intPolarity, float intStrength) {
        if (subj.marks.ContainsKey(newMark))
            subj.marks[newMark].Apply(polarityDelt: intPolarity, strengthDelt: intStrength);
        subj.marks.Add(newMark, new Interaction(intPolarity, intStrength));
        subj.interest += intStrength / 2;
    }

    public void DelMark(Association subj, Association targetMark, float polarityDelt, float strengthDelt) {
        if (subj.marks.ContainsKey(targetMark)) {
            subj.marks[targetMark].Apply(polarityDelt, strengthDelt);
        }
    }

    public void Sensitize(float delt) {
        foreach (Association a in marks.Keys) {
            marks[a].Apply(strengthDelt: delt);
        }
    }

    public void AdjustInterest(float interestDelt) {
        interest += interestDelt;
        if (interest < 0)
            interest = 0;
    }
}

public class VerbAssoc : Association {
    string gerund;
    public string Gerund { get { return gerund; } }

    float polarity; //Kindness of the interaction
    public float Polarity { get { return polarity; } }

    public VerbAssoc(string _name, string _id, string _gerund, float _polarity, Dictionary<string, Interaction> _marks = null) :
                     base(_name, _id, _marks) {
        gerund = _gerund;
        polarity = _polarity;
    }

    public override string ToString() {
        return gerund;
    }
}

public class MoodAssoc : Association {

    float obl;
    public float Obl { get { return obl; } }

    CoreMood cMood;
    public CoreMood CMood { get { return cMood; } }

    public MoodAssoc(string _name, string _id, float _obl, Dictionary<string, Interaction> _marks = null) :
                     base(_name, _id, _marks) {
        obl = _obl;
    }
}

public class ConceptAssoc : Association {
    public ConceptAssoc(string _name, string _id, Dictionary<string, Interaction> _marks = null) :
                       base(_name, _id, _marks) {
    }
}


public class PersonAssoc : Association {
    float obligation;

    public PersonAssoc(string _name, string _id, float _obligation, Dictionary<string, Interaction> _marks = null) :
                       base(_name, _id, _marks) {
        obligation = _obligation;
    }

    public void ApplyObl(float oblDelt) {
        obligation += oblDelt;
    }
}

public class PlaceAssoc : Association {
    public PlaceAssoc(string _name, string _id, Dictionary<string, Interaction> _marks = null) :
                      base(_name, _id, _marks) {
    }
}

public class ItemAssoc : Association {
    public ItemAssoc(string _name, string _id, Dictionary<string, Interaction> _marks = null) :
                     base(_name, _id, _marks) {
    }
}

public class PanAssoc : Association {
    float delt;
    public float Delt { get { return delt; } }

    public PanAssoc(string _name, string _id, float _delt, Dictionary<string, Interaction> _marks = null) :
                     base(_name, _id, _marks) {
        delt = _delt;
    }
}

public struct Interaction {
    float capacity;
    public float Capacity { get { return capacity; } set { capacity = value; } }

    float polarity;
    public float Polarity { get { return polarity; } }

    float strength;
    public float Strength { get { return strength; } }

    public Interaction(float _polarity, float _strength, int _capacity = 0) {
        polarity = _polarity;
        strength = _strength;
        capacity = _capacity;
    }

    public void Apply(float polarityDelt = 0, float strengthDelt = 0, float capacityDelt = 0) {
        float absPolarityDelt = Mathf.Abs(polarityDelt) * Mathf.Abs(polarity);
        polarity += ((Mathf.Sign(polarity) + Mathf.Sign(polarityDelt) == 0)) ? -polarityDelt : polarity;
        strength += strengthDelt;
        strength = (strength < 0) ? 0 : strength;
        capacity += capacityDelt;
    }

    public void Set(float polaritySet = 0, float strengthSet = 0, float capacitySet = 0) {
        polarity = polaritySet;
        strength = strengthSet;
        capacity = capacitySet;
    }

    public float CalibrateStrength() {
        float newStr = (strength > (Mathf.Abs(polarity) * 1.5))
            ? (strength + Mathf.Abs(polarity)) / 2
            : strength;
        Set(strengthSet: newStr);

        return newStr;
    }
}

public struct EventInfo {
    float interest;
    public float Interest { get { return interest; } }

    float polarity;
    public float Polarity { get { return polarity; } }

    float strength;
    public float Strength { get { return strength; } }

    int accesses;
    public int Accesses { get { return accesses; } }

    string[] contexts;
    public string[] Contexts { get { return contexts; } }

    public EventInfo(float _interest, float _polarity, float _strength, string[] _contexts, int _accesses = 1) {
        polarity = _polarity;
        strength = _strength;
        interest = _interest;
        contexts = _contexts;
        accesses = _accesses;
    }

    public void Apply(float interestDelt = 0, float polarityDelt = 0, float strengthDelt = 0, int accessesDelt = 0) {
        interest += interestDelt;
        polarity += ((Mathf.Sign(polarity) + Mathf.Sign(polarityDelt) == 0)) ? -polarityDelt : polarity;
        strength += strengthDelt;
        strength = (strength < 0) ? 0 : strength;
        accesses += accessesDelt;
    }

    public void Set(float interestSet = 0, float polaritySet = 0, float strengthSet = 0, int accessesSet = 0) {
        interest = interestSet;
        polarity = polaritySet;
        strength = strengthSet;
        accesses = accessesSet;
    }
}