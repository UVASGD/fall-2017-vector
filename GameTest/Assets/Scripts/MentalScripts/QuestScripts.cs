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
    protected List<Quest> SubQuests = new List<Quest>();

    public Quest(string _name, Body _genitor, int _priority = 1) {
        name = _name;
        genitor = _genitor;
    }

    public virtual Action GetAction() {
        return new Action("Open", 0, genitor); }
}

public class StayQuest : Quest {

    int duration;
    public StayQuest(string _name, Body _genitor, int _duration = 10, int _priority = 2) : base(_name, _genitor, _priority) {
        duration = _duration;
        Setup();
    }

    void Setup() {
        SubQuests = new List<Quest>() { new StayQuest(name, genitor, duration) };
    }

    public override Action GetAction() {
        if ((duration--) > 0)
            return new HaltAction("Halt", 0, genitor);
        return new Action("Open", 0, genitor);
    }
}

public class MoveToQuest : Quest {

    float target;
    GameObject targetObj;
    public MoveToQuest(string _name, Body _genitor, int _priority = 2) : base(_name, _genitor, _priority) {
        target = Statics.RandomLocIn(_genitor.CurrPlace);
        Setup();
    }
    public MoveToQuest(string _name, Body _genitor, float _target, GameObject _targetObj = null, int _priority = 2) : base(_name, _genitor, _priority) {
        target = _target;
        targetObj = _targetObj;
        Setup();
    }

    void Setup() {
        SubQuests = new List<Quest> { new MoveToQuest(name, genitor, target, targetObj)};
    }
}

public class TalkToQuest : Quest {
    GameObject person;
    public TalkToQuest(string _name, Body _genitor, bool high, float distance, Association trait = null, float obl = 0, float inter = 0, int _priority = 2) 
        : base(_name, _genitor, _priority) {
        //Find person with that trait within a certain distance
        Setup();
    }   
    public TalkToQuest(string _name, Body _genitor, GameObject _person, int _priority = 2) : base(_name, _genitor, _priority) {
        person = _person;
        Setup();
    }

    void Setup() {
        SubQuests = new List<Quest> { new MoveToQuest(name, genitor, 0, person), new TalkToQuest(name, genitor, person)};
    }
}
