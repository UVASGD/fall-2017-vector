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
        }
    }

    void ShowContent() {
        Debug.Log(others);
    }
}
