using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyPunch : Attack {
    public override void AttackConstructor(Body _genitor, int _speed, float _power = 1f) {
        base.AttackConstructor(_genitor, _speed);
        rate *= 3;
        actScheme = genitor.Weapon.HeavyScheme;
        moveScheme = new char[] { 'w', 'f', 'f' };
        moveTimes = new int[] { rate, rate, rate };
        duration = 3;
        moveTimer = moveTimes[0];
        actTimer = 0;
    }
}
