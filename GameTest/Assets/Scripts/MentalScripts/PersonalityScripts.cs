using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality {
    List<Association> associator;
    List<Association> activeAssocs;
    Dictionary<string[], EventInfo> seenEvents; //interest[0], interaction polarity [1], interaction [2], # times [3]
    Dictionary<string[], EventInfo> unseenEvents;
    Identity identity;
    MoodHandler moodHandler;
    List<Context> allContexts;

    float markThreshold;
    float objMarkThreshold;
    float interestThreshold = 60;

    float div = 1;

    public Personality() { }

    public Personality(List<Association> _associator, Identity _identity, MoodHandler _moodHandler, bool t = false) {
        associator = _associator;
        identity = _identity;
        moodHandler = _moodHandler;
        seenEvents = new Dictionary<string[], EventInfo>();
        unseenEvents = new Dictionary<string[], EventInfo>();
        foreach (Association a in associator)
            foreach (string s in a.addToMarks.Keys)
                foreach (Association aOther in associator)
                    if (aOther.Id.Equals(s))
                        a.marks.Add(aOther, a.addToMarks[s]);
    }

    public void CoolDown() {
        //Reduce obligation to people
        foreach (Mood mood in moodHandler.moodList) {
            if (mood.Polarity > 10) mood.ApplyPolarity(-0.01f);
            else if (mood.Polarity < -10) mood.ApplyPolarity(0.01f);
        }
        for (int i = 0; i < activeAssocs.Count; i++) {
            Association a = activeAssocs[i];
            a.Checks++;
            if (a.Checks > 50) {
                if (a.GetType() == typeof(PersonAssoc))
                    ((PersonAssoc)a).ApplyObl(-0.05f);
                foreach (Association mark in a.marks.Keys)
                    a.DelMark(a, mark, -0.5f, -0.5f);
                a.Accesses--;
                if (a.Accesses < 0) {
                    a.Accesses = 0;
                    activeAssocs.Remove(a);
                }
                a.Checks = 0;
            }
        }
    }

    public void Perceive(string[] info, Interaction interaction, string[] contextNames) {
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

        string infoSentence = string.Join(" ", info);
        foreach (string[] sentenceList in seenEvents.Keys) {
            string sentence = string.Join(" ", sentenceList);
            if (sentence.Equals(infoSentence)) {
                div = 0.25f;
                seenEvents[sentenceList].Apply(interestDelt: vb.Interest);
                totalInterest += vb.Interest;
            }
            else {
                totalInterest += subj.Interest + vb.Interest + ((obj != null) ? obj.Interest : 0);
                seenEvents.Add(info, new EventInfo(totalInterest, interaction.Polarity, interaction.Strength, contextNames));
                div = (totalInterest > interestThreshold) ? 1 : 0.25f;
            }
            totalInterest *= div;

            if (interaction.Strength == 0) {
                interaction.Apply(polarityDelt: vb.Polarity, strengthDelt: vb.Interest);
            }
            else { interaction.Set(strengthSet: ((interaction.Strength + totalInterest) / 2)); }
            interaction.Set(polaritySet: (interaction.Polarity * div), strengthSet: (interaction.Strength * div));
            interaction.CalibrateStrength();
            List<string> feelContexts = new List<string>();
            foreach (string s in contextNames)
                feelContexts.Add(s);
            Feel(subj, obj, vb, totalInterest, interaction, div, feelContexts);
        }
    }

    public void ApplyContext(Context con) {
        con.Active = true;
        foreach (AssocAffecter aff in con.assocAffecters) {
            aff.targetAssoc.AdjustInterest(aff.InterestDelt);
            aff.targetAssoc.deletable++;
            foreach (Association mark in aff.markAddList.Keys) {
                if (mark.GetType() != typeof(PanAssoc))
                    aff.targetAssoc.AddMark(aff.targetAssoc, mark, aff.markAddList[mark].Polarity, aff.markAddList[mark].Strength);
                else aff.targetAssoc.Sensitize(((PanAssoc)mark).Delt);
            }
        }
    }

    public void UnapplyContext(Context con) {
        con.Active = false;
        foreach (AssocAffecter aff in con.assocAffecters) {
            aff.targetAssoc.AdjustInterest(-aff.InterestDelt);
            aff.targetAssoc.deletable--;
            foreach (Association mark in aff.markAddList.Keys) {
                if (mark.GetType() != typeof(PanAssoc))
                    aff.targetAssoc.DelMark(aff.targetAssoc, mark, aff.markAddList[mark].Polarity, aff.markAddList[mark].Strength);
                else aff.targetAssoc.Sensitize(-((PanAssoc)mark).Delt);
            }
        }
    }

    public string Feel(Association subj, Association obj, Association vb, float interest, Interaction interaction, float div, 
        List<string> contextNames = null, bool opine = false) {

        List<Context> activeContexts = new List<Context>();
        if (contextNames != null)
            for (int i = 0; i < contextNames.Count; i++)
                foreach (Context con in allContexts)
                    if (contextNames[i].Equals(con.Name)) {
                        if (con.Active)
                            contextNames.Remove(contextNames[i]);
                        else {
                            ApplyContext(con);
                            activeContexts.Add(con);
                        }
                    }

        string returnSentence = "";
        Dictionary<MoodAssoc, float> feels = new Dictionary<MoodAssoc, float>(); //Apply this to the perceiver's moods
        List<Association> branch = new List<Association>();

        subj.GetMood(feels, branch, interest, div, 0);

        vb.GetMood(feels, branch, interest, div, 0);

        if (obj != null)
            obj.GetMood(feels, branch, interaction.Strength * Mathf.Sign(interaction.Polarity), div, 0);

        if (opine) {
            float topVal = 0;
            MoodAssoc topMood = null;
            foreach (MoodAssoc m in feels.Keys) {
                if (Mathf.Abs(feels[m]) > Mathf.Abs(topVal)) {
                    topVal = feels[m];
                    topMood = m;
                }
            }
            if (topMood != null)
                returnSentence = moodHandler.GetName(topMood, topVal);
        }
        //APPLY MOODS HERE
        else {
            moodHandler.ApplyMood(feels);
            foreach (MoodAssoc moodAssoc in feels.Keys)
                if (subj.GetType() == typeof(PersonAssoc))
                    ((PersonAssoc)subj).ApplyObl(moodAssoc.Obl * feels[moodAssoc]);
            if (obj != null) {
                Interaction checkInt = subj.CheckAssoc(obj, interaction);
                subj.GetMarks(subj, obj, branch, checkInt);
            }
            subj.GetMarks(subj, vb, branch, subj.CheckAssoc(vb, new Interaction(1, vb.Interest)));
            if (!activeAssocs.Contains(subj))
                activeAssocs.Add(subj);
        }

        foreach (Context con in activeContexts) {
            UnapplyContext(con);
        }

        return returnSentence;
    }

    public void Feel(Association _concept) { }

}