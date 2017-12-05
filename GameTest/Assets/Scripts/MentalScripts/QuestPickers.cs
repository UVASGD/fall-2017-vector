using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Contexts - lastly do this
 * Special Circumstances - look at this second
 * Moods - look at this first
 */

public class QuestPicker {
    protected Body genitor;
    public QuestPicker(Body _genitor) {
        genitor = _genitor;
    }

    public virtual Quest GetQuest() {
        Quest topQuest = new StayQuest(genitor, 10, 0);
        List<Quest> QuestList = new List<Quest>() { };
        QuestList.AddRange(GetQuestContext());
        QuestList.AddRange(GetQuestSpecial());
        QuestList.AddRange(GetQuestMoods());
        foreach (Quest q in QuestList)
            if (q.Priority > topQuest.Priority)
                topQuest = q;
        return topQuest;
    }

    public virtual List<Quest> GetQuestContext() {
        return new List<Quest>() { new StayQuest(genitor) };
    }

    public virtual List<Quest> GetQuestSpecial() {
        return new List<Quest>() { new StayQuest(genitor) };
    }

    public virtual List<Quest> GetQuestMoods() {
        return new List<Quest>() { new StayQuest(genitor) };
    }
}


//SOLDIER
public class Soldier_QuestPicker : QuestPicker { //needs listener for "x", "tells joke"
    public Soldier_QuestPicker(Body _genitor) : base(_genitor) { }

    public override List<Quest> GetQuestContext() {
        if (genitor.GetPersonality().HasContext("Lethal"))
            Debug.Log("TODO MAKE FIGHT QUEST");
        else if (genitor.GetPersonality().HasContext("Non-Lethal") && genitor.harmQuant > 0.75f)
            Debug.Log("TODO MAKE FIGHT QUEST");
        else if (genitor.GetPersonality().HasContext("Sparring"))
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

