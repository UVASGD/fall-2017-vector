using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchZone : MonoBehaviour {

    List<Body> others;
    public string Name;

    void OnTriggerEnter2D(Collider2D other) {
        AddColliderBody(other);
        ShowContent();
    }

    void OnTriggerExit2D(Collider2D other) {
        others.Remove(GetColliderBody(other));
        ShowContent();
    }

    Body GetColliderBody(Collider2D other) {
        return other.gameObject.GetComponent<Body>();
    }

    void AddColliderBody(Collider2D other) {
        if (other.gameObject.GetComponent<Body>() != null) {
            others.Add(other.gameObject.GetComponent<Body>());
            Body otherBody = other.gameObject.GetComponent<Body>();
            new EventSpawn(otherBody.gameObject.transform.position, new Interaction(), null, otherBody.Id, "approaches", Name, subject:otherBody, _scope:40);
        }
    }
    //new EventSpawn(genitor.transform.position, new Interaction(100, 100), null,
    //  subj.Id, "is associated with", primeAssoc.Id, subject:genitor);
    void ShowContent() {
        Debug.Log(others);
    }
}
