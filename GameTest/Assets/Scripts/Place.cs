using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour {

    public string placeName;
    float size;
    public float Size { get { return size; } }
    float coordinate;
    public float Coordinate { get { return coordinate; } }

    public MusicChoice music = MusicChoice.Null;

    public Place(string _name) {
        name = _name;
    }

	void Start () {
        BoxCollider2D thisCollider = gameObject.GetComponent<BoxCollider2D>();
        size = thisCollider.size.x;
        coordinate = gameObject.transform.position.x;
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
