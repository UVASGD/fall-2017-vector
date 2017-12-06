using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodHandler {
    public List<Mood> moodList;

    public MoodHandler(List<Mood> _moodList) {
        moodList = _moodList;
    }

    public void ApplyMood(Dictionary<MoodAssoc, float> _feels) {
        foreach (MoodAssoc moodAssoc in _feels.Keys)
            foreach (Mood mood in moodList)
                if (moodAssoc.CMood == mood.CMood)
                    mood.ApplyPolarity(_feels[moodAssoc]);
    }

    public string GetName(MoodAssoc moodAssoc, float pol) {
        foreach (Mood mood in moodList)
            if (moodAssoc.CMood == mood.CMood)
                return (Mathf.Sign(pol) == -1) ? mood.Negative : mood.Positive;
        return "";
    }

    public Mood GetDominantMood() {
        Mood curDom = null;

        foreach (Mood m in moodList) {
            if (curDom == null || Mathf.Abs(m.Polarity) > Mathf.Abs(curDom.Polarity))
                curDom = m;
        }
        return curDom;
    }
}

public class Mood {
    string positive;
    public string Positive { get { return positive; } }
    string positiveId;
    public string PositiveId { get { return positiveId; } }

    string negative;
    public string Negative { get { return negative; } }
    string negativeId;
    public string NegativeId { get { return negativeId; } }

    float polarity;
    public float Polarity { get { return polarity; } }

    CoreMood cMood;
    public CoreMood CMood { get { return cMood; } }

    public Mood(string _positive, string _positiveId, string _negative, string _negativeId, float _polarity, CoreMood _cMood) {
        positive = _positive;
        positiveId = _positiveId;
        negative = _negative;
        negativeId = _negativeId;
        polarity = _polarity;
        cMood = _cMood;
    }

    public void ApplyPolarity(float _polarity) {
        polarity += _polarity;
    }

    public override string ToString() {
        string returnString = "";
        if (polarity > 0)
            returnString = positive;
        else returnString = negative;
        return returnString;
    }

    public string GetId() {
        string returnString = "";
        if (polarity > 0)
            returnString = positiveId;
        else returnString = negativeId;
        return returnString;
    }
}