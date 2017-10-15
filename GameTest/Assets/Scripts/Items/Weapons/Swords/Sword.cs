using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : Item, ICloseMelee {

    Resistance blockPower;
    float heavyTime = 2f;
    float power = 1f;

    public Sword(Body _holder, int _size) : base(_holder, _size) {
        LightAttackList = new List<Affecter> { new Wound(holder, 0.2f) };
        lightAttackActScheme.Add(new AttackAct(Attack.Push, 3));

        HeavyAttackList = new List<Affecter> { new Wound(holder, 0.4f),
                                               new Fire(holder, 5f)};
        heavyAttackActScheme.Add(new AttackAct(Attack.Push, 5));

        ParryList = new List<Affecter> { new Wound(holder, 0.1f) };

        blockPower = new Resistance(_holder, 1f);

        name = "Sword";
    }

    public void CloseMeleeLightAttack() {
        SetupAttack(AddAttackScript<CloseMeleeLightAttackScript>("Attack"), LightAttackList, power);
    }

    public void CloseMeleeHeavyAttack() {
        SetupAttack(AddAttackScript<CloseMeleeHeavyAttackScript>("Chest"), HeavyAttackList);
    }

    public void CloseMeleeBlockEnact() {
        blockPower.Present = true;
        holder.AddAffecter(blockPower);
    }

    public void CloseMeleeBlockDeact() {
        //holder.RemoveFromAffecterList(blockPower);
        blockPower.Kill();
    }

    public void CloseMeleeParry() {
        SetupAttack(AddAttackScript<CloseMeleeParryScript>("Attack"), ParryList);
    }

    public override void PlayerInput(MouseState _state) {
        Debug.Log("State = leftUp:" + _state.leftUp + ";");
        Debug.Log("Heavy Time: " + heavyTime);

        if (_state.leftUp && _state.rightHold == 0f && _state.leftHold <= heavyTime) {
            Debug.Log("heavyTime: " + heavyTime + " ; _state.leftHold: " + _state.leftHold + " ; power: " + heavyTime / _state.leftHold);
            if (_state.leftHold == 0f)
                power = 0.1f;
            else
                power = heavyTime / _state.leftHold;
            CloseMeleeLightAttack();
            Debug.Log("Light Attack! Power: " + power);
        }
        else if (_state.leftUp && _state.rightHold == 0f && _state.leftHold > heavyTime) {
            Debug.Log("Heavy Attack!");
            CloseMeleeHeavyAttack();
        }
        power = 1f;
    }
}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}