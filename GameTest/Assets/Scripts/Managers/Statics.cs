using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Statics {

    public static float RandomLocIn(Place place) { return Random.Range(place.Coordinate-place.Size, place.Coordinate+place.Size); }

    public static T RandomElement <T> (List<T> list) { return list[Random.Range(0, list.Count)]; }

}


