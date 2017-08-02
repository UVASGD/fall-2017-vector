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
 * 
 * 
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identity {

    List<Mark> marks;
    public Mark[] Marks { get { return marks.ToArray(); } }

    

    public void AddMark(Mark _mark) {
        marks.Add(_mark);
    }

    public bool RemoveMark(Mark _mark) {
        return marks.Remove(_mark);     
    }

}

public class PersonIdentity : Identity { }

public class FactionIdentity : Identity { }

public class PlaceIdentiy : Identity { }

public class ItemIdentity : Identity { }

