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
}

public class StayQuest : Quest {

    int duration;
    public StayQuest(string _name, Body _genitor, int _duration = 5, int _priority = 2) : base(_name, _genitor, _priority) {
        duration = _duration;        
    }
}

public class MoveToQuest : Quest {

    float target; 
    public MoveToQuest(string _name, Body _genitor, int _priority = 2) : base(_name, _genitor, _priority) {
        target = Statics.RandomLocIn(_genitor.CurrPlace);
    }
    public MoveToQuest(string _name, Body _genitor, float _target, int _priority = 2) : base(_name, _genitor, _priority) {
        target = _target;
    }
}

public class TalkToQuest : Quest {
    GameObject person;
    public TalkToQuest(string _name, Body _genitor, Association trait, bool high, float distance, int _priority = 2) : base(_name, _genitor, _priority) {
        //Find person with that trait within a certain distance
    }

    public TalkToQuest(string _name, Body _genitor, GameObject _person, int _priority = 2) : base(_name, _genitor, _priority) {
        person = _person;
    }
}
