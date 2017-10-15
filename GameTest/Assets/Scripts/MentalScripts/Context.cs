using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context {

    string name;
    public string Name { get { return name; } }

    AssocAffecter[] assocAffecters;
    ActiveAssocAffecter[] activeAssocAffecters;
}

public class ActiveAssocAffecter {
    Association targetAssoc;
    float interestDelta;
    Association[] markAddList;
    Association[] markDelList;

    public ActiveAssocAffecter(Association _targetAssoc, float _interestDelta = 0, Association[] _markAddList = null, Association[] _markDelList = null) {
        targetAssoc = _targetAssoc;
        interestDelta = _interestDelta;
        markAddList = _markAddList ?? new Association[] { };
        markDelList = _markDelList ?? new Association[] { };
    }
}

public class AssocAffecter {
    string targetAssoc;
    float interestDelta;
    string[] markAddList;
    string[] markDelList;

    public AssocAffecter(string _targetAssoc, float _interestDelta = 0, string[] _markAddList = null, string[] _markDelList = null) {
        targetAssoc = _targetAssoc;
        interestDelta = _interestDelta;
        markAddList = _markAddList ?? new string[] { };
        markDelList = _markDelList ?? new string[] { };
    }
}
