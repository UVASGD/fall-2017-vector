//Association class
//Mood class
//List mapping associations to other associations / moods
//special character to denote association with this 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        polarity += polarityDelt;
        strength += strengthDelt;
        capacity += capacityDelt;
    }
}

public class Association {
    string name; //The name that will appear in 'dialogue' strings
    public string Name { get { return name; } }

    string id; //The name that will be used to match with other associations
    public string Id { get { return id; } }

    float interest;
    public float Interest { get { return interest; } }

    float accesses;
    public float Accesses { set { accesses = value; } get { return accesses; } } 

    public Dictionary<string, Interaction> associations; //The assocs this is connected to [0] being polarity, [1] being strength
    public Dictionary<string, Interaction> addToMarks; //This is a temporary list of strings to association values
    public Dictionary<Association, Interaction> marks; //The associations to which this association is 'perma' connected to, [2] being exhaustion multiplier

    public Association(string _name, string _id, Dictionary<string, Interaction> _marks = null) {
        name = _name;
        id = _id;
        addToMarks = _marks;
        accesses = 0;
    }


    public void AddMark(string n, float s) {
    }

    public void AddAssoc(string n, float[] interaction) {
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
    public MoodAssoc(string _name, string _id, Dictionary<string, Interaction> _marks = null) : 
                     base(_name, _id, _marks) {
    }
}

public class PersonAssoc : Association {
    public PersonAssoc(string _name, string _id, Dictionary<string, Interaction> _marks = null) :
                       base(_name, _id, _marks) {
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

public struct EventInfo {
    float interest;
    public float Interest { get { return interest; } }

    float polarity;
    public float Polarity { get { return polarity; } }

    float strength;
    public float Strength { get { return strength; } }

    int accesses;
    public int Accesses { get { return accesses; } }

    public EventInfo(float _interest, float _polarity, float _strength, int _accesses = 1) {
        polarity = _polarity;
        strength = _strength;
        interest = _interest;
        accesses = _accesses;
    }

    public void Apply(float interestDelt = 0, float polarityDelt = 0, float strengthDelt = 0, int accessesDelt = 0) {
        interest += interestDelt;
        polarity += polarityDelt;
        strength += strengthDelt;
        accesses += accessesDelt;
    }
}

public class Personality {
    List<Association> associator;
    Dictionary<string[], EventInfo> seenEvents; //interest[0], interaction polarity [1], interaction [2], # times [3]
    Identity identity;

    float totalInterest;

    float markThreshold;
    float objMarkThreshold;
    float interestThreshold;

    float div = 1;

    public Personality() {
        foreach (Association a in associator)
            foreach (string s in a.addToMarks.Keys)
                foreach (Association aOther in associator)
                    if (aOther.Id.Equals(s))
                        a.marks.Add(aOther, a.addToMarks[s]);
    }

    public void Perceive(string[] info, Interaction interaction) {

        Association subj = null;
        VerbAssoc vb = null;
        Association obj = null;

        foreach (Association a in associator) { //Assign subject, verb, and object accordingly
            if (info.Length == 3) {
                if (a.Id.Equals(info[0])) subj = a;
                else if (a.Id.Equals(info[1])) vb = (VerbAssoc)a;
                else if (a.Id.Equals(info[2])) obj = a;
            }
            else if (info.Length == 2) {
                if (a.Id.Equals(info[0])) subj = a;
                else if (a.Id.Equals(info[1])) vb = (VerbAssoc)a;
            }
        }

        //Logic in here to detect whether subj, vb, or obj are still null!

        if (seenEvents.ContainsKey(info)) {
            div = 0.25f;
            seenEvents[info].Apply(interestDelt: vb.Interest);
        }

        else {
            totalInterest += subj.Interest + vb.Interest + ((obj != null) ? obj.Interest : 0);
            seenEvents.Add(info, new EventInfo(totalInterest, interaction.Polarity, interaction.Strength));
            div = (totalInterest > interestThreshold) ? 1 : 0.25f;
        }

        Feel(subj, obj, vb, div);
    }

    public void Feel(Association _subj, Association _obj, Association _vb, float div, bool opine = false) { }

    public void Feel(Association _concept) { }

}