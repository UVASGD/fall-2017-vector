using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest {

    protected string name;
    public string Name { get { return name; } }
    protected Body genitor;
    public Body Genitor { get { return genitor; } }
    protected int priority;
    public int Priority { get { return priority; } }
    public List<Quest> SubQuests = new List<Quest>() { };
    protected bool done = false;

    public Quest(Body _genitor, int _priority = 1) {
        name = "quest";
        genitor = _genitor;
        priority = _priority;
        priority += Random.Range(-2, 4);
    }

    protected virtual void Setup() {
        SubQuests = new List<Quest>() { new Quest(genitor) };
    }

    public virtual Action GetAction() {
        return new Action("Open", 0, genitor);
    }

    public virtual void End() { }
}

public class StayQuest : Quest {

    int duration;

    public StayQuest(Body _genitor, int _duration = 10, int _priority = 2) : base(_genitor, _priority) {
        name = "stay";
        duration = _duration;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new StayQuest(genitor, duration) };
    }

    public override Action GetAction() {
        if ((duration--) > 0)
            return new HaltAction("Halt", 100, genitor);
        return new Action("Open", 0, genitor);
    }
}

public class MoveToQuest : Quest {

    float target;
    GameObject targetObj;

    public MoveToQuest(Body _genitor, float _target = float.MinValue, GameObject _targetObj = null, int _priority = 2) : base(_genitor, _priority) {
        name = "move to";
        target = (target == float.MinValue) ? _target : Statics.RandomLocIn(_genitor.CurrPlace);
        targetObj = _targetObj;
    }

    protected override void Setup() {
        SubQuests = new List<Quest> { new MoveToQuest(genitor, target, targetObj) };
    }

    public override Action GetAction() {
        if (targetObj != null)
            target = targetObj.transform.position.x;
        int direction = (int)(Mathf.Sign(genitor.transform.position.x - target));
        if (direction == 0 || (targetObj != null && ((genitor.gameObject.transform.position.x - targetObj.transform.position.x) < 3)))
            return new Action("Open", 0, genitor);
        return new MoveAction("Move", genitor.GetMoveSpeed(), genitor, (Direction)direction, 0);
    }
}

public class FollowQuest : Quest {

    GameObject targetPerson = null;
    int duration = 0;

    public FollowQuest(Body _genitor, GameObject _targetPerson, int _duration = 50, int _priority = 2) : base(_genitor, _priority) {
        name = "pick up";
        targetPerson = _targetPerson;
        duration = _duration;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        if ((genitor.gameObject.transform.position.x - targetPerson.gameObject.transform.position.x) < 3) {
            if ((duration--) > 0) {
                return new HaltAction("Halt", 100, genitor);
            }
            else {
                return new Action("Open", 0, genitor);
            }
        }
        int direction = (int)(Mathf.Sign(genitor.transform.position.x - targetPerson.transform.position.x));
        return new MoveAction("Move", genitor.GetMoveSpeed(), genitor, (Direction)direction, 0);
    }
}

public class TalkToQuest : Quest {

    GameObject targetPerson;

    public TalkToQuest(Body _genitor, float distance, bool high = true, Association trait = null, float inter = 0, string line = null, int _priority = 2)
        : base(_genitor, _priority) {
        name = "talk to";
        genitor.GetPersonality().OpeningText = line ?? genitor.GetPersonality().OpeningText;

        if (trait != null)
            targetPerson = genitor.GetPersonality().FindPerson(trait);
        else
            targetPerson = genitor.GetPersonality().FindPerson(high);
    }
    public TalkToQuest(Body _genitor, GameObject _targetPerson, string line = null, int _priority = 2) : base(_genitor, _priority) {
        name = "talk to";
        genitor.GetPersonality().OpeningText = line ?? genitor.GetPersonality().OpeningText;
        targetPerson = _targetPerson;
    }

