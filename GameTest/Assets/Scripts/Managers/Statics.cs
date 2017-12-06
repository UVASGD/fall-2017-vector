using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics {

    public static float RandomLocIn(Place place) { return Random.Range(place.Coordinate-(place.Size/2), place.Coordinate+(place.Size/2)); }

    public static T RandomElement <T> (List<T> list) { if (list.Count > 0) return list[Random.Range(0, list.Count)]; else return default(T); }

}


