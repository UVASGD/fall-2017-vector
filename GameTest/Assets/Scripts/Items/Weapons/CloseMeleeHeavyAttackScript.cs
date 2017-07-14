using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeHeavyAttackScript : Attack {
    public override void AttackConstructor(Body _genitor, int _speed) {
        base.AttackConstructor(_genitor, _speed);
        rate *= 5;
        moveScheme = new char[] { 'w', 'f', 'f', 'f', 'p', 'f' };
        moveTimes = new int[] { rate, rate, rate, rate, 0, rate };
        duration = 5;
        moveTimer = moveTimes[0];
    }
}