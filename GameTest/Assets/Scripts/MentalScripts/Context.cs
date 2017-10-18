using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context {

    string name;
    public string Name { get { return name; } }

    bool active;
    public bool Active { get { return active; } set { active = value; } }

    public AssocAffecter[] assocAffecters;

    public Context(string _name, AssocAffecter[] _assocAffecters) {
        name = _name;
        assocAffecters = _assocAffecters;    }
}

public class AssocAffecter {
    public Association targetAssoc;
    float interestDelt;
    public float InterestDelt { get { return interestDelt; } }
    public Dictionary<Association, Interaction> markAddList;
    public Dictionary<Association, Interaction> markDelList;

    public AssocAffecter(Association _targetAssoc, float _interestDelt = 0, 
        Dictionary<Association, Interaction> _markAddList = null, Dictionary<Association, Interaction> _markDelList = null) {
        targetAssoc = _targetAssoc;
        interestDelt = _interestDelt;
        markAddList = _markAddList ?? new Dictionary<Association, Interaction> { };
        markDelList = _markDelList ?? new Dictionary<Association, Interaction> { };
    }
}