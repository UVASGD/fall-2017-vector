using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//What are the relation strings gonna be? -Simon Whittle, 2012
/*
STRING  ::  OPERATION
"Equals"  ::  .equals()
"In"  ::  in list
GT
LT
GE
LE
*/

public class Conditional : IConditional {

    object target;
    string relation;
    object anchor;

    public Conditional(object _target, string _relation, object _anchor) {
        target = _target;
        relation = _relation;
        anchor = _anchor;
    }

    public bool Check() {
        bool result = false;
        switch (relation) {
            case "In":
                foreach (object o in (List<object>)anchor)
                    result = result || o.Equals(target);
                break;
            case "Equals":
                result = target.Equals(anchor); break;

            case "GTI":
                result = (int)target > (int)anchor; break;
            case "GEI":
                result = (int)target >= (int)anchor; break;
            case "LTI":
                result = (int)target < (int)anchor; break;
            case "LEI":
                result = (int)target <= (int)anchor; break;

            case "GTF":
                result = (float)target > (float)anchor; break;
            case "LTF":
                result = (float)target < (float)anchor; break;
        }
        return result;
    }
}

public class GroupConditional : IConditional {

    protected List<IConditional> conds;
    bool isAndElseOr;

    public GroupConditional(List<IConditional> _conds, bool _isAndElseOr) {
        conds = _conds;
        isAndElseOr = _isAndElseOr;
    }

    public virtual bool Check() {
        bool result = isAndElseOr;
        foreach (IConditional cond in conds) {
            if (isAndElseOr)
                result = result && cond.Check();
            else
                result = result || cond.Check();
        }
        return result;
    }
}

public interface IConditional {
    bool Check();
}

/*
Ends Self
Modulates Own Variable
Spawns Other Effect
Modulates Other Effect
*/

public class Result {

}


