using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left = -1, Right = 1};

public class Thing : MonoBehaviour {

    Action currAct;
    Action currMoveAct;

    float harmQuant;
    float harmThreshold;

    float hinderQuant;
    float hinderThreshold;

    float[] damMods = { 1f, 1f, 1f, 1f, 1f};

    public void Moved(Direction dir) {
        switch (dir) {
            case Direction.Left:
                gameObject.transform.Translate(-1, 0, 0);
                break;
            case Direction.Right:
                gameObject.transform.Translate(1, 0, 0);
                break;
        }
    }

    public void SetCurrAct(Action a) {
        currAct = a;
    }

    public void SetCurrMoveAct(Action mA) {
        currMoveAct = mA;
    }

    public void Harmed(float delt) {
        harmQuant += delt;
    }

    public void Hindered(float delt) {
        hinderQuant += delt;
    }

    public void Damaged(Damage dam) {
        Harmed(dam.quant * damMods[(int)dam.type]);
    }
}
