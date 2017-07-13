using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeHeavyAttackScript : Attack {
    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        rate *= 5;
        moveScheme = new char[] { 'w', 'f', 'f', 'f', 'p', 'f' };
        moveTimes = new int[] { rate, rate, rate, rate, 0, rate };
        duration = 5;
        moveTimer = moveTimes[0];
        //effects = new List<Affecter> { new Wound(genitor, 0.4f) };
    }
}