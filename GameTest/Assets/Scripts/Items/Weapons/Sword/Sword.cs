using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item, ICloseMelee {

    public Sword(Body _holder, int _size) : base(_holder, _size) { }

    public void CloseMeleeLightAttack() {
        GameObject attackObj = (GameObject)Resources.Load("Attack");
        Vector3 pos = new Vector3(holder.transform.position.x + (2 * ((int)holder.face)), 0, 0);
        var newAttackObject = Object.Instantiate(attackObj, pos, Quaternion.identity) as GameObject;

        newAttackObject.AddComponent<CloseMeleeLightAttackScript>();

        CloseMeleeLightAttackScript lightAttack = newAttackObject.GetComponent<CloseMeleeLightAttackScript>();
        lightAttack.AttackConstructor(holder, speed);

        holder.SetCurrAct(new Recovery("Recovery", 10 - holder.Athletics, holder));
    }

    public void CloseMeleeHeavyAttack() {

    }
}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}


