using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TagFrenzy;

public enum DamageType { Crushing, Piercing, Slashing, Burning, Freezing, Electric, Hindering, Magic };

// The Attack class attaches to a prefab gameobject to be instantiated by an AttackAction
public class Attack : MonoBehaviour {

    public int[] moveTimes;
    int moveTimer;
    int currFrame = 0;
    char[] moveScheme;
    Affecter[] damages;
    TimeManager time;
    List<string> targetTags;
    List<GameObject> alreadyHit = new List<GameObject>();

    Direction dir;
    Body genitor;

    bool done = true;

    void Start() {
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        done = false;
        moveScheme = new char[] {'f', 'f', 'f', 'b', 'f', 'f', 'w'};
        moveTimes = new int[] {   4,   3,   2,   1,   2,   3,   4 };
        moveTimer = moveTimes[0];
        damages = new Affecter[] { };
    }

    void Update() {
        if (time.clock) {
            Tick();
        }

        // Deletes parent GameObject when finished
        if (done) {
            Destroy(gameObject);
        }
    }

    public void AttackConstructor(Body _genitor) {
        genitor = _genitor;
        dir = genitor.GetFace();
        targetTags = genitor.GetTargetTags();
        alreadyHit.Add(genitor.gameObject);
    }

    public void setTargetTags(List<string> _targetTags) {
        targetTags = _targetTags;
    }

    public void Tick() {
        if (moveTimer == 0 && !done) {

            if (currFrame >= moveScheme.Length) {
                done = true;
                return;
            }
            Move();
        }
        else if (moveTimer != 0 && !done) {
            moveTimer--;
        }
    }

    public void Move() {
        switch (moveScheme[currFrame++]) {
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
        if (currFrame < moveTimes.Length)
            moveTimer = moveTimes[currFrame];
    }

    private void OnTriggerEnter2D(Collider2D other) {
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
            Body body = other.gameObject.GetComponent<Body>();
            foreach (Affecter dam in damages) {
                body.AddAffecter(dam);
            }
            alreadyHit.Add(other.gameObject);
        }
    }

}