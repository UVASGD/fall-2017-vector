using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item, ICloseMelee {

    public Sword(Body _holder, int _size) : base(_holder, _size) { }

    public void CloseMeleeLightAttack() {
        SetupAttack(AddAttackScript<CloseMeleeLightAttackScript>("Attack"));
    }

    public void CloseMeleeHeavyAttack() {
        SetupAttack(AddAttackScript<CloseMeleeHeavyAttackScript>("Chest"));
    }

}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}