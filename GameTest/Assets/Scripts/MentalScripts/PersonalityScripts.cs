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
        float absPolarityDelt = Mathf.Abs(polarityDelt) * Mathf.Abs(polarity);
        polarity += ((Mathf.Sign(polarity) + Mathf.Sign(polarityDelt) == 0)) ? -polarityDelt : polarity;
        strength += strengthDelt;
        capacity += capacityDelt;
    }

    public void Set(float polaritySet = 0, float strengthSet = 0, float capacitySet = 0) {
        polarity = polaritySet;
        strength = strengthSet;
        capacity = capacitySet;
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



    public Dictionary<Association, Interaction> associations; //The assocs this is connected to [0] being polarity, [1] being strength
    public Dictionary<string, Interaction> addToMarks; //This is a temporary list of strings to association values
    public Dictionary<Association, Interaction> marks; //The associations to which this association is 'perma' connected to, [2] being exhaustion multiplier

    public Association(string _name, string _id, Dictionary<string, Interaction> _marks = null) {
        name = _name;
        id = _id;
        addToMarks = _marks;
        accesses = 0;
    }

    public Interaction CheckAssoc(Association obj, Interaction interaction) {
        float[] thresholds  =new float[]{ 0.3f, 0.5f, 0.7f, 0.9f };
        Interaction retInt = new Interaction(interaction.Polarity, 0);
        for (int i = thresholds.Length; i > 0; i--)
        {
            bool pastMark = associations[obj].Strength <= thresholds[i];
            float tempAdd = Mathf.Clamp(associations[obj].Strength + interaction.Strength,0,1);
            if(pastMark && tempAdd > thresholds[i])
            {
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
                float newStr = (marks[mark].Strength > Mathf.Abs(marks[mark].Polarity)) 
                    ? (marks[mark].Strength + Mathf.Abs(marks[mark].Polarity)) / 2 
                    : marks[mark].Strength;
                newStr *= Mathf.Sign(marks[mark].Polarity);
                marks[mark].Set(strengthSet: newStr);
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
            if (mark.GetType() == typeof(MoodAssoc) || mark.GetType() == typeof(ConceptAssoc)) {
                if (subj.marks.ContainsKey(mark))
                    subj.marks[mark].Apply(polarityDelt: markInteract.Polarity, strengthDelt: interact.Strength * markInteract.Strength);
                else
                    subj.marks.Add(mark, new Interaction(markInteract.Polarity, interact.Strength * markInteract.Strength));
            }

            GetMarks(subj, mark, branch, subj.marks[mark], acc);
        }
        acc--;
        branch.RemoveAt(acc);
    }
    //AddMark(subj, newMarks, subj.GetMarks(obj, newMarks, branch, 0.25f));
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
        polarity += ((Mathf.Sign(polarity) + Mathf.Sign(polarityDelt) == 0)) ? -polarityDelt : polarity;
        strength += strengthDelt;
        accesses += accessesDelt;
    }

    public void Set(float interestSet = 0, float polaritySet = 0, float strengthSet = 0, int accessesSet = 0) {
        interest = interestSet;
        polarity = polaritySet;
        strength = strengthSet;
        accesses = accessesSet;
    }
}

public class Personality {
    List<Association> associator;
    Dictionary<string[], EventInfo> seenEvents; //interest[0], interaction polarity [1], interaction [2], # times [3]
    Identity identity;
    MoodHandler moodHandler;

    float markThreshold;
    float objMarkThreshold;
    float interestThreshold;

    float div = 1;

    public Personality() { }

    public Personality(List<Association> _associator, Identity _identity, MoodHandler _moodHandler, bool t = false) {
        associator = _associator;
        identity = _identity;
        moodHandler = _moodHandler;
        seenEvents = new Dictionary<string[], EventInfo>();
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

        float totalInterest = 0;

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
            totalInterest += vb.Interest;
        }
        else {
            totalInterest += subj.Interest + vb.Interest + ((obj != null) ? obj.Interest : 0);
            seenEvents.Add(info, new EventInfo(totalInterest, interaction.Polarity, interaction.Strength));
            div = (totalInterest > interestThreshold) ? 1 : 0.25f;
        }
        totalInterest *= div;

        if (interaction.Strength == 0) {
            interaction.Apply(polarityDelt: vb.Polarity, strengthDelt: vb.Interest);
        }
        else { interaction.Set(strengthSet: ((interaction.Strength + totalInterest) / 2)); }
        interaction.Set(polaritySet: (interaction.Polarity * div), strengthSet: (interaction.Strength * div));
        float newStr = (interaction.Strength > Mathf.Abs(interaction.Polarity))
            ? (interaction.Strength + Mathf.Abs(interaction.Polarity)) / 2
            : interaction.Strength;
        interaction.Set(strengthSet: newStr);

        Feel(subj, obj, vb, totalInterest, interaction, div);
    }

    public string Feel(Association subj, Association obj, Association vb, float interest, Interaction interaction, float div, bool opine = false)
    {
        Dictionary<MoodAssoc, float> feels = new Dictionary<MoodAssoc, float>(); //Apply this to the perceiver's moods
        List<Association> branch = new List<Association>();

        subj.GetMood(feels, branch, interest, div, 0);

        vb.GetMood(feels, branch, interest, div, 0);

        if (obj != null)
        {
            obj.GetMood(feels, branch, interaction.Strength * Mathf.Sign(interaction.Polarity), div, 0);
        }


        if (opine) {
            float topVal = 0;
            MoodAssoc topMood = null;
            foreach (MoodAssoc m in feels.Keys) {
                if (Mathf.Sign(feels[m]) > Mathf.Sign(topVal)) {
                    topVal = feels[m];
                    topMood = m;
                }
            }
            //If mood != null
            //MoodHandler.GetMoodName(topMood, topVal)
        }
        //APPLY MOODS HERE
        else {
            Dictionary<Association, Interaction> newMarks = new Dictionary<Association, Interaction>();

            if (obj != null) {
                Interaction checkInt = subj.CheckAssoc(obj, interaction);
                subj.GetMarks(subj, obj, branch, checkInt);
            }
            subj.GetMarks(subj, vb, branch, subj.CheckAssoc(vb, new Interaction(1, vb.Interest)));
        }

        return "";
    }

    /*TODO -
     * Mood class needs to exemplify bipartite behavior
     * Feel has to apply feels in order to express actual moods
     */

    public void Feel(Association _concept) { }

}

public class MoodHandler {
    List<Mood> moodList;

    public MoodHandler(List<Mood> _moodList) {
        moodList = _moodList;
    }

    public void ApplyMood(Dictionary<MoodAssoc, float> _feels) {
        foreach (MoodAssoc moodAssoc in _feels.Keys)
            foreach (Mood mood in moodList)
                if (moodAssoc.CMood == mood.CMood)
                    mood.ApplyPolarity(_feels[moodAssoc]);
    }
}

public class Mood {
    string positive;
    public string Positive { get { return positive; } }

    string negative;
    public string Negative { get { return negative; } }

    float polarity;

    CoreMood cMood;
    public CoreMood CMood { get { return cMood; } } 

    public Mood(string _positive, string _negative, float _polarity, CoreMood _cMood) {
        positive = _positive;
        negative = _negative;
        polarity = _polarity;
        cMood = _cMood;
    }

    public void ApplyPolarity(float _polarity) {
        polarity += _polarity;
    }
}