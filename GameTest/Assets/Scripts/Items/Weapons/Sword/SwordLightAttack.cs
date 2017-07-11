using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLightAttack : Attack {
    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        done = false;
        moveScheme = new char[] { 'f', 'f', 'p', 'f' };
        moveTimes = new int[] { 3, 3, 3 };
        moveTimer = moveTimes[0];
        repeats = 0;
        effects = new Affecter[] { new Wound(genitor, 0.1f) };
    }

    public override void Move() {
        switch (moveScheme[currFrame++]) {
            case 'p':
                genitor.transform.Translate((int)dir, 0, 0);
                break;
            default:
                currFrame--;
                break;
        }
        base.Move();
    }
}