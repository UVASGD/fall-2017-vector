using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {

    protected string name;
    protected Action nextAction;
    protected int timeLeft;
    protected Thing genitor;

    public Action(string _name, int _timeLeft, Thing _genitor) {
        name = _name;
        timeLeft = _timeLeft;
        genitor = _genitor;
    }

    protected virtual void Dewit() {
        if (nextAction != null)
            genitor.SetCurrAct(nextAction); 
    }

    public void Tick() {
        timeLeft--;

        if (timeLeft == 0)
            Dewit();
    }
}

public class MoveAction : Action {

    int iterLeft;
    Direction dir;

    public MoveAction(string _name, int _timeLeft, Thing _genitor, Direction _dir, int _iterLeft) : base(_name, _timeLeft, _genitor) {
        iterLeft = _iterLeft;
        dir = _dir;

        if (iterLeft > 0)
            nextAction = new MoveAction(_name, _timeLeft, _genitor, _dir, _iterLeft - 1);

        else {
            nextAction = new Action("Open", 1, _genitor);
        }
    }

    protected override void Dewit() {
        genitor.Moved(dir);
        genitor.SetCurrMoveAct(nextAction);
    }
}

public class AttackAction : Action {

    Direction dir;
    GameObject attack;

    public AttackAction(string _name, int _timeLeft, Thing _genitor, Direction _dir, GameObject _attack) : base(_name, _timeLeft, _genitor) {
        dir = _dir;
        attack = _attack;
    }

    protected override void Dewit() {
        base.Dewit();
    }
}