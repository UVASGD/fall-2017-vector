﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    protected Body holder;
    protected int size;
    protected int speed;
    protected Affecter material;

    protected bool natural;

    protected string name = "Generic Item";
    public string Name { get { return name; } }

    protected List<Affecter> EquippedList;
    protected List<Affecter> HoldingList;
    protected List<Affecter> DroppedList;

    protected List<Affecter> LightAttackList;
    protected List<AttackAct> lightAttackActScheme = new List<AttackAct>();
    public List<AttackAct> LightScheme { get { return lightAttackActScheme; } }

    protected List<Affecter> HeavyAttackList;
    protected List<AttackAct> heavyAttackActScheme = new List<AttackAct>();
    public List<AttackAct> HeavyScheme { get { return heavyAttackActScheme; } }

    protected List<Affecter> LightDashAttackList;
    protected List<AttackAct> lightDashAttackActScheme = new List<AttackAct>();
    public List<AttackAct> LightDashScheme { get { return lightDashAttackActScheme; } }

    protected List<Affecter> HeavyDashAttackList;
    protected List<AttackAct> heavyDashAttackActScheme = new List<AttackAct>();
    public List<AttackAct> HeavyDashScheme { get { return heavyDashAttackActScheme; } }

    protected List<Affecter> ParryList;
    protected List<AttackAct> parryActScheme = new List<AttackAct>();
    public List<AttackAct> ParryScheme { get { return parryActScheme; } }

    protected List<Affecter> RangedAttackList;

    protected List<Affecter> CastList;

    protected List<Affecter> ThrowList;

    public Item(Body _holder, int _size, bool _natural=false) {
        holder = _holder;
        size = _size;
        speed = 5;
        natural = _natural;
    }

    protected T AddAttackScript<T>(string resource, GameObject _sprite=null) where T : Component {
        GameObject attackObj;
        if (_sprite != null)
            attackObj = _sprite;
        else
            attackObj = (GameObject)Resources.Load(resource);
        //_sprite.transform.parent = attackObj.transform;
        Vector3 pos = new Vector3(holder.transform.position.x + (((int)holder.face) * holder.Size), -3.5f, 0);
        var newAttackObject = Object.Instantiate(attackObj, pos, Quaternion.identity) as GameObject;

        newAttackObject.AddComponent<T>();

        T attackScript = newAttackObject.GetComponent<T>();
        return attackScript;
    }

    protected void SetupAttack(Attack _attack, List<Affecter> _effects, float _power=1f) {
        //if (_sprite != null)
        //    _sprite.transform.parent = _attack.transform;
        _attack.AttackConstructor(holder, speed, _power);
        _attack.effects = _effects;
        for (int i = 0; i < _effects.Count; i++) {
            Affecter effect = _effects[i];
            effect.ResetVitality();
            _effects[i] = effect.GetAffecterClone(effect);
        }
        holder.SetCurrAct(new Recovery("Recovery", _attack.Rate * _attack.Duration, holder));
    }

    public virtual void PlayerInput(MouseState _state) {

    }
}

public interface ICloseMelee {
    void CloseMeleeLightAttack();

    void CloseMeleeHeavyAttack();

    void CloseMeleeBlockEnact();

    void CloseMeleeBlockDeact();

    void CloseMeleeParry();
}