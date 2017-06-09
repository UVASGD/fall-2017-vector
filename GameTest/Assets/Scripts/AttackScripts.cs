//IMPLEMENT STATUS EFFECTS/DAMAGE REPERCUSSIONS
//FIGURE OUT HOW HINDERING WORKS
//MAKE ON COLLISION HAPPEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

public enum DamageType { Crushing, Piercing, Slashing, Burning, Hindering, Magic};

// The Attack class operates by being placed on a GameObject by the Weapon object that
// generates it.
public class Attack : MonoBehaviour {

    int currFrame;
    int moveTime;
    int moveTimer;
    char[] moveScheme;
    Damage[] damages;
    TimeManager time;
    List<string> targetTags;
    List<GameObject> alreadyHit = new List<GameObject>();

    Direction dir;
    Thing genitor;

    bool done = true;

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        done = false;
    }

    void Update() {
        if (time.clock) {
            Tick();
        }

        if (done) {
            Destroy(gameObject);
        }
    }
   
    public void setTargetTags(List<string> _targetTags) {
        targetTags = _targetTags;
    }

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

    private void OnCollisionEnter2D(Collider2D other) {
        foreach (GameObject go in alreadyHit)
            if (go == other.gameObject)
                return;

        bool isTarget = false;
        foreach (string tag in other.gameObject.tags()) {
            foreach (string target in targetTags) {
                if (tag.Equals(target))
                    isTarget = true;
            }
        }

        if (isTarget) {
            Thing body = other.gameObject.GetComponent<Thing>();
            foreach (Damage dam in damages) {
                body.Damaged(dam);
            }
            alreadyHit.Add(other.gameObject);
        }
    }

}

public class Damage {

    public DamageType type;
    public int quant;
}
