using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicEventHandler {
    string[] publicEvent = { "", "", "" };
    int tick = 0;

    public void setEvent(string[] _publicEvent) {
        publicEvent = _publicEvent;
        tick = 1;
    }
    
}
