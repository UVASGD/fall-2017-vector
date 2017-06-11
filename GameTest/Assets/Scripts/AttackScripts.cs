//IMPLEMENT STATUS EFFECTS/DAMAGE REPERCUSSIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TagFrenzy;

//public enum DamageType { Crushing, Piercing, Slashing, Burning, Hindering, Magic};

public class Damage {

    public DamageType type;
    public int quant;

    public Damage(DamageType _type, int _quant) {
        type = _type;
        quant = _quant;
    }
}