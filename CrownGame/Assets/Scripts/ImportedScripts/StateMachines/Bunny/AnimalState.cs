using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalState {

    // TODO: Use abstract classes instead of Bunny

    public abstract void Enter(Bunny animal, ref Vector3 velocity);

    public abstract void Exit(Bunny animal);

    public abstract void Update(Bunny animal, ref Vector2 velocity);

}
