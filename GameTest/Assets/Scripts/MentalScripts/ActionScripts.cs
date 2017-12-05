using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

    protected string name;
    public string Name { get { return name; } }
    protected Action nextAction;
    protected int timeLeft;
    protected Body genitor;
    protected float speedMod = 1f;
    protected bool overridable = true;
    public bool Overridable { get { return overridable; } }

    public Action(string _name, int _speedFactor, Body _genitor) {
        name = _name;
        timeLeft = (int) (_speedFactor * speedMod);
        genitor = _genitor;
    }

    protected virtual void Dewit() {
        nextAction = nextAction ?? new Action("Open", 0, genitor);
        genitor.SetCurrAct(nextAction);
    }

    public virtual void Tick() {
        timeLeft--;

        if (timeLeft <= 0)
            Dewit();
    }
}

public class MoveAction : Action {

    int iterLeft;
    Direction dir;

    public MoveAction(string _name, int _speedFactor, Body _genitor, Direction _dir, int _iterLeft) : base(_name, _speedFactor, _genitor) {
        iterLeft = _iterLeft;
        dir = _dir;

        if (iterLeft > 0)
            nextAction = new MoveAction(_name, _speedFactor, _genitor, _dir, _iterLeft - 1);
        else {
            nextAction = new Action("Open", 0, _genitor);
        }
    }

    protected override void Dewit() {
        genitor.Move(dir);
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class HaltAction : Action {
    public HaltAction(string _name, int _speedFactor, Body _genitor) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = _speedFactor;
        nextAction = new Action("Open", 0, genitor);
    }

    protected override void Dewit() {
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class TalkAction : Action {
    public TalkAction(string _name, int _speedFactor, Body _genitor, Personality _otherPerson) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 100;
        nextAction = this;
        if (genitor.Mind.GetType() == typeof(PlayerAI))
            genitor.BeginTalk(_otherPerson);
        else if (_otherPerson.GetBody.Mind.GetType() == typeof(PlayerAI))
            _otherPerson.GetBody.BeginTalk(genitor.GetPersonality());
        else genitor.NPCTalk(_otherPerson);
    }

    protected override void Dewit() {
        timeLeft = 100;
    }
}

public class EndTalkAction : Action {
    public EndTalkAction(string _name, int _speedFactor, Body _genitor) :
        base(_name, _speedFactor, _genitor) {
        genitor.EndTalk();
        nextAction = new Action("Open", 0, genitor);
    }
}

public class AttackAction : Action {

    GameObject attack;

    public AttackAction(string _name, int _speedFactor, Body _genitor, GameObject _attack) : base(_name, _speedFactor, _genitor) {
        attack = _attack;
    }

    protected override void Dewit() {
        /*Vector3 newPos = new Vector3();
        newPos = genitor.transform.position;

        if (genitor.GetFace() == Direction.Left) {
            newPos += new Vector3(-2, 0);
        }
        else {
            newPos += new Vector3(2, 0);
        }

        GameObject newAttackObject = Object.Instantiate(attack, newPos, Quaternion.identity);

        SpriteRenderer attackRender = newAttackObject.GetComponent<SpriteRenderer>();
        attackRender.sortingLayerName = "0";

        Attack newAttackScript = newAttackObject.GetComponent<Attack>();
        newAttackScript.AttackConstructor(genitor);

        nextAction = new Action("Open", 0, genitor);*/
        base.Dewit();
    }
}

public enum ImpedimentLevel { unimpeded, noAttack, noAct, noMove };

public class Recovery : Action {

    ImpedimentLevel curImpLevel = ImpedimentLevel.unimpeded;

    //Affecter curImpediment;

    // TODO: make these Affecters
    //Affecter noMove;  // = new MoveDenier ??
    //Affecter noAct;  //  = new ActionDenier  ??
    //Affecter noAttack;  //  = new AttackDenier  ??

    public Recovery(string _name, int _timeLeft, Body _genitor) : base(_name, _timeLeft, _genitor) {
        overridable = false;
    }

    public override void Tick() {
        if (timeLeft > 20 && curImpLevel == ImpedimentLevel.unimpeded)
            genitor.Impediment = ImpedimentLevel.noMove;
        else if (timeLeft > 10)
            genitor.Impediment = ImpedimentLevel.noAct;
        else if (timeLeft > 5)
            genitor.Impediment = ImpedimentLevel.noAttack;
        else if (timeLeft <= 0)
            genitor.Impediment = ImpedimentLevel.unimpeded;

        base.Tick();
    }
}

public class PerformFavorAction : Action {
    public PerformFavorAction(string _name, int _speedFactor, Body _genitor, Body targetPerson) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        new EventSpawn(genitor.transform.position, targetPerson.GetPersonality().GetInteraction("performs favor for"), null, 
            genitor.Id, "performs favor for", targetPerson.Id, subject: genitor);
        nextAction = new Action("Open", 0, genitor);
    }
}

public class GetHealedAction : Action {
    public GetHealedAction(string _name, int _speedFactor, Body _genitor, Body targetPerson) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        new EventSpawn(genitor.transform.position, targetPerson.GetPersonality().GetInteraction("heals"), null,
            targetPerson.Id, "heals", genitor.Id, subject: targetPerson);
        genitor.harmQuant = 0;
        nextAction = new Action("Open", 0, genitor);
    }
}

