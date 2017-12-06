using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPicker {

    protected Body genitor;
    public List<Listener> Listeners;

    public QuestPicker(Body _genitor) {
        genitor = _genitor;
        Listeners = new List<Listener>() { };
    }

    public virtual Quest CheckListener(Listener listener) {
        return null;
    }

    public virtual Quest GetQuest() {
        Quest topQuest = new StayQuest(genitor, 10, 0);
        List<Quest> QuestList = new List<Quest>() {
            new StayQuest(genitor),
            new MoveToQuest(genitor),
            new TalkToQuest(genitor, 7),
            new GiftQuest(genitor),
            new PerformFavorQuest(genitor, genitor.GetPersonality().FindPerson(true)),
            new AnnounceQuest(genitor, 5)
        }; //ALL 'NORMAL' QUESTS SHOULD BE IN THIS LIST
        QuestList.AddRange(GetQuestContext());
        QuestList.AddRange(GetQuestSpecial());
        QuestList.AddRange(GetQuestMoods());
        bool foundTop = false;
        foreach (Quest q in QuestList)
            if ((q.Priority + Random.Range(-2, 4)) > (topQuest.Priority + 3)) {
                topQuest = q;
                foundTop = true;
            }
        if (foundTop) {
            return topQuest;
        }
        else return Statics.RandomElement(QuestList);

    }

    public virtual List<Quest> GetQuestContext() {
        List<Quest> potentialQuests = new List<Quest>() { };
        return potentialQuests;
    }

    public virtual List<Quest> GetQuestSpecial() {
        List<Quest> potentialQuests = new List<Quest>() { };

        if (genitor.Weapon.GetType() == typeof(Fists)) {
            Item newWeapon = genitor.GetPersonality().FindItem(genitor.GetPersonality().GetAssociation("weaponry"));
            potentialQuests.Add(new EquipQuest(genitor, newWeapon, 5));
        }

        if (genitor.harmQuant > 0.5f) {
            Item newConsumable = genitor.GetPersonality().FindItem(genitor.GetPersonality().GetAssociation("consumable"));
            potentialQuests.Add(new ConsumeQuest(genitor, newConsumable, 5));
            potentialQuests.Add(new GetHealedQuest(genitor));
        }

        return potentialQuests;
    }

    public virtual List<Quest> GetQuestMoods() {
        List<Quest> potentialQuests = new List<Quest>() { };
        Mood topMood = genitor.GetPersonality().moodHandler.GetDominantMood();
        if (topMood.Polarity > 0) {
            potentialQuests.Add(new GiftQuest(genitor));
        }
        else {
            GameObject lowPerson = genitor.GetPersonality().FindPerson(false);
            if (lowPerson != null) {
                Item newItem = Statics.RandomElement(lowPerson.GetComponent<Body>().Inventory);
                if (newItem != null) {
                    potentialQuests.Add(new PickUpQuest(genitor, newItem, newItem.holder.gameObject, newItem.holder, 3));
                }
            }
            potentialQuests.Add(new StartFightQuest(genitor, genitor.GetPersonality().FindPerson(false)));
        }

        string moodstr = topMood.GetId();
        Association a = genitor.GetPersonality().GetAssociation(moodstr);
        if (a == null) {
            GameObject newPal = genitor.GetPersonality().FindPerson(a);
            potentialQuests.Add(new TalkToQuest(genitor, genitor.GetPersonality().FindPerson(newPal), genitor.Id + " feels " + topMood + " with you."));
        }

        return potentialQuests;
    }
}


//SOLDIER
public class Soldier_QuestPicker : QuestPicker { //needs listener for "x", "tells joke"
    public Soldier_QuestPicker(Body _genitor) : base(_genitor) {
        Listeners.Add(new Listener(new string[] { "x", "tells joke" }, genitor.GetPersonality(), false));
    }

    public override List<Quest> GetQuestContext() {
        if (genitor.GetPersonality().HasContext("lethal"))
            Debug.Log("TODO MAKE FIGHT QUEST");
        else if (genitor.GetPersonality().HasContext("non-lethal") && genitor.harmQuant > 0.75f)
            Debug.Log("TODO MAKE FIGHT QUEST");
        else if (genitor.GetPersonality().HasContext("sparring"))
            Debug.Log("TODO MAKE FIGHT QUEST");
        return base.GetQuestContext();
    }

    public override List<Quest> GetQuestSpecial() {
        //If the door watchzone is being collided with
        //Walk there

        //If no weapon
        //Find item associated with weaponry / weapon

        //If low health
        //Find item associated with healing / healing item
        //Or find person associated with healing and request healing

        return base.GetQuestSpecial();
    }

    public override List<Quest> GetQuestMoods() {
        //If angry
        //Patrol, guard door, scold someone

        //If frightened
        //patrol, guard door

        //If nothing
        //Do something normal

        return base.GetQuestMoods();
    }
}