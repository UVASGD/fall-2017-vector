﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item {

    Reduction armorPower;

    public Armor(Body _holder, int _size, string _name) : base(_holder, _size) {
        armorPower = new Reduction(_holder, 1f);
        name = _name;
    }

    public override void EquipTo(Body _holder) {
        holder = _holder;
        _holder.Armor = this;
    }

}
