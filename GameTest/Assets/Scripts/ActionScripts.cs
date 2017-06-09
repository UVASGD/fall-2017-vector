using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

    public string name;
    protected Action nextAction;
    protected int timeLeft;
    protected Body genitor;

    public Action(string _name, int _timeLeft, Body _genitor) {
        name = _name;
        timeLeft = _timeLeft;
        genitor = _genitor;
    }

    protected virtual void Dewit() {
        if (nextAction != null) {
            genitor.SetCurrAct(nextAction);
            return;
        }

        nextAction = new Action("Open", 0, genitor);
    }

    public void Tick() {
        timeLeft--;

        if (timeLeft <= 0)
            Dewit();
    }
}

public class MoveAction : Action {

    int iterLeft;
    Direction dir;

    public MoveAction(string _name, int _timeLeft, Body _genitor, Direction _dir, int _iterLeft) : base(_name, _timeLeft, _genitor) {
        iterLeft = _iterLeft;
        dir = _dir;

        if (iterLeft > 0)
            nextAction = new MoveAction(_name, _timeLeft, _genitor, _dir, _iterLeft - 1);

        else {
            nextAction = new Action("Open", 0, _genitor);
        }
    }

    protected override void Dewit() {
        genitor.Move(dir);
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class AttackAction : Action {

    Direction dir;
    GameObject attack;

    public AttackAction(string _name, int _timeLeft, Body _genitor, GameObject _attack) : base(_name, _timeLeft, _genitor) {
        dir = genitor.getDir();
        attack = _attack;
    }

    protected override void Dewit() {
        Vector3 newPos = new Vector3();
        newPos = genitor.transform.position;
        GameObject newAttackObject = (GameObject) MonoBehaviour.Instantiate(attack, newPos, Quaternion.identity);
        Attack newAttackScript = newAttackObject.GetComponent<Attack>();
        newAttackScript.setTargetTags(genitor.GetTargetTags());

        base.Dewit();
    }
}