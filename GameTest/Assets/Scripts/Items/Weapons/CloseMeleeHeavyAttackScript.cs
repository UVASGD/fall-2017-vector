using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeHeavyAttackScript : Attack {
    public override void AttackConstructor(Body _genitor, int _speed) {
        base.AttackConstructor(_genitor, _speed);
        rate *= 5;
        moveScheme = new char[] { 'w', 'f', 'f', 'f', 'f', 'p' };
        moveTimes = new int[] { rate, rate, rate, rate, rate, 0 };
        duration = 5;
        moveTimer = moveTimes[0];
    }
}