public class GiftAction : Action
{
    public GiftAction(string _name, int _speedFactor, Body _genitor, Body targetPerson,Item item) :
        base(_name, _speedFactor, _genitor)
    {
        timeLeft = 0;
        genitor.Inventory.Remove(item);
        targetPerson.Inventory.Add(item);
        item.SwitchHolder(targetPerson);
        nextAction = new Action("Open", 0, genitor);
    }
}

public class PickupAction : Action
{
    public PickupAction(string _name, int _speedFactor, Body _genitor, ItemPackage targetPerson, Item item) :
        base(_name, _speedFactor, _genitor)
    {
        timeLeft = 0;
        targetPerson.Inventory.Remove(item);
        genitor.Inventory.Add(item);
        item.SwitchHolder(genitor);
        nextAction = new Action("Open", 0, genitor);
    }
}

//SPECIAL ACTIONS

public class ScoldAction : Action {
    public ScoldAction(string _name, int _speedFactor, Body _genitor, Body targetPerson) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        new EventSpawn(genitor.transform.position, targetPerson.GetPersonality().GetInteraction("scolds"), null,
            genitor.Id, "scolds", targetPerson.Id, subject: genitor);
        new EventSpawn(targetPerson.transform.position, targetPerson.GetPersonality().GetInteraction("is disgraced"), null,
            targetPerson.Id, "is disgraced", subject: targetPerson);
        nextAction = new Action("Open", 0, genitor);
    }
}

/*public class OpenDoorAction : Action {
    public OpenDoorAction(string _name, int _speedFactor, Body _genitor, Door targetDoor) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        targetDoor.Open();
        new EventSpawn(genitor.transform.position, new Interaction(), null,
            genitor.Id, "opens door", subject: genitor);
        nextAction = new Action("Open", 0, genitor);
    }
}*/

public class TellJokeAction : Action {
    public TellJokeAction(string _name, int _speedFactor, Body _genitor, Body targetPerson) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        new EventSpawn(genitor.transform.position, new Interaction(), null,
            genitor.Id, "jokes", subject: genitor, _scope:3);
        nextAction = new Action("Open", 0, genitor);
    }
}

public class AnnounceAction : Action {
    public AnnounceAction(string _name, int _speedFactor, Body _genitor, Body targetPerson = null) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 0;
        new EventSpawn(genitor.transform.position, new Interaction(), null,
        genitor.Id, "announces", subject: genitor);
        Association subj = Statics.RandomElement(genitor.GetPersonality().activeAssocs);
        Association primeAssoc = subj.GetPrimeMood();
        if (primeAssoc != null) 
            new EventSpawn(genitor.transform.position, new Interaction(100, 100), null,
                subj.Id, "incites", primeAssoc.Id, subject:genitor);
        nextAction = new Action("Open", 0, genitor);
    }
}