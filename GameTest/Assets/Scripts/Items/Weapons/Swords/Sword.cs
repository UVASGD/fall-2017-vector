using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item, ICloseMelee {

    public Sword(Body _holder, int _size) : base(_holder, _size) {
        LightAttackList = new List<Affecter> { new Wound(holder, 0.1f) };
        HeavyAttackList = new List<Affecter> { new Wound(holder, 0.4f),
                                               new Fire(holder, 5f)};
    }

    public void CloseMeleeLightAttack() {
        SetupAttack(AddAttackScript<CloseMeleeLightAttackScript>("Attack"), LightAttackList);
    }

    public void CloseMeleeHeavyAttack() {
        SetupAttack(AddAttackScript<CloseMeleeHeavyAttackScript>("Chest"), HeavyAttackList);
    }

}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}