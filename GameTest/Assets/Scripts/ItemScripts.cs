using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    protected int size;
    protected Affecter essence;
    protected bool equipped;
    protected bool present;

    public Item(int _size, Affecter _essence) {
        size = _size;
        essence = _essence;
    }



}
