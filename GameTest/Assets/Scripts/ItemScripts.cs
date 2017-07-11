using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    protected Body holder;
    protected int size;
    protected int speed;
    protected Affecter material;

    protected List<Affecter> EquippedList;
    protected List<Affecter> HoldingList;
    protected List<Affecter> DroppedList;

    protected List<Attack> LightAttackList;
    protected List<Attack> HeavyAttackList;
    protected List<Attack> LightDashAttackList;
    protected List<Attack> HeavyDashAttackList;

    protected List<Attack> RangedAttackList;

    protected List<Attack> CastList;

    protected List<Attack> ThrowList;

    public Item(Body _holder, int _size) {
        holder = _holder;
        size = _size;
        speed = 5;
    }

    protected T AddAttackScript<T>(string resource) where T : Component {
        GameObject attackObj = (GameObject)Resources.Load(resource);
        Vector3 pos = new Vector3(holder.transform.position.x + (2 * ((int)holder.face)), 0, 0);
        var newAttackObject = Object.Instantiate(attackObj, pos, Quaternion.identity) as GameObject;

        newAttackObject.AddComponent<T>();

        T attackScript = newAttackObject.GetComponent<T>();
        return attackScript;
    }

    protected void SetupAttack(Attack _attack) {
        _attack.AttackConstructor(holder, speed);
        holder.SetCurrAct(new Recovery("Recovery", _attack.Rate * _attack.Duration, holder));
    }
}

public interface ICloseMelee {
    void CloseMeleeLightAttack();

    void CloseMeleeHeavyAttack();
}