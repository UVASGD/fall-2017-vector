using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Listener {

    string[] infoCheck;
    int priority = 0;
    Personality perceiver;
    int timer = 500;
    public bool temp;

    public Listener(string[] _infoCheck, Personality _perceiver, bool _temp = true, int _priority = 0) {
        infoCheck = _infoCheck;
        perceiver = _perceiver;
        priority = _priority;
        temp = _temp;
    }

    public bool CheckMatch(List<Association> info) {
        if (info.Count != infoCheck.Length)
            return false;
        foreach (Association a in info) {
            foreach (string element in infoCheck) {
                if (element.Equals("x")) {
                    continue;
                }
                else if (element.StartsWith(".")) {
                    Association checkAssoc = perceiver.GetAssociation(element.Substring(1, element.Length));
                    if (!a.marks.ContainsKey(checkAssoc))
                        return false;
                }
                else if (!element.Equals(a.Id))
                    return false;
            }
        }

        return true;
    }
}
