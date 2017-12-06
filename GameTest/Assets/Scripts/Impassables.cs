using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impassable : MonoBehaviour {

    public Direction direct;

    void OnTriggerEnter2D(Collider2D other) {
        Body otherBody = other.GetComponent<Body>();
        if (otherBody != null) {
            otherBody.AddForbiddenDirect(direct);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Body otherBody = other.GetComponent<Body>();
        if (otherBody != null) {
            otherBody.RemoveForbiddenDirect(direct);
        }
    }
}
