using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item {

    Reduction armorPower;

    public Armor(Body _holder, int _size, string _name, float power=1f) : base(_holder, _size, new Color(0.3f,0.3f,0.5f)) {
        armorPower = new Reduction(_holder, power);
        name = _name;
        function = "Armor";
    }

    public override void EquipTo(Body _holder) {
        holder = _holder;
        _holder.Armor = this;
    }

    public void Enact() {
        holder.AddAffecter(armorPower);
    }

    public void Deact() {
        holder.RemoveFromAffecterList(armorPower);
    }
}
