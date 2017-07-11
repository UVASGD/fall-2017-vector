using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Item, ICloseMelee {

    public Sword(Body _holder, int _size) : base(_holder, _size) { }

    public void CloseMeleeLightAttack() {
        Vector3 newPos = new Vector3();
        newPos = holder.transform.position;
        Debug.Log(newPos.x);

        if (holder.GetFace() == Direction.Left) {
            newPos += new Vector3(-2, 0);
        }
        else {
            newPos += new Vector3(2, 0);
        }

        GameObject attackObj = (GameObject)Resources.Load("Attack");

        var newAttackObject = Object.Instantiate(attackObj, newPos, Quaternion.identity) as GameObject;
        newAttackObject.AddComponent<SwordLightAttack>();

        SpriteRenderer attackRender = newAttackObject.GetComponent<SpriteRenderer>();
        attackRender.sortingLayerName = "0";

        SwordLightAttack lightAttack = newAttackObject.GetComponent<SwordLightAttack>();
        lightAttack.AttackConstructor(holder);

        holder.SetCurrAct(new Recovery("Recovery", 10 - holder.Athletics, holder));
    }

    public void CloseMeleeHeavyAttack() {

    }
}

public class SwordMat : Affecter {

    public SwordMat(Body _targetBody, float _vitality, float _vRate) : base(_targetBody, _vitality, _vRate) { }
}

public class SwordHeavyAttack : Attack { }


