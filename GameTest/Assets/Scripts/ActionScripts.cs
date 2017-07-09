using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

    public string name;
    protected Action nextAction;
    protected int timeLeft;
    protected Body genitor;
    protected float speedMod = 1f;

    public Action(string _name, int _speedFactor, Body _genitor) {
        name = _name;
        timeLeft = (int) (_speedFactor * speedMod);
        genitor = _genitor;
    }

    protected virtual void Dewit() {
        if (nextAction != null) {
            genitor.SetCurrAct(nextAction);
            return;
        }

        nextAction = new Action("Open", 0, genitor);
    }

    public virtual void Tick() {
        timeLeft--;

        if (timeLeft <= 0)
            Dewit();
    }
}

public class MoveAction : Action {

    int iterLeft;
    Direction dir;

    public MoveAction(string _name, int _speedFactor, Body _genitor, Direction _dir, int _iterLeft) : base(_name, _speedFactor, _genitor) {
        iterLeft = _iterLeft;
        dir = _dir;

        if (iterLeft > 0)
            nextAction = new MoveAction(_name, _speedFactor, _genitor, _dir, _iterLeft - 1);
        else {
            nextAction = new HaltAction("Halt", 0, _genitor);
        }
    }

    protected override void Dewit() {
        genitor.Move(dir);
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class HaltAction : Action {
    public HaltAction(string _name, int _speedFactor, Body _genitor) :
        base(_name, _speedFactor, _genitor) {
        timeLeft = 100;
        nextAction = this;
    }

    protected override void Dewit() {
        timeLeft = 100;
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class AttackAction : Action {

    GameObject attack;

    public AttackAction(string _name, int _speedFactor, Body _genitor, GameObject _attack) : base(_name, _speedFactor, _genitor) {
        attack = _attack;
    }

    protected override void Dewit() {
        Vector3 newPos = new Vector3();
        newPos = genitor.transform.position;

        if (genitor.GetFace() == Direction.Left) {
            newPos += new Vector3(-2, 0);
        }
        else {
            newPos += new Vector3(2, 0);
        }

        GameObject newAttackObject = Object.Instantiate(attack, newPos, Quaternion.identity);

        SpriteRenderer attackRender = newAttackObject.GetComponent<SpriteRenderer>();
        attackRender.sortingLayerName = "0";

        Attack newAttackScript = newAttackObject.GetComponent<Attack>();
        newAttackScript.AttackConstructor(genitor);

        nextAction = new Action("Open", 0, genitor);
        base.Dewit();
    }
}

public class ShortMeleeLightAttack : Action {

    public ShortMeleeLightAttack(string _name, int _speedFactor, Body _genitor) : base(_name, _speedFactor, _genitor) {
    }
}

public class Recovery : Action {
    protected enum ImpedimentLevel { unimpeded, noAttack, noAct, noMove};

    Affecter curImpediment;

    // TODO: make these Affecters
    Affecter noMove;  // = new MoveDenier ??
    Affecter noAct;  //  = new ActionDenier  ??
    Affecter noAttack;  //  = new AttackDenier  ??

    ImpedimentLevel curImpLevel = ImpedimentLevel.unimpeded;

    public Recovery(string _name, int _speedFactor, Body _genitor) : base(_name, _speedFactor, _genitor) {
    }

    public override void Tick() {
        base.Tick();
        if (timeLeft > 20 && curImpLevel == ImpedimentLevel.unimpeded) {
            ApplyImpediment(ImpedimentLevel.noMove);
        }
        else if (timeLeft > 10) {
            ApplyImpediment(ImpedimentLevel.noAct);
        }
        else if (timeLeft > 0) {
            ApplyImpediment(ImpedimentLevel.noAttack);
        }
    }

    protected void ApplyImpediment(ImpedimentLevel level) {
        if (curImpLevel != ImpedimentLevel.unimpeded) {
            genitor.RemoveFromAffecterList(curImpediment);
        }

        switch (level) {
            case ImpedimentLevel.noMove: curImpediment = noMove; break;
            case ImpedimentLevel.noAct: curImpediment = noAct; break;
            case ImpedimentLevel.noAttack: curImpediment = noAttack; break;
        }

        genitor.AddAffecter(curImpediment);
    }
}