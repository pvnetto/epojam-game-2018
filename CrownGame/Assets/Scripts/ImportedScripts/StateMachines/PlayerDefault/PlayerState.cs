using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    protected PlayerController controller;
    protected Controller2D controller2D;
    protected Player owner;

    private float stompSpeed = 25.0f;
    private float sideBumpSpeed = 12.0f;

    protected Controller2D.CollisionInfo collisionInfo {
        get {
            return controller2D.collisionInfo;
        }
    }

    public PlayerState(Player owner) {
        this.owner = owner;
        controller = owner.controller;
        controller2D = controller.controller2D;
    }

    public virtual void Hit(GameObject attacker, ref HitRecord hitRecord) {
        Hit(attacker, ref hitRecord, Vector2.zero);
    }

    public virtual void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        hitRecord.hitObject = owner;
        hitRecord.hitObjectID = owner.playerID;
        hitRecord.reflected = false;

        controller.player.Knockback(knockbackForce);
    }

    public virtual void Execute(ref Vector2 inputs, ref Vector3 velocity) {
        HandleCCs(ref inputs);
        StateUpdate(ref inputs, ref velocity);
        KnockPlayer(ref velocity);
    }

    protected virtual void HandleCCs(ref Vector2 inputs) {
        if (owner.isStunned) {
            inputs = Vector2.zero;
        }
    }

    protected virtual void KnockPlayer(ref Vector3 velocity) {
        if (collisionInfo.playerLeft || collisionInfo.playerRight) {
            Player playerHit = collisionInfo.playerHit;
            playerHit.Knockback(new Vector2(sideBumpSpeed, 0) * collisionInfo.faceDirection);
        }
        if (collisionInfo.playerBelow) {
            velocity.y = stompSpeed;

            Player playerHit = collisionInfo.playerHit;

            if (!owner.isAlly(playerHit.playerID)) {
                HitRecord hitRecord = new HitRecord();
                playerHit.Hit(owner.gameObject, ref hitRecord);
            }
        }
    }

    protected abstract void StateUpdate(ref Vector2 inputs, ref Vector3 velocity);

}
