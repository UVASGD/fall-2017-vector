using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TagFrenzy;

public enum DamageType { Crushing, Piercing, Slashing, Burning, Freezing, Electric, Hindering, Magic };

// The Attack class attaches to a prefab gameobject to be instantiated by an AttackAction
public class Attack : MonoBehaviour {

    public int[] moveTimes;
    protected int moveTimer;
    protected Vector3 distance;
    protected int currFrame = 0;
    protected int rate;
    public int Rate { get { return rate; } }
    protected int duration;
    public int Duration { get { return duration; } }
    protected int repeats = 0;
    protected char[] moveScheme;
    public List<Affecter> effects;
    protected TimeManager time;
    protected List<string> targetTags;
    protected List<GameObject> alreadyHit = new List<GameObject>();

    protected Direction dir;
    protected Body genitor;

    protected bool done = false;

    void Update() {
        if (time.clock) {
            Tick();
        }
        // Deletes parent GameObject when finished
        if (done) {
            Destroy(gameObject);
        }
    }

    public virtual void AttackConstructor(Body _genitor, int _speed) {
        distance = new Vector3();
        distance.x = (2 * ((int)_genitor.face));
        time = (TimeManager)FindObjectOfType(typeof(TimeManager));
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "0";
        genitor = _genitor;
        dir = genitor.GetFace();
        targetTags = genitor.TargetTags;
        alreadyHit.Add(genitor.gameObject);
        rate = ((5-_speed) + (10 - _genitor.Athletics)) / 2;
    }

    public void setTargetTags(List<string> _targetTags) {
        targetTags = _targetTags;
    }

    public void Tick() {
        transform.position = genitor.transform.position+distance;
        if (moveTimer == 0 && !done) {

            if (currFrame >= moveScheme.Length && repeats <= 0) {
                done = true;
                return;
            }
            else if (currFrame >= moveScheme.Length && repeats > 0) {
                currFrame = 0;
                repeats--;
            }

            Move();
        }
        else if (moveTimer != 0 && !done) {
            moveTimer--;
        }
    }

    public virtual void Move() {
        switch (moveScheme[currFrame++]) {
            case 'p':
                if (genitor.GetCurrMoveAct().name.Equals("Halt"))
                    genitor.transform.Translate((int)dir, 0, 0);
                break;
            case 'l':
                distance.x += -1;
                break;
            case 'r':
                distance.x += 1;
                break;
            case 'f':
                distance.x += (int)dir;
                break;
            case 'b':
                distance.x += -(int)dir;
                break;
            case 'w':
                break;
        }
        if (currFrame < moveTimes.Length)
            moveTimer = moveTimes[currFrame];
    }

    private void OnTriggerEnter2D(Collider2D other) {
        foreach (GameObject go in alreadyHit)
            if (go == other.gameObject) {
                return;
            }

        bool isTarget = false;
        foreach (string tag in other.gameObject.tags()) {
            foreach (string target in targetTags) {
                if (tag.Equals(target))
                    isTarget = true;
            }
        }

        if (isTarget) {
            Body body = other.gameObject.GetComponent<Body>();
            foreach (Affecter eff in effects) {
                body.AddAffecter(eff);
            }
            alreadyHit.Add(other.gameObject);
        }
    }

}