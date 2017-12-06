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
    public List<Quest> SubQuests;
    protected bool done = false;

    public Quest(Body _genitor, int _priority = 1) {
        name = "quest";
        genitor = _genitor;
        priority = _priority;
        SubQuests = new List<Quest>() { this };

    }

    public virtual Action GetAction() {
        return new Action("Open", 0, genitor);
    }

    public virtual void End() { }
}

public class StayQuest : Quest {

    int duration;

    public StayQuest(Body _genitor, int _duration = 5, int _priority = 5) : base(_genitor, _priority) {
        name = "stay";
        duration = _duration;
        SubQuests = new List<Quest>() { this };
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

    public MoveToQuest(Body _genitor, float _target = float.MinValue, GameObject _targetObj = null, int _priority = 1) : base(_genitor, _priority) {
        name = "move to";
        target = (target < -9000) ? _target : Statics.RandomLocIn(_genitor.Mind.Place);
        targetObj = _targetObj;
        SubQuests = new List<Quest> { this };
    }

    public override Action GetAction() {
        if (targetObj != null)
            target = targetObj.transform.position.x;
        int direction = -(int)(Mathf.Sign(genitor.transform.position.x - target));
        if (direction == 0 || (Mathf.Abs(genitor.gameObject.transform.position.x - target) < 3))
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
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        if (Mathf.Abs(genitor.gameObject.transform.position.x - targetPerson.gameObject.transform.position.x) < 3) {
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

        if (targetPerson != null)
            SubQuests = new List<Quest> { new MoveToQuest(genitor, _targetObj: targetPerson), this };
    }

    public TalkToQuest(Body _genitor, GameObject _targetPerson, string line = null, int _priority = 2) : base(_genitor, _priority) {
        name = "talk to";
        genitor.GetPersonality().OpeningText = line ?? genitor.GetPersonality().OpeningText;
        targetPerson = _targetPerson;
        if (targetPerson != null)
            SubQuests = new List<Quest> { new MoveToQuest(genitor, _targetObj: targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (targetPerson != null)
                return new TalkAction("Talk", 0, genitor, targetPerson.GetComponent<Body>().GetPersonality());
            return new Action("Open", 0, genitor);
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
        if (targetPerson != null)
            SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj:targetPerson), new TalkToQuest(genitor, targetPerson), this };
        else SubQuests = new List<Quest>() { new Quest(genitor)};
    }

    public override Action GetAction() {
        //TODO GIVE GIFT TO PERSON
        if (!done) {
            done = true;
            return new GiftAction("Gift", 0, genitor, targetPerson.GetComponent<Body>(), gift);
        }
        return new Action("Open", 0, genitor );
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
        newListener1 = new Listener(new string[] { "x", "gifts", genitor.Id, "." + targetItem.Function }, genitor.GetPersonality());
        genitor.Mind.QuestMind.Listeners.Add(newListener1);
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetObj), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (targetHolder.GetType() == typeof(ItemPackage)) {
                return new PickupAction("Pick Up", 0, genitor, (ItemPackage)targetHolder, targetItem);
            }        }
        //if (targetHolder.GetType() == typeof(Body))
        //    return new PickupAction("Pick Up", 0, genitor, targetHolder, targetItem);
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
        if (equip != null && equip.holder != null && equip.holder.gameObject != null) {
            SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj:equip.holder.gameObject),
                new PickUpQuest(genitor, equip, equip.holder.gameObject, equip.holder), this };
        }
        else SubQuests = new List<Quest>() { new Quest(genitor) };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (equip != null)
                return new EquipAction("Equip", 0, genitor, equip);
        }
        return new Action("Open", 0, genitor); //DELET THIS
    }
}

public class ConsumeQuest : Quest {
    Item consumable = null;

    public ConsumeQuest(Body _genitor, Item _consumable, int _priority = 2) : base(_genitor, _priority) {
        name = "consume";
        consumable = _consumable;
        if (consumable != null)
            SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: consumable.holder.gameObject),
                new PickUpQuest(genitor, consumable, consumable.holder.gameObject, consumable.holder), this };
        else SubQuests = new List<Quest>() { new Quest(genitor) };
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
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (drop != null)
                return new DropAction("Drop", 0, genitor, drop);
        }
        return new Action("Open", 0, genitor); //DELET THIS
    }
}

public class PerformFavorQuest : Quest {

    GameObject targetPerson = null;

    public PerformFavorQuest(Body _genitor, GameObject _targetPerson, int _priority = 2) : base(_genitor, _priority) {
        name = "perform favor for";
        targetPerson = _targetPerson;
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (targetPerson != null)
                return new PerformFavorAction("Perform Favor", 0, genitor, targetPerson.GetComponent<Body>());
        }
        return new Action("Open", 0, genitor);
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
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new GetHealedAction("Get Healed", 0, genitor, targetPerson.GetComponent<Body>());
        }
        return new Action("Open", 0 , genitor);
    }
}

public class StartFightQuest : Quest {
    GameObject targetPerson = null;

