using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;

public class Sword : Item, ICloseMelee {

    Resistance blockPower;
    float heavyTime = 0.5f;
    bool heavy = false;
    float power = 1f;

    int attackLim = 1;

    // GameObject dot;       <- Transferred up to ItemScripts
    GameObject newAttack;
    SpriteRenderer attackRender;
    List<GameObject> attacks = new List<GameObject>();

    public Sword(Body _holder, int _size) : base(_holder, _size, Color.gray) {
        name = "Sword";
        id = "sword";
        function = "weaponry";
        descriptor = "Well, at least it’s sharp. And durable-ish. You think. You haven’t actually tried bending it though, better not.";

        LightAttackList = new List<Affecter> { new Wound(holder, 0.2f) };
        lightAttackActScheme.Add(new AttackAct(Attack.Push, 3, newAttack));

        HeavyAttackList = new List<Affecter> { new Wound(holder, 0.4f),
                                               new Fire(holder, 5f)};
        heavyAttackActScheme.Add(new AttackAct(Attack.Push, 5));

        ParryList = new List<Affecter> { new Wound(holder, 0.1f) };

        blockPower = new Resistance(_holder, 1f);

        name = "Sword";
        function = "Weapon";
        // dot = Resources.Load("Bead", typeof(GameObject)) as GameObject; <-  transfered up to ItemScripts
    }

    public void CloseMeleeLightAttack() {
        SetupAttack(AddAttackScript<CloseMeleeLightAttackScript>("Bead", newAttack), LightAttackList, power);
    }

    public void CloseMeleeHeavyAttack() {
        SetupAttack(AddAttackScript<CloseMeleeHeavyAttackScript>("Bead", newAttack), HeavyAttackList);
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
        SetupAttack(AddAttackScript<CloseMeleeParryScript>("Bead"), ParryList);
    }

    public override void PlayerInput(MouseState _state) {
        // Debug.Log("State = leftUp:" + _state.leftUp + ";");
        // Debug.Log("Heavy Time: " + heavyTime);

        if (_state.cancel) {
            CancelAttack();
        }
        else if (_state.leftUp && _state.rightHold == 0f && _state.leftHold <= heavyTime) {
            // Debug.Log("heavyTime: " + heavyTime + " ; _state.leftHold: " + _state.leftHold + " ; power: " + heavyTime / _state.leftHold);
            if (_state.leftHold == 0f)
                power = 0.1f;
            else
                power = heavyTime / _state.leftHold;
            CloseMeleeLightAttack();
            Object.Destroy((Object)newAttack);
            // Debug.Log("Light Attack! Power: " + power);
        }
        else if (_state.leftUp && _state.rightHold == 0f && _state.leftHold > heavyTime) {
            // Debug.Log("Heavy Attack!");
            CloseMeleeHeavyAttack();
            heavy = false;
            holder.Impediment = ImpedimentLevel.unimpeded;
            Object.Destroy((Object)newAttack);
        }
        else if (_state.rightDown) {
            CloseMeleeBlockEnact();
        }
        else if (_state.rightUp) {
            CloseMeleeBlockDeact();
        }
        else if (_state.rightHold != 0f && _state.leftDown) {
            CloseMeleeParry();
        }
        else if (_state.leftDown) {
            StartAttack();
        }
        else if (_state.leftHold > 0f && newAttack != null) {
            if (_state.leftHold < heavyTime)
                newAttack.transform.localScale = new Vector3(1f, 2 * (_state.leftHold / heavyTime), 1f); //ATTACK EXPANDS TO YOUR HEIGHT
            else if (_state.leftHold > heavyTime && !heavy)
                HeavyEngage();
            UpdateAttackFace();
        }
        power = 1f;
    }

    public void StartAttack() {
        newAttack = Object.Instantiate(dot, holder.transform.position, holder.transform.rotation) as GameObject;
        newAttack.transform.parent = holder.transform;
        attackRender = newAttack.GetComponent<SpriteRenderer>();
        attackRender.sortingLayerName = "0";

        attackRender.color = Color.grey;

        UpdateAttackFace();
    }

    public void CancelAttack() {
        Object.Destroy((Object)newAttack);
    }

    public void UpdateAttackFace() {
        Vector3 thePos = holder.transform.position;

        if (holder.face == Direction.Left)
            thePos.x -= 1;
        else if (holder.face == Direction.Right)
            thePos.x += 1;

        newAttack.transform.position = thePos;
    }

    public void HeavyEngage() {
        heavy = true;
        attackRender.color = Color.red;
        holder.Impediment = ImpedimentLevel.noMove;
    }
}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}