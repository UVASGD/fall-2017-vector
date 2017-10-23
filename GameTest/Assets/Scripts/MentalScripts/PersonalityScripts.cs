using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personality {
    List<Association> associator;
    List<Association> activeAssocs;
    public Dictionary<string[], EventInfo> seenEvents; //interest[0], interaction polarity [1], interaction [2], # times [3]
    public Dictionary<string[], EventInfo> unseenEvents;
    Identity identity;
    public MoodHandler moodHandler;
    List<Context> allContexts;
    Body body;
    public Body GetBody { get { return body; } }

    string openingText;
    public string OpeningText { get { return openingText; } }
    List<string> smallTalk;
    List<string> usedSpeech;

    float markThreshold;
    float objMarkThreshold;
    float interestThreshold = .60f;

    float div = 1;

    public Personality(Body _body = null) {
        body = _body;
        openingText = "AAA A RIDICULOUS FOOL";
    }

    public Personality(Body _body, List<Association> _associator, Identity _identity, MoodHandler _moodHandler, string _openingText, List<string> _smallTalk,
        List<Context> _allContexts = null) {
        body = _body;
        associator = _associator ?? new List<Association>() { };
        activeAssocs = new List<Association>() { };
        identity = _identity;
        moodHandler = _moodHandler;
        seenEvents = new Dictionary<string[], EventInfo>() { };
        unseenEvents = new Dictionary<string[], EventInfo>() { };
        allContexts = _allContexts ?? new List<Context>() { new Context("Middleburg", new AssocAffecter[] { }) };
        for (int i = 0; i < associator.Count; i++)
            foreach (string s in associator[i].addToMarks.Keys)
                for (int other = 0; other < associator.Count; other++) {
                    if (associator[other].Id.Equals(s)) {
                        associator[i].marks.Add(associator[other], associator[i].addToMarks[s]);
                    }
                }
        openingText = _openingText;
        smallTalk = _smallTalk ?? new List<string>() { "..." };
        usedSpeech = new List<string>() { };
    }

    public string ShuffleOpening() {
        if (smallTalk.Count == 0) {
            foreach (string s in usedSpeech) {
                smallTalk.Add(s);
            }
            usedSpeech = new List<string>();
        }
        int rando = Random.Range(0, smallTalk.Count);
        openingText = smallTalk[rando];
        usedSpeech.Add(smallTalk[rando]);
        smallTalk.RemoveAt(rando);
        if (openingText.Equals("AHHH, A BEAR!")) {
            PersonCreator Bear = new PersonCreator("RBear", "Bear", "Bear", "bear", -15.5f, 2, AINum.turret, null);
            Debug.Log("AAAAAA A BEAR!");
            new EventSpawn(body.transform.position, new Interaction(0, 0), null, "bear", "brawls");
        }
        return openingText;
    }

    public void Tick() {
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

    public bool HasContext(string conName) { 
        foreach (Context con in allContexts)
            if (con.Active && con.Name.Equals(conName))
                return true;
        return false;
    }

    public string[] GetActiveContexts() {
        List<string> activeContexts = new List<string>();
        foreach (Context con in allContexts)
            if (con.Active)
                activeContexts.Add(con.Name);
        return activeContexts.ToArray();
    }

    public string DiscussPerceive(string[] info, bool seen = true) {
        Association subj = null;
        VerbAssoc vb = null;
        Association obj = null;

        Dictionary<string[], EventInfo> eventsList;
        eventsList = (seen) ? seenEvents : unseenEvents;

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

        if (subj == null || vb == null || (info.Length == 3 && obj == null))
            return "";

        totalInterest += eventsList[info].Interest;

        List<string> feelContexts = new List<string>(); //Initialize and declare list of contexts
        foreach (string s in eventsList[info].Contexts) //For each context 
            feelContexts.Add(s); //Add it to feelContexts
        return Feel(subj, obj, vb, totalInterest, new Interaction(eventsList[info].Polarity, eventsList[info].Strength), 1, feelContexts, seen); //Feel the event!
    }

    public void Perceive(string[] info, Interaction interaction, string[] contextNames, bool seen = true) {
        div = 1;
        Dictionary<string[], EventInfo> eventsList;
        eventsList = (seen) ? seenEvents : unseenEvents;

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

        if (subj == null || vb == null || (info.Length == 3 && obj == null))
            return;

        string infoSentence = string.Join(" ", info); //convert info so it can be compared to info
        infoSentence = infoSentence.ToLower(); //convert info so it can be compared to info


        //GET TOTALINTEREST AND DIV
        bool alreadySeen = false; //Whether this event has already been seen
        foreach (string[] sentenceList in eventsList.Keys) { //Iterate through each sentence
            string sentence = string.Join(" ", sentenceList); //Convert sentence so it can be compared to info
            sentence = sentence.ToLower(); //Convert sentence so it can be compared to info
            if (sentence.Equals(infoSentence)) { //If sentence equals info
                div = 0.25f; //Experience this event at a quarter capacity
                eventsList[sentenceList].Apply(interestDelt: vb.Interest); //Increase the int of the already witnessed event by the int level of the verb
                totalInterest += eventsList[sentenceList].Interest; //Increase the current interest by accumulative interest of the event
                alreadySeen = true; //This event has already been witnessed
                break; //Stop iterating
            }
        }
        if (!alreadySeen) //If this event has not already been witnessed
            totalInterest += subj.Interest + vb.Interest + ((obj != null) ? obj.Interest : 0); //Add the interest levels of subj, vb, and obj to totalInt

        div = (totalInterest > interestThreshold) ? 1 : 0.25f; //If the totalInterest surpasses the threshold,

        totalInterest *= div; //Divide totalInterest by the appropriate amount

        //GET INTERACTION CALIBRATED
        if (interaction.Strength == 0) //If strength is null, i.e. the object supplied no reaction to the verb
            interaction.Apply(polarityDelt: vb.Polarity, strengthDelt: vb.Interest); //Apply the polarity and interest of the verb
        else { interaction.Set(strengthSet: ((interaction.Strength + totalInterest) / 2)); } //Increase the strength by some portion of the interest
        interaction.Set(polaritySet: (interaction.Polarity * div), strengthSet: (interaction.Strength * div)); //Divide the pol and str appropriately
        interaction.CalibrateStrength(); //Set strength aright

        if (!alreadySeen)
            eventsList.Add(info, new EventInfo(totalInterest, interaction.Polarity, interaction.Strength, contextNames)); //Add this event to seen events

        //FEEL EVENT
        if (seen && totalInterest > interestThreshold) { //If seen
            List<string> feelContexts = new List<string>(); //Initialize and declare list of contexts
            foreach (string s in contextNames) //For each context 
                feelContexts.Add(s); //Add it to feelContexts
            Feel(subj, obj, vb, totalInterest, interaction, div, feelContexts); //Feel the event!
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
        Dictionary<MoodAssoc, float> feels = new Dictionary<MoodAssoc, float>() { }; //Apply this to the perceiver's moods
        List<Association> branch = new List<Association>() { };

        subj.GetMood(feels, branch, interest, div, 0);

        vb.GetMood(feels, branch, interest, div, 0);
        subj.AddAssociation(vb, 1, vb.Interest * interest);

        if (obj != null) {
            obj.GetMood(feels, branch, interaction.Strength * Mathf.Sign(interaction.Polarity), div, 0);
            subj.AddAssociation(obj, interaction.Polarity*interest, interaction.Strength*interest);
        }

        //APPLY MOODS HERE
        if (!opine) {
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

        foreach (Context con in activeContexts) {
            UnapplyContext(con);
        }

        return returnSentence;
    }

    public void Feel(Association _concept) { }

}