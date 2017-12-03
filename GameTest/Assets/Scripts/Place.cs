using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour {

    float size;
    public float Size { get { return size; } }
    float coordinate;
    public float Coordinate { get { return coordinate; } }

	void Start () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        AI theMind = GetColliderAI(other);
        theMind.Place = this;

    }

    void OnTriggerExit2D(Collider2D other) {
        AI theMind = GetColliderAI(other);
        theMind.Place = null;
    }

    AI GetColliderAI(Collider2D other) {
        return other.gameObject.GetComponent<Body>().Mind;
    }

}
