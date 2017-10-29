/*
 * Couriers
 * Economizers
 * East Gov
 * West Gov
 * Sanctum Arcanum
 * Those Knights, tho
 * Middleburg
 * Middler Monks
 * Mild Bandits
 * Wild Bandits
 * Cultists
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity {

    bool living;
    GameObject objectReference;
    string role;
    string place;
    //FactionAssoc faction;

    public Identity(GameObject _objectReference, string _role = "", string _place = "") {
        living = true;
        objectReference = _objectReference;
        role = _role;
        place = _place;
    }
}

/*
public class PersonIdentity : Identity { }

public class FactionIdentity : Identity { }

public class PlaceIdentiy : Identity { }

public class ItemIdentity : Identity { }
*/

