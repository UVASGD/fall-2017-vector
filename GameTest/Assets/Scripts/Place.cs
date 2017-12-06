using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : MonoBehaviour {

    public string placeName;
    float size;
    public float Size { get { return size; } }
    float coordinate;
    public float Coordinate { get { return coordinate; } }
    bool bears = false;

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
        if (placeName.Equals("Forest") && other.gameObject.name.Equals("Player") && !bears) {
            bears = true;
            PersonCreator Bear = new PersonCreator("Bear", "bear", Coordinate+15, 2, AINum.turret, null);
            new EventSpawn(gameObject.transform.position, new Interaction(0, 0), null, "bear", "brawls");
            PersonCreator Bear2 = new PersonCreator("Bear", "bear", Coordinate + 20, 2, AINum.turret, null);
            new EventSpawn(gameObject.transform.position, new Interaction(0, 0), null, "bear", "brawls");
            PersonCreator Bear3 = new PersonCreator("Bear", "bear", Coordinate + 25, 2, AINum.turret, null);
            new EventSpawn(gameObject.transform.position, new Interaction(0, 0), null, "bear", "brawls");
            PersonCreator Bear4 = new PersonCreator("Bear", "bear", Coordinate + 35, 2, AINum.turret, null);
            new EventSpawn(gameObject.transform.position, new Interaction(0, 0), null, "bear", "brawls");
            PersonCreator Bear5 = new PersonCreator("Bear", "bear", Coordinate + 45, 2, AINum.turret, null);
            new EventSpawn(gameObject.transform.position, new Interaction(0, 0), null, "bear", "brawls");
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        AI theMind = GetColliderAI(other);
        theMind.Place = null;
    }

    AI GetColliderAI(Collider2D other) {
        return other.gameObject.GetComponent<Body>().Mind;
    }

}