    protected override void Setup() {
        if (targetPerson != null)
            SubQuests = new List<Quest> { new MoveToQuest(genitor, _targetObj: targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new TalkAction("Talk", 0, genitor, targetPerson.GetComponent<Body>().GetPersonality());
        }
        else
            return new Action("Open", 0, genitor);
    }
}

public class GiftQuest : Quest {

    Item gift = null;
    GameObject targetPerson = null;

    public GiftQuest(Body _genitor, GameObject _targetPerson = null, Item _gift = null, int _priority = 2) : base(_genitor, _priority) {
        name = "gift";
        gift = _gift ?? Statics.RandomElement(genitor.Inventory);
        targetPerson = _targetPerson ?? genitor.GetPersonality().FindPerson(true);
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        //TODO GIVE GIFT TO PERSON
        return new GiftAction("Gift", 0, genitor, targetPerson.GetComponent<Body>(), gift); //DELET THIS
    }
}

public class PickUpQuest : Quest {

    Item targetItem = null;
    GameObject targetObj = null;
    Body targetHolder = null;
    Listener newListener1;

    public PickUpQuest(Body _genitor, Item _targetItem, GameObject _targetObj, Body _targetHolder, int _priority = 2) : base(_genitor, _priority) {
        name = "pick up";
        targetItem = _targetItem;
        targetObj = _targetObj;
        targetHolder = _targetHolder;
        newListener1 = new Listener(new string[] { "x", "gives", genitor.Id, "." + targetItem.Function }, genitor.GetPersonality());
        genitor.Mind.QuestMind.Listeners.Add(newListener1);
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetObj), this };
    }

    public override Action GetAction() {
        //TODO IF TARGETHOLDER IS PACKAGE PICK UP TARGETITEM
        //TODO IF TARGETHOLDER IS PERSON GET ITEM BY REQUEST OR DEMAND
        return new Action("Open", 0, genitor); //DELET THIS
    }

    public override void End() {
        genitor.Mind.QuestMind.Listeners.Remove(newListener1);
    }
}

public class EquipQuest : Quest {
    Item equip = null;

    public EquipQuest(Body _genitor, Item _equip, int _priority = 2) : base(_genitor, _priority) {
        name = "equip";
        equip = _equip;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new PickUpQuest(genitor, equip, equip.holder.gameObject, equip.holder), this };
    }

    public override Action GetAction() {
        //EQUIP ITEM
        return new Action("Open", 0, genitor); //DELET THIS
    }
}

public class ConsumeQuest : Quest {
    Item consumable = null;

    public ConsumeQuest(Body _genitor, Item _consumable, int _priority = 2) : base(_genitor, _priority) {
        name = "consume";
        consumable = _consumable;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new PickUpQuest(genitor, consumable, consumable.holder.gameObject, consumable.holder), this };
    }

    public override Action GetAction() {
        //CONSUME ITEM
        return new Action("Open", 0, genitor); //DELET THIS
    }
}

public class DropQuest : Quest {
    Item drop = null;

    public DropQuest(Body _genitor, Item _drop, int _priority = 2) : base(_genitor, _priority) {
        name = "drop";
        drop = _drop;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        //TODO DROP ITEM
        return new Action("Open", 0, genitor); //DELET THIS
    }
}

public class PerformFavorQuest : Quest {

    GameObject targetPerson = null;

    public PerformFavorQuest(Body _genitor, GameObject _targetPerson, int _priority = 2) : base(_genitor, _priority) {
        name = "perform favor for";
        targetPerson = _targetPerson;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        return new PerformFavorAction("Perform Favor", 0, genitor, targetPerson.GetComponent<Body>());
    }
}

public class GetHealedQuest : Quest {

    GameObject targetPerson = null;
    Listener newListener1;
    Listener newListener2;

    public GetHealedQuest(Body _genitor, GameObject _targetPerson = null, int _priority = 2) : base(_genitor, _priority) {
        name = "get healed";
        targetPerson = _targetPerson ?? genitor.GetPersonality().FindPerson("healer");
        newListener1 = new Listener(new string[] { "x", "heals", genitor.Id }, genitor.GetPersonality());
        newListener2 = new Listener(new string[] { "x", "gives", genitor.Id, ".healing" }, genitor.GetPersonality());
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        return new GetHealedAction("Get Healed", 0, genitor, targetPerson.GetComponent<Body>());
    }
}

