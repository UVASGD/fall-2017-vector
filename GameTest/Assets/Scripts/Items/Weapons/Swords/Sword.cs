using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : Item, ICloseMelee {

    public Sword(Body _holder, int _size) : base(_holder, _size) {
        LightAttackList = new List<Affecter> { new Wound(holder, 0.1f) };
        lightAttackActScheme.Add(new AttackAct(Attack.Push, 3));

        HeavyAttackList = new List<Affecter> { new Wound(holder, 0.4f),
                                               new Fire(holder, 5f)};
        heavyAttackActScheme.Add(new AttackAct(Attack.Push, 5));
    }

    public void CloseMeleeLightAttack() {
        SetupAttack(AddAttackScript<CloseMeleeLightAttackScript>("Attack"), LightAttackList);
    }

    public void CloseMeleeHeavyAttack() {
        SetupAttack(AddAttackScript<CloseMeleeHeavyAttackScript>("Chest"), HeavyAttackList);
    }

    public void CloseMeleeBlockEnact() {

    }

    public void CloseMeleeBlockDeact() {

    }
}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}