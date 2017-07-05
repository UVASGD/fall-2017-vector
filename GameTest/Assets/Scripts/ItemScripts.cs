using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {
    protected Body holder;
    protected int size;
    protected List<Affecter> EquippedList;
    protected List<Affecter> HoldingList;
    protected List<Affecter> DroppedList;

    protected List<Attack> LightAttackList;
    protected List<Attack> HeavyAttackList;
    protected List<Attack> LightDashAttackList;
    protected List<Attack> HeavyDashAttackList;

    protected List<Attack> RangedAttackList;

    protected List<Attack> CastList;

    protected List<Attack> ThrowList;

    public Item(int _size, Affecter _essence) {
        size = _size;
    }
}
