using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeLightAttackScript : Attack {
    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        done = false;
        moveScheme = new char[] { 'f', 'f', 'p', 'f' };
        moveTimes = new int[] { rate, rate, 0, rate };
        moveTimer = moveTimes[0];
        repeats = 0;
        effects = new Affecter[] { new Wound(genitor, 0.1f) };
    }
}