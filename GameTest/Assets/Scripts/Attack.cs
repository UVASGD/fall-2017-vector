using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

// The Attack class attaches to a prefab gameobject to be instantiated by an AttackAction
public class Attack : MonoBehaviour {
    protected Vector3 distance;
    public int[] moveTimes;
    protected int moveTimer;
    protected char[] moveScheme;
    protected List<AttackAct> actScheme;
    protected int actTimer;
    protected int currFrame = 0;
    protected int rate;
    public int Rate { get { return rate; } }
    protected int duration;
    public int Duration { get { return duration; } }
    protected int repeats = 0;
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
            Act();
            Move();
        }
        else if (moveTimer != 0 && !done) {
            moveTimer--;
        }
    }

    public virtual void Move() {
        switch (moveScheme[currFrame++]) {
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

    public virtual void Act() {
        if (actTimer < actScheme.Count) {
            AttackAct a = actScheme[actTimer];
            if (a.Check(currFrame, this)) {
                actTimer++;
            }
        }
    }

    public static void Push(Attack a, int amount = 1) {
        if (a.genitor.GetCurrMoveAct().name.Equals("Halt"))
            a.genitor.transform.Translate(amount * (int)a.dir, 0, 0);
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