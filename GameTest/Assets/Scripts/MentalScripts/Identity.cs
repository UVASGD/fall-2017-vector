using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity {

    public bool living;
    public bool gone;
    public bool generic;
    public GameObject objectReference;
    string id;
    public string Id { get { return id; } }
    string role;
    public string Role { get { return role; } }
    string place;
    public string Place { get { return place; } }

    public Identity(string _id, bool _generic = true, string _role = "", string _place = "", GameObject _objectReference = null) {
        gone = false;
        living = true;
        objectReference = _objectReference;
        id = _id;
        generic = _generic;
        role = _role;
        place = _place;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().TheKnow.AddToAll(this);
    }

    public void ChangeId(string newId) {
        id = newId;
        
    }
}

/*
public class PersonIdentity : Identity { }

public class FactionIdentity : Identity { }

public class PlaceIdentiy : Identity { }

public class ItemIdentity : Identity { }
*/