    public StartFightQuest(Body _genitor, GameObject _targetPerson = null, int _priority = 5) : base(_genitor, _priority) {
        name = "start fight";
        targetPerson = _targetPerson ?? genitor.GetPersonality().FindPerson(false);
        if (targetPerson != null) {
            SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), this, new FightQuest(genitor, targetPerson) };
        }
        else SubQuests = new List<Quest>() { new Quest(genitor) };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            if (targetPerson == null)
                return new Action("Open", 0, genitor);
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
        }
        return new Action("Open", 0, genitor);
    }
}

public class FightQuest : Quest {
    GameObject targetPerson = null;

    public FightQuest(Body _genitor, GameObject _targetPerson, int _priority = 10) : base(_genitor, _priority) {
        name = "fight";
        targetPerson = _targetPerson;
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        Vector3 targetPos = targetPerson.transform.position;
        Vector3 genPos = genitor.gameObject.transform.position;

        float targetDist = Mathf.Abs(targetPos.x - genPos.x);
        Body targetBod = targetPerson.GetComponent<Body>();

        float targetHarm = targetBod.HarmQuant;
        float targetHinder = targetBod.HinderQuant;

        float genHarm = genitor.HarmQuant;
        float genHinder = genitor.HinderQuant;

        if (genHarm > 0.85) {
            Quest nextQuest = new FleeQuest(genitor, targetPerson);
            genitor.SetNextQuest(nextQuest);
            return new Action("Open", 0, genitor);
        }
        else if (targetDist > 5 && targetDist < 10 && !targetBod.Dashing) {
            return new MoveAction("Dash", genitor.GetDashSpeed(), genitor, Direction.Left, 5);
        }
        else if (targetDist < 3 && targetBod.CurrAct.GetType() != typeof(Block)) {
            ((ICloseMelee)targetBod.Weapon).CloseMeleeLightAttack();
            return new Action("Hold", 0, genitor);
        }

        Action targetAction = targetBod.CurrAct;
        Direction targetFace = targetBod.face;
        bool targetDash = targetBod.Dashing;
        return new Action("Open", 0, genitor);
    }
}


public class FleeQuest : Quest {
    GameObject targetObj = null;
    float target;

    public FleeQuest(Body _genitor, GameObject _taretObj, int _priority=10) : base(_genitor, _priority) {
        name = "flee";
        targetObj = _taretObj;
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        if (targetObj != null)
            target = targetObj.transform.position.x;
        float targetDist = Mathf.Abs(target - genitor.transform.position.x);
        int direction = (int)(Mathf.Sign(target - genitor.transform.position.x));
        if (direction == 0 || (targetObj != null && ((genitor.gameObject.transform.position.x - targetObj.transform.position.x) > 50)))
            return new Action("Open", 0, genitor);
        if (targetDist < 15)
            return new MoveAction("Dash", genitor.GetDashSpeed(), genitor, (Direction)direction, 5);
        else
            return new MoveAction("Move", genitor.GetMoveSpeed(), genitor, (Direction)direction, 0);
    }
}


public class AdvanceQuest : Quest {
    GameObject targetPerson = null;

    public AdvanceQuest(Body _genitor, GameObject _targetPerson, int _priority=8) : base(_genitor, _priority) {
        name = "advance";
        targetPerson = _targetPerson;
        SubQuests = new List<Quest>() { this };
    }

    public override Action GetAction() {
        return base.GetAction();
    }
}


//--------USED BY SOLDIER AND GUARDS--------

public class PatrolQuest : Quest {

    public PatrolQuest(Body _genitor, int _priority = 2) : base(_genitor, _priority) {
        name = "patrol";
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, genitor.Mind.Place.Coordinate-genitor.Mind.Place.Size/4),
                                        new StayQuest(genitor, 20),
                                        new MoveToQuest(genitor, genitor.Mind.Place.Coordinate+genitor.Mind.Place.Size/4),
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
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new ScoldAction("Scold", 0, genitor, targetPerson.GetComponent<Body>());
        }
        return new Action("Open", 0, genitor);
    }
}

public class OpenDoorQuest : Quest {

    GameObject targetDoor;

    public OpenDoorQuest(Body _genitor, GameObject _targetDoor, int _priority = 2) : base(_genitor, _priority) {
        name = "scold";
        targetDoor = _targetDoor;
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetDoor), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new OpenDoorAction("Open Door", 0, genitor, targetDoor.GetComponent<Impassable>());
        }
        return new Action("Open", 0, genitor);
    }
}

public class GuardDoorQuest : Quest {

    GameObject targetDoor;

    public GuardDoorQuest(Body _genitor, GameObject _targetDoor, int _priority = 2) : base(_genitor, _priority) {
        name = "guard door";
        targetDoor = _targetDoor;
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
        SubQuests = new List<Quest>() { new MoveToQuest(genitor, _targetObj: targetPerson), new TalkToQuest(genitor, targetPerson), this };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new TellJokeAction("Tell Joke", 0, genitor, targetPerson.GetComponent<Body>());
        }
        return new Action("Open", 0, genitor);
    }
}

//--------USED BY TOWN CRIER--------

public class AnnounceQuest : Quest {

    public AnnounceQuest(Body _genitor, int _priority = 2) : base(_genitor, _priority) {
        name = "tell joke";
        SubQuests = new List<Quest>() { this, new StayQuest(genitor), this, new StayQuest(genitor), this, new StayQuest(genitor) };
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new AnnounceAction("Announce", 0, genitor);
        }
        return new Action("Open", 0, genitor);
    }
}
