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

    public Quest(string _name, Body _genitor, int _priority = 1) {
        name = _name;
        genitor = _genitor;
    }

    protected virtual void Setup() {
        SubQuests = new List<Quest>() { new Quest("Open", genitor) };
    }

    public virtual Action GetAction() {
        return new Action("Open", 0, genitor); }
}

public class StayQuest : Quest {

    int duration;
    public StayQuest(string _name, Body _genitor, int _duration = 10, int _priority = 2) : base(_name, _genitor, _priority) {
        duration = _duration;
    }

    protected override void Setup() {
        SubQuests = new List<Quest>() { new StayQuest(name, genitor, duration) };
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
    public MoveToQuest(string _name, Body _genitor, int _priority = 2) : base(_name, _genitor, _priority) {
        target = Statics.RandomLocIn(_genitor.CurrPlace);
        targetObj = null;
    }
    public MoveToQuest(string _name, Body _genitor, float _target, GameObject _targetObj = null, int _priority = 2) : base(_name, _genitor, _priority) {
        target = _target;
        targetObj = _targetObj;
    }

    protected override void Setup() {
        SubQuests = new List<Quest> { new MoveToQuest(name, genitor, target, targetObj)};
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

public class TalkToQuest : Quest {
    GameObject person;
    public TalkToQuest(string _name, Body _genitor, float distance, bool high = true, Association trait = null, float inter = 0, int _priority = 2) 
        : base(_name, _genitor, _priority) {
        if (trait != null)
            person = genitor.GetPersonality().FindPerson(trait);
        else
            person = genitor.GetPersonality().FindPerson(high);
    }   
    public TalkToQuest(string _name, Body _genitor, GameObject _person, int _priority = 2) : base(_name, _genitor, _priority) {
        person = _person;
    }

    protected override void Setup() {
        if (person != null)
            SubQuests = new List<Quest> { new MoveToQuest(name, genitor, 0, person), this};
    }

    public override Action GetAction() {
        if (!done) {
            done = true;
            return new TalkAction("Talk", 0, genitor, person.GetComponent<Body>().GetPersonality());
        }
        else
            return new Action("Open", 0, genitor);
    }
}
