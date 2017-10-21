﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawn {
    //string[] info, Interaction interaction, string[] contextNames
    Vector2 origin;
    string subj;
    string obj;
    string vb;
    string[] info;
    Interaction interaction;
    string[] contextNames;

    public EventSpawn(Vector2 _origin, Interaction _interaction, string[] _contextNames, string _subj, string _vb, string _obj = "") {
        origin = _origin;
        interaction = _interaction;
        contextNames = (_contextNames.Length == 0) ? new string[] {"middleburg"} : _contextNames;
        info = (obj.Equals("")) ? new string[] { _subj, _vb } : new string[] { _subj, _vb, _obj };
        Cast(-1);
        Cast(1);
    }

    public void Cast(int dir) {
        RaycastHit2D[] perceivers = Physics2D.RaycastAll(origin, new Vector2(dir, 0));
        foreach (RaycastHit2D hit in perceivers) {
            float dist = Mathf.Abs(hit.collider.gameObject.transform.position.x - origin.x);
            Body bodhit = hit.collider.gameObject.GetComponent<Body>();
            if (bodhit == null || bodhit.GetPersonality() == null)
                continue;
            Personality personality = bodhit.GetPersonality();
            bool seen = (dist > 12);
            personality.Perceive(info, interaction, contextNames, seen);
        }
    }

}