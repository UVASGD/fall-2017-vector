using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseMeleeParryScript : Attack {
    public override void AttackConstructor(Body _genitor, int _speed) {
        base.AttackConstructor(_genitor, _speed);
        targetTags.Add("Attack");
        rate *= 3;
        actScheme = genitor.Weapon.ParryScheme;
        moveScheme = new char[] { 'f', 'f'};
        moveTimes = new int[] { rate, rate};
        duration = 3;
        moveTimer = moveTimes[0];
        actTimer = 0;
    }
}
