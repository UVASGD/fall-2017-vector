using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void attackDelegate(Attack attack, int amount = 1);

public class AttackAct {
    public attackDelegate attackDel;
    GameObject sprite;
    int enactTime;
    int amount;
    public int EnactTime { get { return enactTime; } }

    public AttackAct(attackDelegate _attackDel, int _enactTime, GameObject _sprite = null, int _amount = 1) {
        attackDel = _attackDel;
        enactTime = --_enactTime;
        amount = _amount;
        sprite = _sprite;
    }

    public bool Check(int currTime, Attack _attack) {
        if (currTime >= enactTime)
            return Enact(_attack);
        return false;
    }

    public bool Enact(Attack _attack) {
        if (sprite != null)
            _attack.setSprite(sprite);
        attackDel(_attack, amount);
        return true;
    }
}
