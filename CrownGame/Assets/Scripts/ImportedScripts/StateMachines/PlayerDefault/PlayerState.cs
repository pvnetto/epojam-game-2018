using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    public abstract void Enter(Player player, ref Vector3 velocity);

    public abstract void Exit(Player player);

    public virtual void Hit(Player player, GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        hitRecord.hitObject = player;
        hitRecord.hitObjectID = player.playerID;
        hitRecord.reflected = false;

        player.Knockback(knockbackForce);
    }

    public virtual void Execute(Player player, ref Vector2 inputs, ref Vector3 velocity) {
        HandleCCs(player, ref inputs);
        Update(player, ref inputs, ref velocity);
    }

    protected virtual void HandleCCs(Player player, ref Vector2 inputs) {
        if (player.isStunned) {
            inputs = Vector2.zero;
        }
    }

    protected abstract void Update(Player player, ref Vector2 inputs, ref Vector3 velocity);

}