public class StartFightQuest : Quest {
    GameObject targetPerson = null;

    public StartFightQuest(Body _genitor, GameObject _targetPerson = null, int _priority = 2) : base(_genitor, _priority) {
        name = "start fight";
        targetPerson = _targetPerson ?? genitor.GetPersonality().FindPerson(false);
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), this };
    }

    public override Action GetAction() {
        float obl = genitor.GetPersonality().GetObligation(targetPerson.GetComponent<Body>().Id);
        if (obl < -0.70f) {
            return new StartFightAction("Start Fight", 0, genitor, "fights", "lethal");
        }
        else if (obl < -0.40f) {
            return new StartFightAction("Start Fight", 0, genitor, "brawls", "non-lethal");
        }
        else if (obl < 0) {
            return new StartFightAction("Start Fight", 0, genitor, "spars", "sparring");
        }
        else return new Action("Open", 0, genitor);
    }
}


//--------USED BY SOLDIER AND GUARDS--------

public class PatrolQuest : Quest {

    public PatrolQuest(Body _genitor, int _priority = 2) : base(_genitor, _priority) {
        name = "patrol";
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, genitor.CurrPlace.Coordinate-genitor.CurrPlace.Size/4),
                                        new StayQuest(genitor, 20),
                                        new MoveToQuest(genitor, genitor.CurrPlace.Coordinate+genitor.CurrPlace.Size/4),
                                        new StayQuest(genitor, 20)
        };
    }

    public override Action GetAction() {
        return new Action("Open", 0, genitor);
    }
}

public class ScoldQuest : Quest {

    GameObject targetPerson = null;

    public ScoldQuest(Body _genitor, GameObject _targetPerson = null, int _priority = 2) : base(_genitor, _priority) {
        name = "scold";
        targetPerson = _targetPerson ?? genitor.GetPersonality().FindPerson(false);
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        return new ScoldAction("Scold", 0, genitor, targetPerson.GetComponent<Body>());
    }
}

public class OpenDoorQuest : Quest {

    GameObject targetDoor;

    public OpenDoorQuest(Body _genitor, GameObject _targetDoor, int _priority = 2) : base(_genitor, _priority) {
        name = "scold";
        targetDoor = _targetDoor;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetDoor), this };
    }

    public override Action GetAction() {
        return new OpenDoorAction("Open Door", 0, genitor, targetDoor.GetComponent<Impassable>());
    }
}

public class GuardDoorQuest : Quest {

    GameObject targetDoor;

    public GuardDoorQuest(Body _genitor, GameObject _targetDoor, int _priority = 2) : base(_genitor, _priority) {
        name = "guard door";
        targetDoor = _targetDoor;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj:targetDoor),
                                        new StayQuest(genitor, 30),
        };
    }

    public override Action GetAction() {
        return new Action("Open", 0, genitor);
    }
}

//--------USED BY OLD MAN--------

public class TellJokeQuest : Quest {

    GameObject targetPerson;

    public TellJokeQuest(Body _genitor, int _priority = 2) : base(_genitor, _priority) {
        name = "tell joke";
        targetPerson = genitor.GetPersonality().FindPerson("Player");
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        return new TellJokeAction("Tell Joke", 0, genitor, targetPerson.GetComponent<Body>());
    }
}

//--------USED BY TOWN CRIER--------

public class AnnounceQuest : Quest {

    public AnnounceQuest(Body _genitor, int _priority = 2) : base(_genitor, _priority) {
        name = "tell joke";
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { this, new StayQuest(genitor), this, new StayQuest(genitor), this, new StayQuest(genitor) };
    }

    public override Action GetAction() {
        return new AnnounceAction("Announce", 0, genitor);
    }
}
