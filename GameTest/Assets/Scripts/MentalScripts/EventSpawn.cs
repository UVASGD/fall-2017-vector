using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawn {
    //string[] info, Interaction interaction, string[] contextNames
    Vector2 origin;
    string[] info;
    Interaction interaction;
    string[] contextNames;
    List<Body> alreadyHit;

    public EventSpawn(Vector2 _origin, Interaction _interaction, string[] _contextNames, string _subj, string _vb, string _obj = "", string _sup = "", 
        Body subject = null) {
        origin = _origin;
        interaction = _interaction;
        contextNames = _contextNames ?? new string[] { "middleburg"};
        //contextNames = (_contextNames.Length == 0) ? new string[] {"middleburg"} : _contextNames;
        info = (_obj.Equals("")) ? new string[] { _subj, _vb } : new string[] { _subj, _vb, _obj };
        if (_obj.Equals(""))
            info = new string[] { _subj, _vb };
        else if (_sup.Equals(""))
            info = new string[] { _subj, _vb, _obj };
        else info = new string[] { _subj, _vb, _obj, _sup};
        alreadyHit = new List<Body>();
        if (subject != null)
            alreadyHit.Add(subject);
        Cast(-1);
        Cast(1);
    }

    public void Cast(int dir) {
        RaycastHit2D[] perceivers = Physics2D.RaycastAll(origin, new Vector2(dir, 0));
        foreach (RaycastHit2D hit in perceivers) {
            float dist = Mathf.Abs(hit.collider.gameObject.transform.position.x - origin.x);
            Body bodhit = hit.collider.gameObject.GetComponent<Body>();
            if (bodhit == null || bodhit.GetPersonality() == null || alreadyHit.Contains(bodhit))
                continue;
            Personality personality = bodhit.GetPersonality();
            alreadyHit.Add(bodhit);
            bool seen = (dist < 12);
            personality.Perceive(info, interaction, contextNames, seen);
        }
    }

}
