using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterclockwiseEntity : SymbolEntity
{
    public override void transitionToEnable()
    {
        Invoke("entityDie", 5f);
        Sounds.ƒÊ ±’Î∆Ù∂Ø.playWithPitch();
        base.transitionToEnable();
    }
}
