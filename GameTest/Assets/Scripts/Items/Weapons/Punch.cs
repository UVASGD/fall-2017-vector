using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : Attack {

    public override void AttackConstructor(Body _genitor, int _speed, float _power = 1f) {
        base.AttackConstructor(_genitor, _speed);
        rate *= 2;
        actScheme = genitor.Weapon.LightScheme;
        moveScheme = new char[] { 'f', 'f' };
        moveTimes = new int[] { rate, rate };
        duration = 2;
        moveTimer = moveTimes[0];
        actTimer = 0;
    }
}
