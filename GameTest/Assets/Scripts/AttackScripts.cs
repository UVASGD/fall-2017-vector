//IMPLEMENT STATUS EFFECTS/DAMAGE REPERCUSSIONS
//FIGURE OUT HOW HINDERING WORKS
//MAKE ON COLLISION HAPPEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Crushing, Piercing, Slashing, Burning, Hindering, Magic};

// The Attack class operates by being placed on a GameObject by the Weapon object that
// generates it.
public class Attack : MonoBehaviour {

    int currFrame;
    int moveTime;
    int moveTimer;
    char[] moveScheme;
    Damage[] damages;

    Direction dir;
    Thing enactor;

    bool done;

    public void Tick() {
        if (moveTimer == 0 && !done) {
            Move();
            currFrame++;
            moveTimer = moveTime;
            if (currFrame >= moveScheme.Length)
                done = true;
        }
        else if (moveTimer != 0 && !done) {
            moveTimer--;
        }
    }

    public void Move() {
        switch (moveScheme[currFrame]) {
            case 'l':
                gameObject.transform.Translate(-1, 0, 0);
                break;
            case 'r':
                gameObject.transform.Translate(1, 0, 0);
                break;
            case 'f':
                gameObject.transform.Translate((int)dir, 0, 0);
                break;
            case 'b':
                gameObject.transform.Translate((int)dir * -1, 0, 0);
                break;
            case 'w':
                break;
        }
    }
}

public class Damage {

    public DamageType type;
    public int quant;
}
