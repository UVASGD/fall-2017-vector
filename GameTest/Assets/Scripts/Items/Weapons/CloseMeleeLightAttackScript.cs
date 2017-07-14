using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeLightAttackScript : Attack {
    public override void AttackConstructor(Body _genitor, int _speed) {
        base.AttackConstructor(_genitor, _speed);
        rate *= 3;
        moveScheme = new char[] { 'f', 'f', 'f', 'p' };
        moveTimes = new int[] { rate, rate, rate, 0 };
        duration = 3;
        moveTimer = moveTimes[0];
    }
}